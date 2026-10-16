/* PASO 1 - Ejecutar en Query Tool conectado a la base postgres.
 Mantener Auto-commit activado: CREATE DATABASE no admite una transaccion explicita.
 Ejecutar una sola vez. Si papeleriadb ya existe, PostgreSQL informa el error y
 conserva su contenido. No borrar una base existente para volver a ejecutar.
 Despues: actualizar Databases y abrir NUEVO Query Tool sobre papeleriadb.
 Ejecutar alli 01_Papeleria_PostgreSQL_Estructura.sql y luego 02_...DatosPrueba.sql.
 Documentacion: https://www.postgresql.org/docs/18/sql-createdatabase.html
*/
CREATE DATABASE papeleriadb WITH TEMPLATE template0 ENCODING 'UTF8';
