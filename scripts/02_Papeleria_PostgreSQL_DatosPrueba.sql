/* PAPELERIA - DATOS FICTICIOS PARA POSTGRESQL 18
 Ejecutar TODO el archivo en Query Tool conectado a papeleriadb,
 DESPUES de 01_Papeleria_PostgreSQL_Estructura.sql.
 Requiere las nueve tablas vacias. No borra registros ni reinicia secuencias.
 Se detiene si ya existen datos. Toda la carga es atomica.
 Si ocurre un error y pgAdmin deja la transaccion abortada, ejecutar ROLLBACK;
 antes de corregirlo y reintentar. Los huecos en secuencias son normales.

 6501 registros: 20 usuarios, 20 categorias, 200 productos, 300 clientes,
 40 proveedores, 121 compras, 800 detalles de compra, 1000 ventas,
 4000 detalles de venta. Periodo: enero-junio 2026, UTC.
 Nombres/contactos ficticios; los telefonos DEMO no son utilizables.
 Usuarios DESACTIVADOS, hashes ASP.NET Identity V3 PBKDF2-SHA512 con sal
 individual y contrasenas aleatorias descartadas. No permiten login.
 Representan empleados historicos que posteriormente fueron desactivados.
 El administrador real se creara desde la configuracion inicial de la aplicacion.
 La existencia de cuentas DEMO inactivas no debe bloquear esa configuracion.

 Se calculan existencias iniciales suficientes para las ventas del periodo.
 Incluye documentos confirmados, borradores y anulados con motivo y responsable.
 Se comprueba el saldo final y el historico, incluidas las anulaciones.
*/
BEGIN;
SET LOCAL TIME ZONE 'UTC';
SET LOCAL search_path = public, pg_catalog;
DO $validar$
DECLARE t text; cantidad bigint;
BEGIN
 IF current_database() <> 'papeleriadb' THEN
   RAISE EXCEPTION 'Abra Query Tool sobre papeleriadb.';
 END IF;
 FOREACH t IN ARRAY ARRAY['usuario','categoria','producto','cliente','proveedor',
                           'compra','detalle_compra','venta','detalle_venta'] LOOP
   IF to_regclass('public.' || t) IS NULL THEN
     RAISE EXCEPTION 'Falta la tabla %. Ejecute primero el script de estructura.', t;
   END IF;
   EXECUTE format('LOCK TABLE public.%I IN ACCESS EXCLUSIVE MODE', t);
   EXECUTE format('SELECT count(*) FROM public.%I', t) INTO cantidad;
   IF cantidad > 0 THEN RAISE EXCEPTION 'Carga cancelada: % contiene datos.',t; END IF;
 END LOOP;
END;
$validar$;

CREATE TEMP TABLE demo_u(n integer PRIMARY KEY, nombre varchar(100), login varchar(50),
 hash varchar(512), rol varchar(20), id integer) ON COMMIT DROP;
