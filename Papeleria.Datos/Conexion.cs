using Npgsql;

namespace Papeleria.Datos;

public sealed class Conexion : IDisposable 
{
    private readonly NpgsqlDataSource _origenDatos;

    public Conexion(string clave)
    {
        var configuracion = new NpgsqlConnectionStringBuilder
        {
            Host = "localhost",
            Port = 5432,
            Database = "papeleriadb",
            Username = "postgres",
            Password = clave,
            Timeout = 5,
            CommandTimeout = 5
        };

        _origenDatos = NpgsqlDataSource.Create(configuracion.ConnectionString);
    }

    public async Task<string> ProbarConexionAsync()
    {
        await using var comando = _origenDatos.CreateCommand(
            "SELECT current_database();");
        var resultado = await comando.ExecuteScalarAsync();
        return resultado?.ToString() ?? string.Empty;
    }

    // Permite a las clases de Datos crear comandos con la configuración existente.
    // internal limita este método al proyecto Papeleria.Datos.
    // Quien recibe el comando debe liberarlo con using o await using.
    internal NpgsqlCommand CrearComando(string sql)
    {
        return _origenDatos.CreateCommand(sql);
    }

    public void Dispose() => _origenDatos.Dispose();
}
