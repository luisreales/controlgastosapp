using ControlGastosApp.Web.Models;
using ControlGastosApp.Web.ViewModels.Gastos;

namespace ControlGastosApp.Web.Services
{
    public class GastosService
    {
        private readonly JsonDataService _dataService;

        public GastosService(JsonDataService dataService)
        {
            _dataService = dataService;
        }

        public (bool Success, string Message, List<PresupuestoExcedido>? PresupuestosExcedidos) ValidarYGuardarGasto(RegistroGasto gasto)
        {
            if (!gasto.TieneDetalles)
            {
                return (false, "El gasto debe tener al menos un detalle.", null);
            }

            // Validar presupuestos
            var presupuestosExcedidos = new List<PresupuestoExcedido>();
            var presupuestos = _dataService.GetPresupuestos();
            var mesActual = gasto.Fecha.ToString("MMMM yyyy");

            foreach (var detalle in gasto.Detalles)
            {
                var presupuesto = presupuestos.FirstOrDefault(p => 
                    p.TipoGastoId == detalle.TipoGastoId && 
                    p.Mes == mesActual);

                if (presupuesto != null)
                {
                    // Calcular el total gastado hasta ahora (incluyendo el nuevo gasto)
                    var gastosExistentes = _dataService.GetGastos()
                        .Where(g => g.Fecha.ToString("MMMM yyyy") == mesActual)
                        .SelectMany(g => g.Detalles)
                        .Where(d => d.TipoGastoId == detalle.TipoGastoId)
                        .Sum(d => d.Monto);

                    var totalGastado = gastosExistentes + detalle.Monto;

                    if (totalGastado > presupuesto.Monto)
                    {
                        presupuestosExcedidos.Add(new PresupuestoExcedido
                        {
                            TipoGastoId = detalle.TipoGastoId,
                            TipoGastoNombre = presupuesto.TipoGasto?.Nombre ?? "Desconocido",
                            MontoPresupuestado = presupuesto.Monto,
                            MontoGastado = totalGastado,
                            Excedente = totalGastado - presupuesto.Monto
                        });
                    }
                }
            }

            if (presupuestosExcedidos.Any())
            {
                return (false, "Se han excedido los presupuestos en algunos tipos de gasto.", presupuestosExcedidos);
            }

            // Si todo está bien, guardar el gasto
            _dataService.AddGasto(gasto);
            return (true, "Gasto guardado correctamente.", null);
        }

        public (bool Success, string Message, List<PresupuestoExcedido>? PresupuestosExcedidos) ValidarYActualizarGasto(RegistroGasto gasto)
        {
            if (!gasto.TieneDetalles)
            {
                return (false, "El gasto debe tener al menos un detalle.", null);
            }

            // Validar presupuestos
            var presupuestosExcedidos = new List<PresupuestoExcedido>();
            var presupuestos = _dataService.GetPresupuestos();
            var mesActual = gasto.Fecha.ToString("MMMM yyyy");

            foreach (var detalle in gasto.Detalles)
            {
                var presupuesto = presupuestos.FirstOrDefault(p => 
                    p.TipoGastoId == detalle.TipoGastoId && 
                    p.Mes == mesActual);

                if (presupuesto != null)
                {
                    // Calcular el total gastado hasta ahora (excluyendo el gasto actual)
                    var gastosExistentes = _dataService.GetGastos()
                        .Where(g => g.Fecha.ToString("MMMM yyyy") == mesActual && g.Id != gasto.Id)
                        .SelectMany(g => g.Detalles)
                        .Where(d => d.TipoGastoId == detalle.TipoGastoId)
                        .Sum(d => d.Monto);

                    var totalGastado = gastosExistentes + detalle.Monto;

                    if (totalGastado > presupuesto.Monto)
                    {
                        presupuestosExcedidos.Add(new PresupuestoExcedido
                        {
                            TipoGastoId = detalle.TipoGastoId,
                            TipoGastoNombre = presupuesto.TipoGasto?.Nombre ?? "Desconocido",
                            MontoPresupuestado = presupuesto.Monto,
                            MontoGastado = totalGastado,
                            Excedente = totalGastado - presupuesto.Monto
                        });
                    }
                }
            }

            if (presupuestosExcedidos.Any())
            {
                return (false, "Se han excedido los presupuestos en algunos tipos de gasto.", presupuestosExcedidos);
            }

            // Si todo está bien, actualizar el gasto
            _dataService.UpdateGasto(gasto);
            return (true, "Gasto actualizado correctamente.", null);
        }
    }

    public class PresupuestoExcedido
    {
        public int TipoGastoId { get; set; }
        public string TipoGastoNombre { get; set; } = string.Empty;
        public decimal MontoPresupuestado { get; set; }
        public decimal MontoGastado { get; set; }
        public decimal Excedente { get; set; }
    }
} 