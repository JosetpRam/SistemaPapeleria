namespace Papeleria.Entidades
{
    // Representa un registro de la tabla public.producto.
    // Contiene datos; las consultas SQL se implementan en la capa Datos.
    public class Producto
    {
        // Identificador generado por PostgreSQL al insertar el registro.
        public int ProductoId { get; set; }

        // Código del producto: hasta 30 caracteres en la base de datos.
        public string Codigo { get; set; } = string.Empty;

        // Nombre del producto: hasta 150 caracteres en la base de datos.
        public string Nombre { get; set; } = string.Empty;

        // Identificador de la categoría a la que pertenece el producto.
        public int CategoriaId { get; set; }

        // Precio de venta. decimal permite representar valores monetarios.
        public decimal PrecioVenta { get; set; }

        // Cantidad mínima deseada; la tabla admite tres decimales.
        // No representa las existencias actuales del inventario.
        public decimal ExistenciaMinima { get; set; }

        // Indica si el producto está habilitado para su uso.
        public bool Activo { get; set; } = true;

        // Corresponde a fecha_creacion (timestamptz).
        // Al consultar, Datos debe asignar la fecha recibida en UTC.
        // Al insertar, se puede dejar que PostgreSQL genere la fecha.
        public DateTime FechaCreacion { get; set; }
    }
}
