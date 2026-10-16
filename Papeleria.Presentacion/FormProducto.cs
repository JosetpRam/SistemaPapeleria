using DevExpress.Utils;
using DevExpress.XtraEditors;
using Papeleria.Entidades;
using Papeleria.Negocio;

namespace Papeleria.Presentacion
{
    public partial class FormProducto : XtraForm
    {
        // Evita iniciar otra consulta mientras la anterior sigue en curso.
        private bool _cargando;

        public FormProducto()
        {
            InitializeComponent();
            ConfigurarColumnas();
            FormClosing += (_, e) => { if (_cargando) e.Cancel = true; };
        }

        // Los FieldName deben coincidir con las propiedades de Producto.
        private void ConfigurarColumnas()
        {
            vistaProductos.Columns.AddVisible(nameof(Producto.ProductoId), "ID");
            vistaProductos.Columns.AddVisible(nameof(Producto.Codigo), "Código");
            vistaProductos.Columns.AddVisible(nameof(Producto.Nombre), "Producto");
            vistaProductos.Columns.AddVisible(nameof(Producto.CategoriaId), "ID categoría");
            var precio = vistaProductos.Columns.AddVisible(nameof(Producto.PrecioVenta), "Precio de venta");
            precio.DisplayFormat.FormatType = FormatType.Numeric;
            precio.DisplayFormat.FormatString = "n2";
            var minimo = vistaProductos.Columns.AddVisible(nameof(Producto.ExistenciaMinima), "Existencia mínima");
            minimo.DisplayFormat.FormatType = FormatType.Numeric;
            minimo.DisplayFormat.FormatString = "n3";
            vistaProductos.Columns.AddVisible(nameof(Producto.Activo), "Activo");
            var fecha = vistaProductos.Columns.AddVisible(nameof(Producto.FechaCreacion), "Creación (UTC)");
            fecha.DisplayFormat.FormatType = FormatType.DateTime;
            fecha.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            vistaProductos.Columns[nameof(Producto.Nombre)].Width = 260;
            vistaProductos.Columns[nameof(Producto.ProductoId)].Width = 55;
        }

        // Carga la lista al mostrar este formulario por primera vez.
        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);
            await CargarProductosAsync();
        }

        private async void btnActualizar_Click(object sender, EventArgs e)
        {
            await CargarProductosAsync();
        }

        private async Task CargarProductosAsync()
        {
            if (_cargando || IsDisposed || Disposing) return;
            _cargando = true;
            CambiarBotones(false);
            UseWaitCursor = true;
            lblEstado.Text = "Consultando productos...";

            try
            {
                // Lee la clave local sin guardarla en el código ni mostrarla.
                string? clave = Environment.GetEnvironmentVariable(
                    "PAPELERIA_DB_PASSWORD", EnvironmentVariableTarget.User);
                if (string.IsNullOrEmpty(clave))
                {
                    lblEstado.Text = "Falta configurar la contraseña local.";
                    XtraMessageBox.Show(this,
                        "Configura PAPELERIA_DB_PASSWORD en tu usuario de Windows y pulsa Actualizar.",
                        "Configuración pendiente", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var negocio = new ProductoNegocio();
                var productos = await negocio.ListarAsync(clave);
                // El usuario puede cerrar el formulario mientras se espera al servidor.
                if (IsDisposed || Disposing) return;

                // GridControl recibe la lista; GridView define cómo se presenta.
                gridProductos.DataSource = productos;
                lblEstado.Text = productos.Count == 0
                    ? "No hay productos registrados."
                    : $"{productos.Count} productos cargados | Actualizado: {DateTime.Now:HH:mm:ss}";
            }
            catch (Exception ex)
            {
                if (IsDisposed || Disposing) return;
                // Si falla una actualización, conserva los últimos datos visibles.
                lblEstado.Text = "No se pudo actualizar. Los datos visibles pueden estar desactualizados.";
                XtraMessageBox.Show(this,
                    $"Revisa PostgreSQL y la configuración de conexión.\nTipo de error: {ex.GetType().Name}",
                    "Error al consultar productos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _cargando = false;
                if (!IsDisposed && !Disposing)
                {
                    CambiarBotones(true);
                    UseWaitCursor = false;
                }
            }
        }

        // Se conserva el evento del diseñador; la consulta se inicia en OnShown.
        private void FormProducto_Load(object sender, EventArgs e)
        {
        }

        private void CambiarBotones(bool habilitar)
        {
            btnActualizar.Enabled = btnNuevo.Enabled = btnEditar.Enabled = btnEliminar.Enabled = habilitar;
            gridProductos.Enabled = habilitar;
        }

        private async void btnNuevo_Click(object sender, EventArgs e) => await AbrirEditorAsync(null);
        private async void btnEditar_Click(object sender, EventArgs e)
        {
            if (vistaProductos.GetFocusedRow() is Producto p) await AbrirEditorAsync(p);
            else XtraMessageBox.Show(this, "Seleccione un producto en la tabla.", "Editar");
        }

        private async Task AbrirEditorAsync(Producto? original)
        {
            if (_cargando) return;
            _cargando = true; CambiarBotones(false); UseWaitCursor = true;
            bool guardado = false;
            try
            {
                string clave = Environment.GetEnvironmentVariable("PAPELERIA_DB_PASSWORD", EnvironmentVariableTarget.User) ?? "";
                var categorias = await new ProductoNegocio().ListarCategoriasAsync(clave);
                UseWaitCursor = false;
                using var editor = new FormProductoEdicion(original, categorias);
                guardado = editor.ShowDialog(this) == DialogResult.OK;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(this, "No se pudo abrir el editor. Revise la conexión. Tipo: " + ex.GetType().Name, "Productos");
            }
            finally { _cargando = false; CambiarBotones(true); UseWaitCursor = false; }
            if (guardado)
            {
                // Quita filtros para que también sea visible el producto recién creado.
                vistaProductos.ClearColumnsFilter(); vistaProductos.FindFilterText = "";
                await CargarProductosAsync();
            }
        }

        private async void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_cargando) return;
            if (vistaProductos.GetFocusedRow() is not Producto p)
            { XtraMessageBox.Show(this, "Seleccione un producto.", "Eliminar"); return; }
            if (XtraMessageBox.Show(this, $"¿Eliminar definitivamente {p.Codigo} - {p.Nombre}?\nEsta acción no se puede deshacer.",
                "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;
            _cargando = true; CambiarBotones(false); UseWaitCursor = true;
            bool eliminado = false;
            try
            {
                string clave = Environment.GetEnvironmentVariable("PAPELERIA_DB_PASSWORD", EnvironmentVariableTarget.User) ?? "";
                await new ProductoNegocio().EliminarAsync(clave, p);
                eliminado = true;
            }
            catch (Exception ex)
            {
                string mensaje = ex is InvalidOperationException or ArgumentException ? ex.Message :
                    "No se pudo confirmar la eliminación. Actualice antes de reintentar. Tipo: " + ex.GetType().Name;
                XtraMessageBox.Show(this, mensaje, "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally { _cargando = false; CambiarBotones(true); UseWaitCursor = false; }
            if (eliminado) await CargarProductosAsync();
        }
    }
}
