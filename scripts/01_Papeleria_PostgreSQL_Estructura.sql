/* PAPELERIA - ESTRUCTURA PARA POSTGRESQL 18
 1. Ejecutar 00_Papeleria_PostgreSQL_CrearBase.sql conectado a postgres.
 2. En pgAdmin, actualizar Databases y abrir Query Tool sobre papeleriadb.
 3. Ejecutar TODO este archivo (no solamente una seleccion).
 4. Ejecutar 02_Papeleria_PostgreSQL_DatosPrueba.sql en papeleriadb.

 Nombres en minusculas y snake_case; esquema public. No requiere extensiones.
 9 tablas, 11 claves foraneas y 6 vistas; sin eliminaciones en cascada.
 No reemplaza tablas. Un error revierte la creacion de todo el esquema.
 Si pgAdmin deja la transaccion abortada, ejecutar ROLLBACK; antes de reintentar.
 Fechas TIMESTAMPTZ: representan instantes; la sesion decide como mostrarlos.
 vw_ventas_por_dia agrupa por fecha UTC, como el ejercicio original.

 PasswordHash: usar una biblioteca de hash de contrasenas con sal (ej. Identity).
 Crear el primer administrador desde la aplicacion, nunca con clave publica fija.
 Los roles de esta tabla son de la aplicacion, no roles de conexion PostgreSQL.
 Login y codigo de producto son unicos sin distinguir mayusculas/minusculas.
 Las demas comparaciones de texto siguen la configuracion de PostgreSQL.

 REGLAS PENDIENTES EN LA CAPA DE NEGOCIO O PROCEDIMIENTOS:
 Validar usuario activo y permisos; guardar cabecera y detalle en una transaccion;
 confirmar solo documentos con detalles; verificar stock y bloquear los productos
 afectados al vender; impedir cambios de detalles confirmados/anulados;
 verificar stock al anular compras. Las FK y CHECK no imponen estas reglas entre
 filas. Usar cuenta de conexion con permisos limitados en la aplicacion.
 Inventario = compras confirmadas - ventas confirmadas. Entrada inicial mediante
 compra de tipo INVENTARIO_INICIAL. Alcance: contado, una moneda, sin impuestos
 ni facturacion fiscal. Subtotales redondeados por renglon a dos decimales.
*/
BEGIN;
SET LOCAL TIME ZONE 'UTC';
SET LOCAL search_path = public, pg_catalog;
DO $verificar$
BEGIN
 IF current_database() <> 'papeleriadb' THEN
   RAISE EXCEPTION 'Abra Query Tool sobre papeleriadb antes de ejecutar este archivo.';
 END IF;
 IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema='public' AND table_type='BASE TABLE') THEN
   RAISE EXCEPTION 'El esquema public ya contiene tablas. No se modifico el esquema existente.';
 END IF;
