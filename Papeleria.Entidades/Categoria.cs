namespace Papeleria.Entidades;

// Opción del selector de categorías del producto.
public class Categoria
{
    public int CategoriaId { get; set; }
    public string Nombre { get; set; } = "";
    public bool Activo { get; set; }
    public string Descripcion => Activo ? Nombre : Nombre + " (inactiva)";
}
