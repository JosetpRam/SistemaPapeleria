using Papeleria.Entidades;

namespace Papeleria.Datos
{
    // Consulta la tabla producto y transforma sus filas en objetos Producto.
    public class ProductoDatos
    {
        private readonly Conexion _conexion;

        // Recibe una conexión configurada para reutilizarla en las consultas.
        // Quien crea Conexion es responsable de liberarla al terminar.
        public ProductoDatos(Conexion conexion)
        {
            ArgumentNullException.ThrowIfNull(conexion);
            _conexion = conexion;
        }

        // Devuelve todos los productos, incluidos los inactivos, ordenados por código.
        // Si no hay registros, devuelve una lista vacía; los errores se propagan.
        public async Task<List<Producto>> ListarAsync()
        {
            const string sql = """
                SELECT producto_id, codigo, nombre, categoria_id,
                       precio_venta, existencia_minima, activo, fecha_creacion
                FROM public.producto
                ORDER BY codigo, producto_id;
                """;

            var productos = new List<Producto>();

            // El origen de datos administra la conexión necesaria para ejecutar SQL.
            await using var comando = _conexion.CrearComando(sql);
            await using var lector = await comando.ExecuteReaderAsync();

            // ReadAsync avanza una fila cada vez, sin bloquear la interfaz.
            while (await lector.ReadAsync())
            {
                // Los índices comienzan en cero y corresponden al orden del SELECT.
                // Estas ocho columnas están definidas como NOT NULL en la tabla.
                var producto = new Producto
                {
                    ProductoId = lector.GetInt32(0),
                    Codigo = lector.GetString(1),
                    Nombre = lector.GetString(2),
                    CategoriaId = lector.GetInt32(3),
                    PrecioVenta = lector.GetDecimal(4),
                    ExistenciaMinima = lector.GetDecimal(5),
                    Activo = lector.GetBoolean(6),
                    FechaCreacion = lector.GetDateTime(7)
                };

                productos.Add(producto);
            }

            // await using libera el lector y el comando, incluso si hay un error.
            return productos;
        }

        public async Task<List<Categoria>> ListarCategoriasAsync()
        {
            await using var cmd = _conexion.CrearComando(
                "SELECT categoria_id, nombre, activo FROM public.categoria ORDER BY nombre");
            await using var rd = await cmd.ExecuteReaderAsync();
            var lista = new List<Categoria>();
            while (await rd.ReadAsync())
                lista.Add(new Categoria { CategoriaId = rd.GetInt32(0), Nombre = rd.GetString(1), Activo = rd.GetBoolean(2) });
            return lista;
        }

        // Las condiciones originales evitan sobrescribir cambios de otra ventana.
        private const string CoincideOriginal = """
            producto_id = @id AND codigo = @oc AND nombre = @on
            AND categoria_id = @ocat AND precio_venta = @op
            AND existencia_minima = @om AND activo = @oa AND fecha_creacion = @of
            """;

        private static void Original(Npgsql.NpgsqlCommand cmd, Producto p)
        {
            cmd.Parameters.AddWithValue("id", p.ProductoId);
            cmd.Parameters.AddWithValue("oc", p.Codigo);
            cmd.Parameters.AddWithValue("on", p.Nombre);
            cmd.Parameters.AddWithValue("ocat", p.CategoriaId);
            cmd.Parameters.AddWithValue("op", p.PrecioVenta);
            cmd.Parameters.AddWithValue("om", p.ExistenciaMinima);
            cmd.Parameters.AddWithValue("oa", p.Activo);
            cmd.Parameters.AddWithValue("of", p.FechaCreacion);
        }

        public async Task<int> GuardarAsync(Producto p, Producto? original)
        {
            // La categoría debe estar activa, salvo conservar la categoría original.
            string categoriaValida = """
                EXISTS (SELECT 1 FROM public.categoria c WHERE c.categoria_id = @cat
                        AND (c.activo OR c.categoria_id = @anterior))
                """;
            string sql = original == null
                ? "INSERT INTO public.producto (codigo,nombre,categoria_id,precio_venta,existencia_minima,activo) " +
                  "SELECT @codigo,@nombre,@cat,@precio,@minimo,@activo WHERE " + categoriaValida + " RETURNING producto_id"
                : "UPDATE public.producto SET codigo=@codigo,nombre=@nombre,categoria_id=@cat," +
                  "precio_venta=@precio,existencia_minima=@minimo,activo=@activo WHERE " + CoincideOriginal +
                  " AND " + categoriaValida + " RETURNING producto_id";
            await using var cmd = _conexion.CrearComando(sql);
            cmd.Parameters.AddWithValue("codigo", p.Codigo);
            cmd.Parameters.AddWithValue("nombre", p.Nombre);
            cmd.Parameters.AddWithValue("cat", p.CategoriaId);
            cmd.Parameters.AddWithValue("precio", p.PrecioVenta);
            cmd.Parameters.AddWithValue("minimo", p.ExistenciaMinima);
            cmd.Parameters.AddWithValue("activo", p.Activo);
            cmd.Parameters.AddWithValue("anterior", original?.CategoriaId ?? 0);
            if (original != null) Original(cmd, original);
            try
            {
                object? id = await cmd.ExecuteScalarAsync();
                if (id == null) throw new InvalidOperationException(
                    "No se guardó: la categoría no está disponible o el producto cambió. Cancele y actualice la lista.");
                return (int)id;
            }
            catch (Npgsql.PostgresException ex) when (ex.SqlState == "23505")
            { throw new InvalidOperationException("Ya existe un producto con ese código."); }
            catch (Npgsql.PostgresException ex) when (ex.SqlState == "23503")
            { throw new InvalidOperationException("La categoría ya no existe. Actualice la lista."); }
        }

        public async Task EliminarAsync(Producto original)
        {
            await using var cmd = _conexion.CrearComando("DELETE FROM public.producto WHERE " + CoincideOriginal);
            Original(cmd, original);
            try
            {
                if (await cmd.ExecuteNonQueryAsync() != 1)
                    throw new InvalidOperationException("El producto cambió o fue eliminado. Actualice antes de continuar.");
            }
            catch (Npgsql.PostgresException ex) when (ex.SqlState == "23503")
            { throw new InvalidOperationException("Este producto tiene movimientos asociados. Puede desactivarlo desde Editar."); }
        }
    }
}