END;
$verificar$;

    create table public.usuario
    (
        usuario_id integer generated always as identity not null constraint pk_usuario primary key,
        nombre_completo varchar(100) not null,
        nombre_usuario varchar(50) not null,
        password_hash varchar(512) not null,
        rol varchar(20) not null,
        activo boolean not null constraint df_usuario_activo default (true),
        fecha_creacion timestamptz(0) not null constraint df_usuario_fecha default (current_timestamp),
        ultimo_acceso timestamptz(0) null,
        constraint ck_usuario_nombre check (length(ltrim(rtrim(nombre_completo))) > 0),
        constraint ck_usuario_login check (length(ltrim(rtrim(nombre_usuario))) > 0
            and octet_length(nombre_usuario) = octet_length(ltrim(rtrim(nombre_usuario)))),
        constraint ck_usuario_hash check (length(ltrim(rtrim(password_hash))) > 0),
        constraint ck_usuario_rol check (rol in ('Administrador', 'Operador'))
    );

    create table public.categoria
    (
        categoria_id integer generated always as identity not null constraint pk_categoria primary key,
        nombre varchar(80) not null,
        activo boolean not null constraint df_categoria_activo default (true),
        constraint uq_categoria_nombre unique (nombre),
        constraint ck_categoria_nombre check (length(ltrim(rtrim(nombre))) > 0)
    );

    create table public.producto
    (
        producto_id integer generated always as identity not null constraint pk_producto primary key,
        codigo varchar(30) not null,
        nombre varchar(150) not null,
        categoria_id int not null,
        precio_venta decimal(18,2) not null,
        existencia_minima decimal(18,3) not null constraint df_producto_minimo default (0),
        activo boolean not null constraint df_producto_activo default (true),
        fecha_creacion timestamptz(0) not null constraint df_producto_fecha default (current_timestamp),
        constraint fk_producto_categoria foreign key (categoria_id) references public.categoria(categoria_id),
        constraint ck_producto_codigo check (length(ltrim(rtrim(codigo))) > 0
            and octet_length(codigo) = octet_length(ltrim(rtrim(codigo)))),
        constraint ck_producto_nombre check (length(ltrim(rtrim(nombre))) > 0),
        constraint ck_producto_precio check (precio_venta >= 0),
        constraint ck_producto_minimo check (existencia_minima >= 0)
    );

    create table public.cliente
    (
        cliente_id integer generated always as identity not null constraint pk_cliente primary key,
        nombre varchar(150) not null,
        telefono varchar(30) null,
        direccion varchar(250) null,
        activo boolean not null constraint df_cliente_activo default (true),
        constraint ck_cliente_nombre check (length(ltrim(rtrim(nombre))) > 0)
    );

    create table public.proveedor
    (
        proveedor_id integer generated always as identity not null constraint pk_proveedor primary key,
        nombre varchar(150) not null,
        contacto varchar(100) null,
        telefono varchar(30) null,
        direccion varchar(250) null,
        activo boolean not null constraint df_proveedor_activo default (true),
        constraint ck_proveedor_nombre check (length(ltrim(rtrim(nombre))) > 0)
    );

    create table public.compra
    (
        compra_id integer generated always as identity not null constraint pk_compra primary key,
        fecha timestamptz(0) not null constraint df_compra_fecha default (current_timestamp),
        tipo varchar(20) not null constraint df_compra_tipo default ('COMPRA'),
        proveedor_id int null,
        usuario_id int not null,
        referencia varchar(60) null,
        observaciones varchar(500) null,
        estado varchar(15) not null constraint df_compra_estado default ('BORRADOR'),
        fecha_anulacion timestamptz(0) null,
        usuario_anulacion_id int null,
        motivo_anulacion varchar(250) null,
        constraint fk_compra_proveedor foreign key (proveedor_id) references public.proveedor(proveedor_id),
        constraint fk_compra_usuario foreign key (usuario_id) references public.usuario(usuario_id),
        constraint fk_compra_usuarioanulacion foreign key (usuario_anulacion_id) references public.usuario(usuario_id),
        constraint ck_compra_tipo check (tipo in ('COMPRA', 'INVENTARIO_INICIAL')),
        constraint ck_compra_proveedor check ((tipo = 'COMPRA' and proveedor_id is not null)
            or (tipo = 'INVENTARIO_INICIAL' and proveedor_id is null)),
        constraint ck_compra_estado check (estado in ('BORRADOR', 'CONFIRMADA', 'ANULADA')),
        constraint ck_compra_anulacion check (
            (estado = 'ANULADA' and fecha_anulacion is not null and fecha_anulacion >= fecha
             and usuario_anulacion_id is not null and motivo_anulacion is not null
             and length(ltrim(rtrim(motivo_anulacion))) > 0)
            or (estado <> 'ANULADA' and fecha_anulacion is null
                and usuario_anulacion_id is null and motivo_anulacion is null))
    );

    create table public.detalle_compra
    (
        detalle_compra_id integer generated always as identity not null constraint pk_detallecompra primary key,
        compra_id int not null,
        producto_id int not null,
        cantidad decimal(18,3) not null,
        costo_unitario decimal(18,2) not null,
        subtotal numeric(28,2) generated always as (round(cantidad * costo_unitario, 2)) stored,
        constraint fk_detallecompra_compra foreign key (compra_id) references public.compra(compra_id),
        constraint fk_detallecompra_producto foreign key (producto_id) references public.producto(producto_id),
        constraint uq_detallecompra_producto unique (compra_id, producto_id),
        constraint ck_detallecompra_cantidad check (cantidad > 0),
        constraint ck_detallecompra_costo check (costo_unitario >= 0)
    );

    create table public.venta
    (
        venta_id integer generated always as identity not null constraint pk_venta primary key,
        fecha timestamptz(0) not null constraint df_venta_fecha default (current_timestamp),
        cliente_id int null, -- null representa consumidor ocasional sin registro.
        usuario_id int not null,
        observaciones varchar(500) null,
        estado varchar(15) not null constraint df_venta_estado default ('BORRADOR'),
        fecha_anulacion timestamptz(0) null,
        usuario_anulacion_id int null,
        motivo_anulacion varchar(250) null,
        constraint fk_venta_cliente foreign key (cliente_id) references public.cliente(cliente_id),
        constraint fk_venta_usuario foreign key (usuario_id) references public.usuario(usuario_id),
        constraint fk_venta_usuarioanulacion foreign key (usuario_anulacion_id) references public.usuario(usuario_id),
        constraint ck_venta_estado check (estado in ('BORRADOR', 'CONFIRMADA', 'ANULADA')),
        constraint ck_venta_anulacion check (
            (estado = 'ANULADA' and fecha_anulacion is not null and fecha_anulacion >= fecha
             and usuario_anulacion_id is not null and motivo_anulacion is not null
             and length(ltrim(rtrim(motivo_anulacion))) > 0)
            or (estado <> 'ANULADA' and fecha_anulacion is null
                and usuario_anulacion_id is null and motivo_anulacion is null))
    );

    create table public.detalle_venta
    (
        detalle_venta_id integer generated always as identity not null constraint pk_detalleventa primary key,
        venta_id int not null,
        producto_id int not null,
        cantidad decimal(18,3) not null,
        precio_unitario decimal(18,2) not null, -- precio aplicado al momento de vender.
        subtotal numeric(28,2) generated always as (round(cantidad * precio_unitario, 2)) stored,
        constraint fk_detalleventa_venta foreign key (venta_id) references public.venta(venta_id),
        constraint fk_detalleventa_producto foreign key (producto_id) references public.producto(producto_id),
        constraint uq_detalleventa_producto unique (venta_id, producto_id),
        constraint ck_detalleventa_cantidad check (cantidad > 0),
        constraint ck_detalleventa_precio check (precio_unitario >= 0)
    );

    -- no se usan eliminaciones en cascada: se conserva la integridad del historial.
    create index ix_producto_categoria on public.producto(categoria_id);
    create index ix_compra_proveedor on public.compra(proveedor_id);
    create index ix_compra_usuario on public.compra(usuario_id);
    create index ix_compra_anulador on public.compra(usuario_anulacion_id);
    create index ix_compra_fecha on public.compra(fecha, estado);
    create index ix_detallecompra_producto on public.detalle_compra(producto_id) include (compra_id, cantidad);
    create index ix_venta_cliente on public.venta(cliente_id);
    create index ix_venta_usuario on public.venta(usuario_id);
    create index ix_venta_anulador on public.venta(usuario_anulacion_id);
    create index ix_venta_fecha on public.venta(fecha, estado);
    create index ix_detalleventa_producto on public.detalle_venta(producto_id) include (venta_id, cantidad);