INSERT INTO demo_u(n,nombre,login,hash,rol) VALUES
(1, 'Ana Lopez [DEMO]', 'demo.usuario01', 'AQAAAAIAAYagAAAAEN4AeplRrnzGNIzYCOkpeUcsaSi23S4Ei1+6PgQw6oPiv1NWqktInnQlxuBuGhs7/w==', 'Administrador'),
(2, 'Carlos Garcia [DEMO]', 'demo.usuario02', 'AQAAAAIAAYagAAAAENrtGfbZG7We0kS/HYXxGgNI2vXhDVmIAtDrTqLgyPBvC/wylVF7/KuQcMhrbJs2ow==', 'Administrador'),
(3, 'Maria Martinez [DEMO]', 'demo.usuario03', 'AQAAAAIAAYagAAAAEKlSweAgIIoLgpX9r+DR2hv0hzz5Oa8qY0IhTn6vkBGTU6589r8TzHZDUDMtWJG24Q==', 'Administrador'),
(4, 'Jose Perez [DEMO]', 'demo.usuario04', 'AQAAAAIAAYagAAAAEGVvqZuCwO4vtxHGkNuTWT3tLQTiG3t45sWpsukmxRA4Uf6gwIHrr+lmpxCXSU3M+g==', 'Operador'),
(5, 'Lucia Ramirez [DEMO]', 'demo.usuario05', 'AQAAAAIAAYagAAAAEDMAuEmPWpN3lcMbU7YXbB/Mn3AsV5sPo3gdzK53LH3Qo/rNlz11iqzKUWYNVxxFMA==', 'Operador'),
(6, 'Pedro Torres [DEMO]', 'demo.usuario06', 'AQAAAAIAAYagAAAAEETCjEXnNg/1RQVUt3SMYoQ+KIHxp5iMS74LNBfBlAOB4cENMpbkYG05p/wETR8oSg==', 'Operador'),
(7, 'Sofia Flores [DEMO]', 'demo.usuario07', 'AQAAAAIAAYagAAAAEE+WYJG4z2ijvkSjl4jjgN1/eExIHiO3EoqMIHL2Ghm3+dAA+f/56idGCEZ7yt03pQ==', 'Operador'),
(8, 'Luis Castro [DEMO]', 'demo.usuario08', 'AQAAAAIAAYagAAAAEEcxUFe1uA3SLX7e2azwG7wPUoEjNRixcafdbKnR9qP2EfAvJ33s0R5TUAj/57nwFQ==', 'Operador'),
(9, 'Elena Reyes [DEMO]', 'demo.usuario09', 'AQAAAAIAAYagAAAAELpJNivt9UNRuQvxt0f9b/Jxo8ItAW/IqsuZ1ZgZ0pEUGYZidxyQ2XzXGTsdfDuIcA==', 'Operador'),
(10, 'Diego Morales [DEMO]', 'demo.usuario10', 'AQAAAAIAAYagAAAAEKmWBcPDi4eWnnBaHpSNzIzMa06hAmnKt3Go4Eat+SuKW7VQRQT1AJGhMl/NjCM2bQ==', 'Operador'),
(11, 'Paula Cruz [DEMO]', 'demo.usuario11', 'AQAAAAIAAYagAAAAEFoXpdAD2gIPeHmthtJEwvmLYdfW5HYdAgYtzVQ3QahqZjLBqs9uNRpE8SX0sOW62g==', 'Operador'),
(12, 'Mario Mendoza [DEMO]', 'demo.usuario12', 'AQAAAAIAAYagAAAAED0lyqs3Xh5hRducps1AwFc2uJqoywxAAC41R3nHgSjxiAAiOj1XI+zq5Gp5CfNxAg==', 'Operador'),
(13, 'Laura Romero [DEMO]', 'demo.usuario13', 'AQAAAAIAAYagAAAAEHc+JpoemBwrrHSqq0+NXkYtBItnzribwUESbt1aOBNYJveN6SAhms3/J/vW0RlHAw==', 'Operador'),
(14, 'Daniel Vargas [DEMO]', 'demo.usuario14', 'AQAAAAIAAYagAAAAECseQB4F9MxlWUWdkzTkhRif4oEuohU5h9VP5W1Kg1ouRgmdrQGb2DHg3Nv2XThhIA==', 'Operador'),
(15, 'Rosa Herrera [DEMO]', 'demo.usuario15', 'AQAAAAIAAYagAAAAEPPF2Pa+xGuw061LuQlbb6UiS4HuT4w/k+ya61MxdV4jFJ3Gg4DM3lwuygrQHEbkzg==', 'Operador'),
(16, 'Jorge Aguilar [DEMO]', 'demo.usuario16', 'AQAAAAIAAYagAAAAEIXre/vGdgVMfhoivPPfFQySiyt9kAHqy3Y7/zGTL9YvYXMhSpTWV9tDpZWLw8oKVQ==', 'Operador'),
(17, 'Andrea Ramos [DEMO]', 'demo.usuario17', 'AQAAAAIAAYagAAAAEOmJ59oRLLd7zZUum0TDHEAW0GYp4xlgB2mSCq8OPZaLX6U2IB0nka/aaDjQbtQDIQ==', 'Operador'),
(18, 'Oscar Ortiz [DEMO]', 'demo.usuario18', 'AQAAAAIAAYagAAAAEDkz3AvrxgEVskE8xET4EZDK66iRe2YO+a2mKv6vYWugCteJSnvFLw3f4PswAbTEaw==', 'Operador'),
(19, 'Clara Medina [DEMO]', 'demo.usuario19', 'AQAAAAIAAYagAAAAEC8m/YXCddbCATTQz+U1i/Bh7F+0PMLpZcWuxb0YjWdb/75IKa5jdubN07xX63SpiQ==', 'Operador'),
(20, 'Miguel Santos [DEMO]', 'demo.usuario20', 'AQAAAAIAAYagAAAAEA/SGIPn7hMgvUKgXRpW/4rfmSbbke9JShj0mf5Nzgoc/PuZqlLq452dxsaDRysDSA==', 'Operador');
INSERT INTO public.usuario(nombre_completo,nombre_usuario,password_hash,rol,activo,fecha_creacion)
SELECT nombre,login,hash,rol,false,TIMESTAMPTZ '2025-12-01 00:00:00+00' FROM demo_u ORDER BY n;
UPDATE demo_u m SET id=u.usuario_id FROM public.usuario u WHERE u.nombre_usuario=m.login;

