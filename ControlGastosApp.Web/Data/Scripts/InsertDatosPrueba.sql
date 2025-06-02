-- Primero limpiamos las tablas y reseteamos las identidades
DELETE FROM DetallesGastos;
DELETE FROM RegistrosGastos;
DELETE FROM Depositos;
DELETE FROM Presupuestos;
DELETE FROM TiposGasto;
DELETE FROM FondosMonetarios;

DBCC CHECKIDENT ('FondosMonetarios', RESEED, 0);
DBCC CHECKIDENT ('TiposGasto', RESEED, 0);
DBCC CHECKIDENT ('Presupuestos', RESEED, 0);
DBCC CHECKIDENT ('RegistrosGastos', RESEED, 0);
DBCC CHECKIDENT ('DetallesGastos', RESEED, 0);
DBCC CHECKIDENT ('Depositos', RESEED, 0);

-- Insertar Fondos Monetarios
INSERT INTO FondosMonetarios (Nombre, Saldo) VALUES
('Caja Principal', 5000000),
('Caja Chica', 1000000),
('Fondo de Emergencia', 2000000);

-- Insertar Tipos de Gasto
INSERT INTO TiposGasto (Nombre, Codigo, Descripcion) VALUES
('Alimentación', 'TG1', 'Gastos en supermercados y restaurantes'),
('Transporte', 'TG2', 'Combustible, pasajes y mantenimiento de vehículos'),
('Servicios Básicos', 'TG3', 'Luz, agua, gas, internet y teléfono'),
('Salud', 'TG4', 'Medicamentos y atención médica'),
('Educación', 'TG5', 'Libros, cursos y materiales educativos'),
('Entretenimiento', 'TG6', 'Cine, teatro y actividades recreativas'),
('Ropa y Calzado', 'TG7', 'Vestuario y accesorios'),
('Hogar', 'TG8', 'Muebles, electrodomésticos y mantenimiento'),
('Impuestos', 'TG9', 'Pagos de impuestos y tasas'),
('Otros', 'TG10', 'Gastos varios no categorizados');

-- Insertar Presupuestos para el mes actual
DECLARE @MesActual VARCHAR(7) = FORMAT(GETDATE(), 'yyyy-MM');

INSERT INTO Presupuestos (TipoGastoId, Mes, Monto) VALUES
(1, @MesActual, 500000),  -- Alimentación
(2, @MesActual, 200000),  -- Transporte
(3, @MesActual, 300000),  -- Servicios Básicos
(4, @MesActual, 150000),  -- Salud
(5, @MesActual, 200000),  -- Educación
(6, @MesActual, 100000),  -- Entretenimiento
(7, @MesActual, 150000),  -- Ropa y Calzado
(8, @MesActual, 100000),  -- Hogar
(9, @MesActual, 200000),  -- Impuestos
(10, @MesActual, 100000); -- Otros

-- Insertar Depositos
INSERT INTO Depositos (FondoMonetarioId, Fecha, Monto, Descripcion) VALUES
(1, DATEADD(DAY, -30, GETDATE()), 2000000, 'Depósito inicial'),
(1, DATEADD(DAY, -15, GETDATE()), 1500000, 'Depósito quincenal'),
(2, DATEADD(DAY, -20, GETDATE()), 500000, 'Depósito inicial caja chica'),
(3, DATEADD(DAY, -25, GETDATE()), 1000000, 'Depósito fondo emergencia');

-- Insertar Registros de Gasto con sus Detalles
-- Gasto 1: Supermercado (Alimentación + Hogar)
INSERT INTO RegistrosGastos (Fecha, FondoMonetarioId, Comercio, TipoDocumento, Observaciones)
VALUES (DATEADD(DAY, -5, GETDATE()), 1, 'Supermercado XYZ', 1, 'Compra mensual');

DECLARE @GastoId1 INT = SCOPE_IDENTITY();

INSERT INTO DetallesGastos (RegistroGastoId, TipoGastoId, Monto) VALUES
(@GastoId1, 1, 150000), -- Alimentación
(@GastoId1, 8, 50000);  -- Hogar

-- Gasto 2: Combustible (Transporte)
INSERT INTO RegistrosGastos (Fecha, FondoMonetarioId, Comercio, TipoDocumento, Observaciones)
VALUES (DATEADD(DAY, -4, GETDATE()), 1, 'Estación de Servicio ABC', 1, 'Llenado de tanque');

DECLARE @GastoId2 INT = SCOPE_IDENTITY();

INSERT INTO DetallesGastos (RegistroGastoId, TipoGastoId, Monto) VALUES
(@GastoId2, 2, 75000); -- Transporte

