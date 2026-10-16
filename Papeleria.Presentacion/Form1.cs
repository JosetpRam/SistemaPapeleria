// Permite utilizar ConexionNegocio, definida en la capa Negocio.
using Papeleria.Negocio;

namespace Papeleria.Presentacion
{
    public partial class Form1 : Form
    {
        // Constructor: prepara los controles definidos en el diseñador.
        public Form1()
        {
            InitializeComponent();
        }

        // Ejecuta la prueba únicamente al pulsar Test de conexión.
        // async permite esperar la consulta sin bloquear la interfaz.
        private async void btnTestConexion_Click(object sender, EventArgs e)
        {
            // Deshabilita los botones para evitar pruebas simultáneas.
            btnTestConexion.Enabled = false;
            btnProductos.Enabled = false;

            try
            {
                // Lee la contraseña de la configuración del usuario de Windows.
                // string? permite null si la variable no existe; no se escribe la clave en el código.
                string? clave = Environment.GetEnvironmentVariable(
                    "PAPELERIA_DB_PASSWORD", EnvironmentVariableTarget.User);

                // Antes de conectar, comprueba que exista una contraseña configurada.
                if (string.IsNullOrEmpty(clave))
                {
                    MessageBox.Show(this,
                        "Falta configurar la variable PAPELERIA_DB_PASSWORD en tu usuario de Windows.",
                        "Configuración pendiente", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    // Termina la prueba; el formulario permanece abierto.
                    return;
                }

                // Presentación solicita la operación a Negocio, que utiliza la capa Datos.
                var negocio = new ConexionNegocio();
                // Espera el resultado de SELECT current_database(): el nombre de la base.
                string baseDatos = await negocio.ProbarAsync(clave);

                if (IsDisposed || Disposing) return;

                // Muestra el éxito solo cuando la consulta termina correctamente.
                MessageBox.Show(this, $"Conexión exitosa a {baseDatos}.",
                    "Prueba de conexión", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            // Captura los errores de la prueba y permite informar al usuario.
            catch (Exception ex)
            {
                if (IsDisposed || Disposing) return;
                MessageBox.Show(this,
                    // Muestra el tipo de error sin incluir la contraseña.
                    $"No se pudo conectar. Revisa el servicio PostgreSQL, el puerto y la contraseña.\nTipo de error: {ex.GetType().Name}",
                    "Error de conexión", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                if (!IsDisposed && !Disposing)
                {
                    btnTestConexion.Enabled = true;
                    btnProductos.Enabled = true;
                }
            }
        }

        // Abre Productos por separado; al cerrarlo se vuelve a la prueba de conexión.
        private void btnProductos_Click(object sender, EventArgs e)
        {
            using var formulario = new FormProducto();
            formulario.ShowDialog(this);
        }

        // La carga inicial no realiza consultas; cada botón ejecuta su operación.
        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