CREATE TEMP TABLE demo_c(n integer PRIMARY KEY,nombre varchar(80),base varchar(100),id integer) ON COMMIT DROP;
INSERT INTO demo_c(n,nombre,base) VALUES
(1, 'Cuadernos', 'Cuaderno rayado'),
(2, 'Lapices', 'Lapiz de grafito'),
(3, 'Boligrafos', 'Boligrafo azul'),
(4, 'Marcadores', 'Marcador permanente'),
(5, 'Borradores', 'Borrador blanco'),
(6, 'Sacapuntas', 'Sacapuntas metalico'),
(7, 'Reglas', 'Regla graduada'),
(8, 'Pegamentos', 'Pegamento escolar'),
(9, 'Tijeras', 'Tijera escolar'),
(10, 'Carpetas', 'Carpeta plastica'),
(11, 'Archivadores', 'Archivador de palanca'),
(12, 'Papel de oficina', 'Paquete de papel'),
(13, 'Cartulinas', 'Paquete de cartulina'),
(14, 'Sobres', 'Paquete de sobres'),
(15, 'Etiquetas', 'Paquete de etiquetas'),
(16, 'Grapadoras', 'Grapadora de escritorio'),
(17, 'Grapas', 'Caja de grapas'),
(18, 'Cintas adhesivas', 'Cinta adhesiva'),
(19, 'Pinturas', 'Pintura escolar'),
(20, 'Pinceles', 'Pincel escolar');
INSERT INTO public.categoria(nombre) SELECT nombre FROM demo_c ORDER BY n;
UPDATE demo_c m SET id=c.categoria_id FROM public.categoria c WHERE c.nombre=m.nombre;

CREATE TEMP TABLE demo_p(n integer PRIMARY KEY,codigo varchar(30),id integer) ON COMMIT DROP;
INSERT INTO demo_p(n,codigo) SELECT n,'DEMO-P'||lpad(n::text,4,'0') FROM generate_series(1,200) AS s(n);
INSERT INTO public.producto(codigo,nombre,categoria_id,precio_venta,existencia_minima,activo,fecha_creacion)
SELECT p.codigo,concat(c.base,' - presentacion ',1+(p.n-1)%10),c.id,
       (5+c.n*2+((p.n-1)%10)*1.50)::numeric(18,2),25,true,TIMESTAMPTZ '2025-12-01 00:00:00+00'