-- Gasto 3: Servicios (Servicios Básicos)
INSERT INTO RegistrosGastos (Fecha, FondoMonetarioId, Comercio, TipoDocumento, Observaciones)
VALUES (DATEADD(DAY, -3, GETDATE()), 1, 'Compañía de Servicios', 1, 'Pago servicios básicos');

DECLARE @GastoId3 INT = SCOPE_IDENTITY();

INSERT INTO DetallesGastos (RegistroGastoId, TipoGastoId, Monto) VALUES
(@GastoId3, 3, 250000); -- Servicios Básicos

-- Gasto 4: Farmacia (Salud)
INSERT INTO RegistrosGastos (Fecha, FondoMonetarioId, Comercio, TipoDocumento, Observaciones)
VALUES (DATEADD(DAY, -2, GETDATE()), 2, 'Farmacia XYZ', 1, 'Medicamentos');

DECLARE @GastoId4 INT = SCOPE_IDENTITY();

INSERT INTO DetallesGastos (RegistroGastoId, TipoGastoId, Monto) VALUES
(@GastoId4, 4, 45000); -- Salud

-- Gasto 5: Entretenimiento
INSERT INTO RegistrosGastos (Fecha, FondoMonetarioId, Comercio, TipoDocumento, Observaciones)
VALUES (DATEADD(DAY, -1, GETDATE()), 2, 'Cine ABC', 1, 'Entradas cine');

DECLARE @GastoId5 INT = SCOPE_IDENTITY();

INSERT INTO DetallesGastos (RegistroGastoId, TipoGastoId, Monto) VALUES
(@GastoId5, 6, 25000); -- Entretenimiento

-- Actualizar saldos de fondos monetarios
UPDATE FondosMonetarios 
SET Saldo = Saldo - (
    SELECT COALESCE(SUM(d.Monto), 0)
    FROM RegistrosGastos r
    JOIN DetallesGastos d ON r.Id = d.RegistroGastoId
    WHERE r.FondoMonetarioId = FondosMonetarios.Id
)
WHERE Id IN (1, 2); 

-- =============================
-- Movimientos de ejemplo para reportes
-- =============================

-- Depósitos recientes
INSERT INTO Depositos (FondoMonetarioId, Fecha, Monto, Descripcion) VALUES
(1, DATEADD(DAY, -2, GETDATE()), 500000, 'Depósito de prueba 1'),
(2, DATEADD(DAY, -10, GETDATE()), 200000, 'Depósito de prueba 2'),
(1, DATEADD(DAY, -20, GETDATE()), 300000, 'Depósito de prueba 3');

-- Gasto 1: Alimentación
INSERT INTO RegistrosGastos (Fecha, FondoMonetarioId, Comercio, TipoDocumento, Observaciones)
VALUES (DATEADD(DAY, -1, GETDATE()), 1, 'Supermercado Prueba', 1, 'Compra semanal');
DECLARE @GastoEj1 INT = SCOPE_IDENTITY();
INSERT INTO DetallesGastos (RegistroGastoId, TipoGastoId, Monto) VALUES
(@GastoEj1, 1, 120000);

-- Gasto 2: Transporte
INSERT INTO RegistrosGastos (Fecha, FondoMonetarioId, Comercio, TipoDocumento, Observaciones)
VALUES (DATEADD(DAY, -5, GETDATE()), 2, 'Gasolinera Prueba', 1, 'Carga de combustible');
DECLARE @GastoEj2 INT = SCOPE_IDENTITY();
INSERT INTO DetallesGastos (RegistroGastoId, TipoGastoId, Monto) VALUES
(@GastoEj2, 2, 50000);

-- Gasto 3: Entretenimiento
INSERT INTO RegistrosGastos (Fecha, FondoMonetarioId, Comercio, TipoDocumento, Observaciones)
VALUES (DATEADD(DAY, -15, GETDATE()), 1, 'Cine Prueba', 1, 'Entradas de cine');
DECLARE @GastoEj3 INT = SCOPE_IDENTITY();
INSERT INTO DetallesGastos (RegistroGastoId, TipoGastoId, Monto) VALUES
(@GastoEj3, 6, 30000);

-- Gasto 4: Otros
INSERT INTO RegistrosGastos (Fecha, FondoMonetarioId, Comercio, TipoDocumento, Observaciones)
VALUES (DATEADD(DAY, -25, GETDATE()), 2, 'Varios Prueba', 1, 'Gasto varios');
DECLARE @GastoEj4 INT = SCOPE_IDENTITY();
INSERT INTO DetallesGastos (RegistroGastoId, TipoGastoId, Monto) VALUES
(@GastoEj4, 10, 20000); 