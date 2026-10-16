# Sistema Papelería — Solución en Cuatro Capas con C#, PostgreSQL y DevExpress

Este proyecto implementa el sistema integral de gestión de papelería estructurado en arquitectura limpia de cuatro capas, siguiendo rigurosamente las guías de los 4 manuales de laboratorio:

1. **Manual 1: PostgreSQL y Arquitectura en Cuatro Capas**
   - Configuración de la base de datos `papeleriadb`.
   - Scripts SQL: creación de base (`00`), estructura relacional (`01`), y carga de 6,501 registros de prueba (`02`).
   - Creación de la solución `Sistema Papeleria.sln` y los cuatro proyectos en .NET 8:
     - `Papeleria.Entidades` (Biblioteca de clases)
     - `Papeleria.Datos` (Biblioteca de clases)
     - `Papeleria.Negocio` (Biblioteca de clases)
     - `Papeleria.Presentacion` (Windows Forms)
   - Configuración de dependencias unidireccionales y control de versiones Git.

2. **Manual 2: Conexión de Cuatro Capas con Npgsql**
   - Integración del controlador `Npgsql` en `Papeleria.Datos`.
   - Implementación de `Conexion.cs` utilizando `NpgsqlDataSource` optimizado.
   - Capa de negocio `ConexionNegocio.cs` para orquestar la llamada.
   - `Form1.cs` con botón de **Test de conexión** asíncrono, protegiendo las credenciales mediante la variable de entorno `PAPELERIA_DB_PASSWORD`.

3. **Manual 3: Consulta y Filtrado de Productos con DevExpress**
   - Entidad `Producto.cs` en `Papeleria.Entidades`.
   - Métodos asíncronos en `ProductoDatos.cs` y `ProductoNegocio.cs` para consultar la lista de productos.
   - Creación de `FormProducto.cs` con componentes DevExpress (`GridControl`, `GridView`) configurando columnas formateadas para ID, Código, Producto, Categoría, Precio de venta (`n2`), Existencia mínima (`n3`), Activo y Creación en UTC.
   - Actualización de `Form1.cs` para incluir el botón **Abrir productos**.

4. **Manual 4: De Consulta a CRUD Completo de Productos**
   - Entidad `Categoria.cs` para dar soporte a la selección de categoría.
   - Ampliación de `ProductoDatos.cs` con operaciones transaccionales y seguras contra concurrencia optimista (`CoincideOriginal`):
     - `ListarAsync()`, `ListarCategoriasAsync()`
     - `GuardarAsync()` (manejo de INSERT y UPDATE según identificador y validación de categorías activas)
     - `EliminarAsync()` (con verificación de integridad referencial de PostgreSQL, código de error `23503`)
   - Capa de negocio `ProductoNegocio.cs` con validación estricta de dominios de negocio (longitudes, importes no negativos, existencia mínima válida y clave de base de datos).
   - Formulario de edición modal `FormProductoEdicion.cs` creado íntegramente por código C# con soporte de escalado DPI, controles DevExpress (`LookUpEdit`, `SpinEdit`, `TextEdit`, `CheckEdit`, `SimpleButton`), protección de cambios no guardados al cerrar y doble modo (Nuevo / Edición).
   - Integración en `FormProducto.cs` de las acciones CRUD: **Nuevo**, **Editar**, **Eliminar** y **Actualizar**, con confirmaciones de borrado y refresco automático.

---

## Estructura de Proyectos y Dependencias

```
Sistema Papeleria.sln
│
├── Papeleria.Entidades
│   ├── Producto.cs
│   └── Categoria.cs
│
├── Papeleria.Datos  (Referencia a: Entidades | Paquete: Npgsql)
│   ├── Conexion.cs
│   └── ProductoDatos.cs
│
├── Papeleria.Negocio (Referencia a: Datos, Entidades)
│   ├── ConexionNegocio.cs
│   └── ProductoNegocio.cs
│
├── Papeleria.Presentacion (Referencia a: Negocio, Entidades | Paquete: DevExpress.Win.Design 26.1.5)
│   ├── Program.cs
│   ├── Form1.cs & Form1.Designer.cs
│   ├── FormProducto.cs & FormProducto.Designer.cs & FormProducto.resx
│   └── FormProductoEdicion.cs
│
└── scripts/
    ├── 00_Papeleria_PostgreSQL_CrearBase.sql
    ├── 01_Papeleria_PostgreSQL_Estructura.sql
    └── 02_Papeleria_PostgreSQL_DatosPrueba.sql
```

---

## Configuración y Ejecución

1. **Configurar la base de datos PostgreSQL**:
   - Ejecutar en Query Tool de pgAdmin o mediante `psql`:
     1. `scripts/00_Papeleria_PostgreSQL_CrearBase.sql` (en base `postgres`)
     2. `scripts/01_Papeleria_PostgreSQL_Estructura.sql` (en base `papeleriadb`)
     3. `scripts/02_Papeleria_PostgreSQL_DatosPrueba.sql` (en base `papeleriadb`)

2. **Configurar la variable de entorno**:
   - En Windows (PowerShell):
     ```powershell
     [Environment]::SetEnvironmentVariable("PAPELERIA_DB_PASSWORD", "tu_contraseña_postgres", "User")
     ```

3. **Compilar y Ejecutar**:
   - Compilación:
     ```powershell
     dotnet build "Sistema Papeleria.sln"
     ```
   - Ejecución:
     ```powershell
     dotnet run --project Papeleria.Presentacion/Papeleria.Presentacion.csproj
     ```