FROM demo_p p JOIN demo_c c ON c.n=1+(p.n-1)/10 ORDER BY p.n;
UPDATE demo_p m SET id=p.producto_id FROM public.producto p WHERE p.codigo=m.codigo;

CREATE TEMP TABLE demo_clientes(n integer PRIMARY KEY,id integer NOT NULL) ON COMMIT DROP;
CREATE TEMP TABLE demo_proveedores(n integer PRIMARY KEY,id integer NOT NULL) ON COMMIT DROP;
DO $catalogos$
DECLARE i integer; nuevo_id integer;
BEGIN
 FOR i IN 1..300 LOOP
   INSERT INTO public.cliente(nombre,telefono,direccion,activo)
   SELECT concat(u.nombre,' / cliente ',i),concat('DEMO-TEL-',i),
          concat('Direccion ficticia: sector ',1+(i-1)%12,', local ',i),i%30<>0
   FROM demo_u u WHERE n=1+(i-1)%20 RETURNING cliente_id INTO nuevo_id;
   INSERT INTO demo_clientes VALUES(i,nuevo_id);
 END LOOP;
 FOR i IN 1..40 LOOP
   INSERT INTO public.proveedor(nombre,contacto,telefono,direccion)
   SELECT concat('Distribuidora Escolar DEMO ',i),nombre,concat('DEMO-PROV-',i),
          concat('Zona comercial ficticia, bodega ',i)
   FROM demo_u WHERE n=1+(i-1)%20 RETURNING proveedor_id INTO nuevo_id;
   INSERT INTO demo_proveedores VALUES(i,nuevo_id);
 END LOOP;
END;
$catalogos$;

CREATE TEMP TABLE demo_plan_venta(n integer PRIMARY KEY,fecha timestamptz(0),estado varchar(15),id integer) ON COMMIT DROP;
INSERT INTO demo_plan_venta(n,fecha,estado)
SELECT n,TIMESTAMPTZ '2026-01-01 00:00:00+00'+((n-1)%181)*INTERVAL '1 day'+(840+n%480)*INTERVAL '1 minute',
       CASE WHEN n%20=0 THEN 'ANULADA' WHEN n%25=0 THEN 'BORRADOR' ELSE 'CONFIRMADA' END
FROM generate_series(1,1000) AS s(n);
CREATE TEMP TABLE demo_plan_detalle(n integer,producto_n integer,cantidad integer,
 PRIMARY KEY(n,producto_n)) ON COMMIT DROP;
INSERT INTO demo_plan_detalle
SELECT v.n,1+((v.n-1)*7+j.n*37)%200,1+(v.n+j.n)%3
FROM demo_plan_venta v CROSS JOIN generate_series(0,3) AS j(n);

DO $operaciones$
DECLARE i integer; nuevo_id integer; u integer; admin_id integer; prov integer; cli integer;
        fecha_doc timestamptz(0); estado_doc varchar(15); inicial integer;
