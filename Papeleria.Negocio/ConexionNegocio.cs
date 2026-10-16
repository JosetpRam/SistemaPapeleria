using Papeleria.Datos;

namespace Papeleria.Negocio;

public class ConexionNegocio
{
    public async Task<string> ProbarAsync(string clave)
    {
        using var conexion = new Conexion(clave);
        return await conexion.ProbarConexionAsync();
    }
}
