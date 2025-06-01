-- Script para insertar presupuestos para cada tipo de gasto
-- Primero limpiamos la tabla
DELETE FROM Presupuestos;

-- Insertamos los nuevos registros
INSERT INTO Presupuestos (TipoGastoId, Mes, Monto) VALUES
-- Vivienda (ID: 3)
(3, '2024-03', 800000),  -- Alquiler/Hipoteca
(3, '2024-04', 150000),  -- Servicios
(3, '2024-05', 800000),  -- Alquiler/Hipoteca

-- Entretenimiento (ID: 4)
(4, '2024-03', 200000),  -- Cine, restaurantes
(4, '2024-04', 150000),  -- Hobbies
(4, '2024-05', 180000),  -- Actividades recreativas

-- Salud (ID: 5)
(5, '2024-03', 300000),  -- Consultas médicas
(5, '2024-04', 150000),  -- Medicamentos
(5, '2024-05', 200000),  -- Seguros de salud

-- Educación (ID: 6)
(6, '2024-03', 500000),  -- Matrículas
(6, '2024-04', 100000),  -- Libros
(6, '2024-05', 150000),  -- Cursos

-- Ropa (ID: 7)
(7, '2024-03', 200000),  -- Ropa de temporada
(7, '2024-04', 150000),  -- Calzado
(7, '2024-05', 180000),  -- Accesorios

-- Servicios (ID: 8)
(8, '2024-03', 100000),  -- Teléfono, internet
(8, '2024-04', 50000),   -- Streaming
(8, '2024-05', 100000),  -- Servicios digitales

-- Seguros (ID: 9)
(9, '2024-03', 200000),  -- Seguro de coche
(9, '2024-04', 150000),  -- Seguro de hogar
(9, '2024-05', 100000),  -- Seguro de vida

-- Impuestos (ID: 10)
(10, '2024-03', 300000), -- Impuestos personales
(10, '2024-04', 200000), -- Impuestos de propiedad
(10, '2024-05', 250000), -- Otros impuestos

-- Viajes (ID: 11)
(11, '2024-03', 500000), -- Vacaciones
(11, '2024-04', 300000), -- Viajes de fin de semana
(11, '2024-05', 400000), -- Viajes de negocios

-- Regalos (ID: 12)
(12, '2024-03', 100000), -- Regalos familiares
(12, '2024-04', 80000),  -- Regalos amigos
(12, '2024-05', 120000), -- Regalos especiales

-- Donaciones (ID: 13)
(13, '2024-03', 50000),  -- Contribuciones mensuales
(13, '2024-04', 30000),  -- Donaciones puntuales
(13, '2024-05', 40000),  -- Ayudas benéficas

-- Mascotas (ID: 14)
(14, '2024-03', 100000), -- Comida y cuidados
(14, '2024-04', 50000),  -- Veterinario
(14, '2024-05', 80000),  -- Accesorios

-- Cuidado Personal (ID: 15)
(15, '2024-03', 100000), -- Peluquería
(15, '2024-04', 80000),  -- Gimnasio
(15, '2024-05', 120000), -- Cosméticos

-- Reparaciones (ID: 16)
(16, '2024-03', 300000), -- Reparaciones hogar
(16, '2024-04', 200000), -- Reparaciones vehículo
(16, '2024-05', 250000), -- Mantenimiento

-- Deudas (ID: 17)
(17, '2024-03', 400000), -- Préstamos
(17, '2024-04', 300000), -- Tarjetas de crédito
(17, '2024-05', 350000), -- Otros pagos

-- Inversiones (ID: 18)
(18, '2024-03', 500000), -- Cuentas de inversión
(18, '2024-04', 400000), -- Fondos
(18, '2024-05', 450000), -- Otros instrumentos

-- Varios (ID: 19)
(19, '2024-03', 50000),  -- Gastos menores
(19, '2024-04', 40000),  -- Compras pequeñas
(19, '2024-05', 60000),  -- Otros gastos

-- Emergencias (ID: 20)
(20, '2024-03', 200000), -- Fondo de emergencia
(20, '2024-04', 150000), -- Reserva
(20, '2024-05', 180000); -- Contingencia 