BEGIN
 SELECT id INTO admin_id FROM demo_u WHERE n=1;
 INSERT INTO public.compra(fecha,tipo,proveedor_id,usuario_id,referencia,observaciones,estado)
 VALUES(TIMESTAMPTZ '2026-01-01 00:00:00+00','INVENTARIO_INICIAL',NULL,admin_id,
        'DEMO-INICIAL','Carga inicial ficticia para clase','CONFIRMADA') RETURNING compra_id INTO inicial;
 INSERT INTO public.detalle_compra(compra_id,producto_id,cantidad,costo_unitario)
 SELECT inicial,p.id,coalesce(d.demanda,0)+CASE WHEN p.n%10=0 THEN 5 ELSE 100 END,
        round(pr.precio_venta*0.60,2)
 FROM demo_p p JOIN public.producto pr ON pr.producto_id=p.id
 LEFT JOIN (SELECT d.producto_n,sum(d.cantidad) demanda FROM demo_plan_detalle d
            JOIN demo_plan_venta v ON v.n=d.n WHERE v.estado='CONFIRMADA' GROUP BY d.producto_n) d
 ON d.producto_n=p.n;

 FOR i IN 1..120 LOOP
   SELECT id INTO prov FROM demo_proveedores WHERE n=1+(i-1)%40;
   SELECT id INTO u FROM demo_u WHERE n=1+(i-1)%3;
   fecha_doc:=TIMESTAMPTZ '2026-01-01 00:00:00+00'+(1+(i-1)%179)*INTERVAL '1 day'+INTERVAL '12 hours';
   estado_doc:=CASE WHEN i%15=0 THEN 'ANULADA' WHEN i%20=0 THEN 'BORRADOR' ELSE 'CONFIRMADA' END;
   INSERT INTO public.compra(fecha,tipo,proveedor_id,usuario_id,referencia,observaciones,estado,
                            fecha_anulacion,usuario_anulacion_id,motivo_anulacion)
   VALUES(fecha_doc,'COMPRA',prov,u,concat('DEMO-C-',i),'Compra ficticia de reposicion',estado_doc,
          CASE WHEN estado_doc='ANULADA' THEN fecha_doc+INTERVAL '1 hour' END,
          CASE WHEN estado_doc='ANULADA' THEN admin_id END,
          CASE WHEN estado_doc='ANULADA' THEN 'Documento de prueba duplicado' END)
   RETURNING compra_id INTO nuevo_id;
   INSERT INTO public.detalle_compra(compra_id,producto_id,cantidad,costo_unitario)
   SELECT nuevo_id,p.id,2,round(pr.precio_venta*(0.55+(i%6)*0.01),2)
   FROM generate_series(0,4) AS j(n) JOIN demo_p p ON p.n=1+((i-1)*5+j.n)%200
   JOIN public.producto pr ON pr.producto_id=p.id;
 END LOOP;

 FOR i IN 1..1000 LOOP
   SELECT id INTO u FROM demo_u WHERE n=4+(i-1)%17;
   cli:=NULL;
   IF i%5<>0 THEN SELECT id INTO cli FROM demo_clientes WHERE n=1+(i-1)%300; END IF;
   SELECT fecha,estado INTO fecha_doc,estado_doc FROM demo_plan_venta WHERE n=i;
   INSERT INTO public.venta(fecha,cliente_id,usuario_id,observaciones,estado,
                           fecha_anulacion,usuario_anulacion_id,motivo_anulacion)
   VALUES(fecha_doc,cli,u,concat('DEMO-V-',i),estado_doc,
          CASE WHEN estado_doc='ANULADA' THEN fecha_doc+INTERVAL '30 minutes' END,
          CASE WHEN estado_doc='ANULADA' THEN admin_id END,
          CASE WHEN estado_doc='ANULADA' THEN 'Cliente cancelo la compra de prueba' END)
   RETURNING venta_id INTO nuevo_id;
   UPDATE demo_plan_venta SET id=nuevo_id WHERE n=i;
   INSERT INTO public.detalle_venta(venta_id,producto_id,cantidad,precio_unitario)
   SELECT nuevo_id,p.id,d.cantidad,round(pr.precio_venta*(1-(i%4)*0.02),2)
   FROM demo_plan_detalle d JOIN demo_p p ON p.n=d.producto_n
   JOIN public.producto pr ON pr.producto_id=p.id WHERE d.n=i;
 END LOOP;
END;
$operaciones$;

