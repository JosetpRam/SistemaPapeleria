using Papeleria.Datos;
using Papeleria.Entidades;

namespace Papeleria.Negocio
{
    // Ofrece a Presentación las operaciones relacionadas con productos.
    // Las consultas SQL permanecen en ProductoDatos.
    public class ProductoNegocio
    {
        // Devuelve todos los productos, incluidos los inactivos.
        // La contraseña se recibe como parámetro; no se guarda en el código.
        public async Task<List<Producto>> ListarAsync(string clave)
        {
            // Una clave ausente es un problema de configuración.
            // No se recorta: los espacios pueden formar parte de la contraseña.
            if (string.IsNullOrEmpty(clave))
            {
                throw new ArgumentException(
                    "Debe configurar la contraseña de PostgreSQL.", nameof(clave));
            }

            // Sigue el mismo patrón utilizado en ConexionNegocio para esta práctica.
            // using libera el origen de datos cuando termina la operación.
            using var conexion = new Conexion(clave);
            var datos = new ProductoDatos(conexion);

            // await espera a que Datos termine de leer antes de liberar Conexion.
            // Si no hay filas, devuelve una lista vacía. Si ocurre un error,
            // se propaga a Presentación para que pueda informar al usuario.
            return await datos.ListarAsync();
        }

        public async Task<List<Categoria>> ListarCategoriasAsync(string clave)
        {
            ValidarClave(clave);
            using var conexion = new Conexion(clave);
            return await new ProductoDatos(conexion).ListarCategoriasAsync();
        }

        private static void ValidarClave(string clave)
        {
            if (string.IsNullOrEmpty(clave)) throw new ArgumentException("Configure la contraseña de PostgreSQL.");
        }

        // Las reglas se verifican también fuera del formulario.
        public static void Validar(Producto p)
        {
            ArgumentNullException.ThrowIfNull(p);
            p.Codigo = (p.Codigo ?? "").Trim();
            p.Nombre = (p.Nombre ?? "").Trim();
            if (p.Codigo.Length is < 1 or > 30) throw new ArgumentException("El código debe tener entre 1 y 30 caracteres.");
            if (p.Nombre.Length is < 1 or > 150) throw new ArgumentException("El nombre debe tener entre 1 y 150 caracteres.");
            if (p.CategoriaId <= 0) throw new ArgumentException("Seleccione una categoría.");
            if (p.PrecioVenta < 0 || p.PrecioVenta > 9999999999999999.99m || decimal.Round(p.PrecioVenta, 2) != p.PrecioVenta)
                throw new ArgumentException("Precio inválido: use un valor no negativo con hasta dos decimales.");
            if (p.ExistenciaMinima < 0 || p.ExistenciaMinima > 999999999999999.999m || decimal.Round(p.ExistenciaMinima, 3) != p.ExistenciaMinima)
                throw new ArgumentException("Existencia mínima inválida: use un valor no negativo con hasta tres decimales.");
        }

        public async Task<int> GuardarAsync(string clave, Producto p, Producto? original = null)
        {
            ValidarClave(clave);
            Validar(p);
            if (original != null && (original.ProductoId <= 0 || p.ProductoId != original.ProductoId))
                throw new ArgumentException("Identificador de producto inválido.");
            if (original == null && p.ProductoId != 0) throw new ArgumentException("El nuevo producto no debe tener ID.");
            using var conexion = new Conexion(clave);
            return await new ProductoDatos(conexion).GuardarAsync(p, original);
        }

        public async Task EliminarAsync(string clave, Producto original)
        {
            ValidarClave(clave);
            ArgumentNullException.ThrowIfNull(original);
            if (original.ProductoId <= 0) throw new ArgumentException("Seleccione un producto.");
            using var conexion = new Conexion(clave);
            await new ProductoDatos(conexion).EliminarAsync(original);
        }
    }
}
