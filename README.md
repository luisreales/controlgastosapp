# Control de Gastos - Aplicación Web

## Descripción del Proyecto
Aplicación web para el control de gastos personales que permite gestionar ingresos y egresos por fondo monetario. Desarrollada en Microsoft .NET con SQL Server y componentes DevExpress.

## Requisitos Técnicos
- Framework: Microsoft .NET
- Base de Datos: SQL Server
- Componentes UI: DevExpress
- Tipo: Aplicación Web 100%

## Credenciales de Acceso
- Usuario: admin
- Contraseña: admin

## Arquitectura del Sistema

### Capas de la Aplicación
1. **Capa de Presentación (UI)**
   - Implementada con DevExpress
   - Interfaz web responsiva
   - Componentes de visualización y entrada de datos

2. **Capa de Negocio (Business Logic)**
   - Lógica de negocio centralizada
   - Validaciones de reglas de negocio
   - Gestión de transacciones

3. **Capa de Acceso a Datos (Data Access)**
   - Conexión con SQL Server
   - Operaciones CRUD
   - Manejo de transacciones

### Módulos del Sistema

#### 1. Mantenimientos
- **Tipos de Gasto**
  - Generación automática de códigos
  - Gestión de categorías de gastos

- **Fondo Monetario**
  - Gestión de cuentas bancarias
  - Gestión de fondos de caja menuda

#### 2. Movimientos
- **Presupuesto por Tipo de Gasto**
  - Asignación mensual de presupuestos
  - Control por usuario y tipo de gasto

- **Registro de Gastos**
  - Transacciones de encabezado y detalle
  - Campos:
    - Fecha
    - Fondo Monetario
    - Observaciones
    - Nombre de Comercio
    - Tipo de Documento (Comprobante, Factura, Otro)
  - Detalle:
    - Tipo de Gasto
    - Monto
  - Validación de presupuesto excedido

- **Depósitos**
  - Registro de ingresos
  - Campos:
    - Fecha
    - Fondo Monetario
    - Monto

#### 3. Consultas y Reportes
- **Consulta de Movimientos**
  - Filtrado por rango de fechas
  - Visualización de depósitos y gastos

- **Gráfico Comparativo**
  - Comparación presupuesto vs. ejecución
  - Visualización por tipo de gasto
  - Gráfico tipo barras

## Características Principales
- Validación automática de presupuestos
- Alertas de sobregiro
- Gestión transaccional de registros
- Interfaz intuitiva y responsiva
- Reportes gráficos comparativos

## Notas de Implementación
- La aplicación debe estar publicada para su evaluación
- El tiempo máximo de entrega es de tres días
- Se requiere acceso remoto para revisión del código