DO $comprobar$
DECLARE negativos bigint;
BEGIN
 IF (SELECT count(*) FROM public.usuario)<>20 OR (SELECT count(*) FROM public.categoria)<>20
 OR (SELECT count(*) FROM public.producto)<>200 OR (SELECT count(*) FROM public.cliente)<>300
 OR (SELECT count(*) FROM public.proveedor)<>40 OR (SELECT count(*) FROM public.compra)<>121
 OR (SELECT count(*) FROM public.detalle_compra)<>800 OR (SELECT count(*) FROM public.venta)<>1000
 OR (SELECT count(*) FROM public.detalle_venta)<>4000 THEN
   RAISE EXCEPTION 'No coinciden las cantidades esperadas. Carga revertida.';
 END IF;
 IF EXISTS (SELECT 1 FROM public.vw_inventario WHERE existencia<0) THEN
   RAISE EXCEPTION 'Se detecto inventario negativo. Carga revertida.';
 END IF;
 IF (SELECT count(*) FROM public.vw_productos_bajo_minimo)<>20 THEN
   RAISE EXCEPTION 'Se esperaban 20 productos para practicar reposicion.';
 END IF;
 WITH m AS (
   SELECT d.producto_id,c.fecha,d.cantidad FROM public.compra c
   JOIN public.detalle_compra d ON d.compra_id=c.compra_id WHERE c.estado<>'BORRADOR'
   UNION ALL
   SELECT d.producto_id,c.fecha_anulacion,-d.cantidad FROM public.compra c
   JOIN public.detalle_compra d ON d.compra_id=c.compra_id WHERE c.estado='ANULADA'
   UNION ALL
   SELECT d.producto_id,v.fecha,-d.cantidad FROM public.venta v
   JOIN public.detalle_venta d ON d.venta_id=v.venta_id WHERE v.estado<>'BORRADOR'
   UNION ALL
   SELECT d.producto_id,v.fecha_anulacion,d.cantidad FROM public.venta v
   JOIN public.detalle_venta d ON d.venta_id=v.venta_id WHERE v.estado='ANULADA'
 ), g AS (SELECT producto_id,fecha,sum(cantidad) cantidad FROM m GROUP BY producto_id,fecha),
 s AS (SELECT sum(cantidad) OVER(PARTITION BY producto_id ORDER BY fecha ROWS UNBOUNDED PRECEDING) saldo FROM g)
 SELECT count(*) INTO negativos FROM s WHERE saldo<0;
 IF negativos>0 THEN RAISE EXCEPTION 'Hay un saldo historico negativo. Carga revertida.'; END IF;
 RAISE NOTICE 'Verificaciones correctas: 6501 registros, sin saldos negativos.';
END;
$comprobar$;
COMMIT;

-- Resultados para revisar en Data Output.
SELECT 'usuario' AS tabla,count(*) AS registros FROM public.usuario
UNION ALL SELECT 'categoria',count(*) FROM public.categoria
UNION ALL SELECT 'producto',count(*) FROM public.producto
UNION ALL SELECT 'cliente',count(*) FROM public.cliente
UNION ALL SELECT 'proveedor',count(*) FROM public.proveedor
UNION ALL SELECT 'compra',count(*) FROM public.compra
UNION ALL SELECT 'detalle_compra',count(*) FROM public.detalle_compra
UNION ALL SELECT 'venta',count(*) FROM public.venta
UNION ALL SELECT 'detalle_venta',count(*) FROM public.detalle_venta;
-- Para ver cada reporte, seleccionar su consulta y ejecutarla por separado:
-- SELECT estado,count(*) AS documentos FROM public.venta GROUP BY estado;
-- SELECT * FROM public.vw_inventario ORDER BY codigo;
-- SELECT * FROM public.vw_productos_bajo_minimo ORDER BY codigo;
-- SELECT * FROM public.vw_productos_mas_vendidos ORDER BY cantidad_vendida DESC,producto_id LIMIT 10;
-- SELECT * FROM public.vw_ventas_por_dia ORDER BY fecha_utc;