CREATE UNIQUE INDEX uq_usuario_nombre_usuario ON public.usuario (lower(nombre_usuario));
CREATE UNIQUE INDEX uq_producto_codigo ON public.producto (lower(codigo));
create view public.vw_inventario as
        with movimientos as
        (
            select d.producto_id, d.cantidad
            from public.detalle_compra d inner join public.compra c on c.compra_id = d.compra_id
            where c.estado = 'CONFIRMADA'
            union all
            select d.producto_id, -d.cantidad
            from public.detalle_venta d inner join public.venta v on v.venta_id = d.venta_id
            where v.estado = 'CONFIRMADA'
        ), saldos as
        (
            select producto_id, sum(cantidad) as existencia
            from movimientos group by producto_id
        )
        select p.producto_id, p.codigo, p.nombre, c.nombre as categoria,
            p.precio_venta, p.existencia_minima, p.activo,
            coalesce(s.existencia, 0::numeric(38,3)) as existencia
        from public.producto p
        inner join public.categoria c on c.categoria_id = p.categoria_id
        left join saldos s on s.producto_id = p.producto_id;

create view public.vw_productos_bajo_minimo as
        select producto_id, codigo, nombre, categoria, existencia, existencia_minima
        from public.vw_inventario where activo = true and existencia <= existencia_minima;

create view public.vw_total_ventas as
        select v.venta_id, v.fecha, v.cliente_id, v.usuario_id, v.estado,
            coalesce(t.total, 0::numeric(38,2)) as total
        from public.venta v
        left join (select venta_id, sum(subtotal) as total
                   from public.detalle_venta group by venta_id) t on t.venta_id = v.venta_id;

create view public.vw_total_compras as
        select c.compra_id, c.fecha, c.tipo, c.proveedor_id, c.usuario_id, c.estado,
            coalesce(t.total, 0::numeric(38,2)) as total
        from public.compra c
        left join (select compra_id, sum(subtotal) as total
                   from public.detalle_compra group by compra_id) t on t.compra_id = c.compra_id;

create view public.vw_ventas_por_dia as
        select (fecha at time zone 'UTC')::date as fecha_utc, count(*) as numero_ventas,
            sum(total) as total_vendido
        from public.vw_total_ventas where estado = 'CONFIRMADA'
        group by (fecha at time zone 'UTC')::date;

create view public.vw_productos_mas_vendidos as
        select p.producto_id, p.codigo, p.nombre,
            sum(d.cantidad) as cantidad_vendida, sum(d.subtotal) as importe_vendido
        from public.detalle_venta d
        inner join public.venta v on v.venta_id = d.venta_id
        inner join public.producto p on p.producto_id = d.producto_id
        where v.estado = 'CONFIRMADA'
        group by p.producto_id, p.codigo, p.nombre;
COMMIT;
SELECT table_name AS tabla FROM information_schema.tables
WHERE table_schema='public' AND table_type='BASE TABLE' ORDER BY table_name;
SELECT conname AS relacion, conrelid::regclass AS tabla_origen,
       confrelid::regclass AS tabla_destino
FROM pg_constraint WHERE contype='f' AND connamespace='public'::regnamespace
ORDER BY conrelid::regclass::text, conname;
