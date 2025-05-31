using Microsoft.AspNetCore.Mvc;
using ControlGastosApp.Web.Services;
using ControlGastosApp.Web.Models;
using ControlGastosApp.Web.Models.Reportes;
using ControlGastosApp.Web.ViewModels.Reportes;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ControlGastosApp.Web.Controllers
{
    public class ReportesController : Controller
    {
        private readonly JsonDataService _dataService;
        private readonly ILogger<ReportesController> _logger;

        public ReportesController(JsonDataService dataService, ILogger<ReportesController> logger)
        {
            _dataService = dataService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GastosPorTipo()
        {
            var gastos = _dataService.GetGastos();
            var tiposGasto = _dataService.GetTiposGasto();
            
            var reporte = gastos
                .SelectMany(g => g.Detalles)
                .GroupBy(d => d.TipoGastoId)
                .Select(g => new GastoPorTipoViewModel
                {
                    TipoGasto = tiposGasto.First(t => t.Id == g.Key).Nombre,
                    Total = g.Sum(d => d.Monto)
                })
                .OrderByDescending(x => x.Total)
                .ToList();

            return View(reporte);
        }

        public IActionResult GastosPorMes()
        {
            var gastos = _dataService.GetGastos();
            
            var reporte = gastos
                .GroupBy(g => new { g.Fecha.Year, g.Fecha.Month })
                .Select(g => new GastoPorMesViewModel
                {
                    Mes = $"{g.Key.Year}/{g.Key.Month:D2}",
                    Total = g.Sum(x => x.Detalles.Sum(d => d.Monto))
                })
                .OrderBy(x => x.Mes)
                .ToList();

            return View(reporte);
        }

        public IActionResult Presupuestos()
        {
            var presupuestos = _dataService.GetPresupuestos();
            var gastos = _dataService.GetGastos();
            var tiposGasto = _dataService.GetTiposGasto();
            
            var reporte = presupuestos
                .Select(p => new PresupuestoReporteViewModel
                {
                    TipoGasto = tiposGasto.First(t => t.Id == p.TipoGastoId).Nombre,
                    Presupuestado = p.Monto,
                    Gastado = gastos
                        .SelectMany(g => g.Detalles)
                        .Where(d => d.TipoGastoId == p.TipoGastoId)
                        .Sum(d => d.Monto),
                })
                .ToList();

            foreach (var item in reporte)
            {
                item.Diferencia = item.Presupuestado - item.Gastado;
            }

            return View(reporte);
        }

        public IActionResult Fondos()
        {
            var fondos = _dataService.GetFondos();
            var total = fondos.Sum(f => f.Saldo);
            
            var reporte = fondos
                .Select(f => new FondoReporteViewModel
                {
                    Nombre = f.Nombre,
                    Saldo = f.Saldo,
                    Porcentaje = total > 0 ? (f.Saldo / total) * 100 : 0
                })
                .OrderByDescending(x => x.Saldo)
                .ToList();

            return View(reporte);
        }

        public IActionResult Movimientos()
        {
            var viewModel = new MovimientosReporteViewModel
            {
                FechaInicio = DateTime.Today.AddMonths(-1),
                FechaFin = DateTime.Today
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Movimientos([FromForm] MovimientosReporteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo inválido en Movimientos: {Errors}", 
                    string.Join(", ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)));
                return View(model);
            }

            try
            {
                _logger.LogInformation("Buscando movimientos entre {FechaInicio} y {FechaFin}", 
                    model.FechaInicio, model.FechaFin);

                var gastos = _dataService.GetGastos()
                    .Where(g => g.Fecha.Date >= model.FechaInicio.Date && g.Fecha.Date <= model.FechaFin.Date)
                    .Select(g => new MovimientoViewModel
                    {
                        Fecha = g.Fecha,
                        FechaFormateada = g.Fecha.ToString("dd/MM/yyyy"),
                        Tipo = "Gasto",
                        Descripcion = g.Observaciones,
                        Monto = -g.Detalles.Sum(d => d.Monto),
                        FondoNombre = g.Fondo?.Nombre ?? "Desconocido"
                    })
                    .ToList();

                _logger.LogInformation("Encontrados {Count} gastos", gastos.Count);

                var depositos = _dataService.GetDepositos()
                    .Where(d => d.Fecha.Date >= model.FechaInicio.Date && d.Fecha.Date <= model.FechaFin.Date)
                    .Select(d => new MovimientoViewModel
                    {
                        Fecha = d.Fecha,
                        FechaFormateada = d.Fecha.ToString("dd/MM/yyyy"),
                        Tipo = "Depósito",
                        Descripcion = d.Descripcion ?? "Depósito",
                        Monto = d.Monto,
                        FondoNombre = d.FondoMonetario?.Nombre ?? "Desconocido"
                    })
                    .ToList();

                _logger.LogInformation("Encontrados {Count} depósitos", depositos.Count);

                model.Movimientos = gastos.Concat(depositos)
                    .OrderByDescending(m => m.Fecha)
                    .ToList();

                _logger.LogInformation("Total de movimientos: {Count}", model.Movimientos.Count);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar movimientos");
                ModelState.AddModelError("", "Ocurrió un error al buscar los movimientos. Por favor, intente nuevamente.");
                return View(model);
            }
        }

        public IActionResult ComparativoPresupuestos()
        {
            var viewModel = new ComparativoPresupuestosViewModel
            {
                FechaInicio = DateTime.Today.AddMonths(-1),
                FechaFin = DateTime.Today,
                TiposGasto = _dataService.GetTiposGasto()
                    .Select(t => new TipoGastoViewModel { Id = t.Id, Nombre = t.Nombre })
                    .ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ComparativoPresupuestos([FromForm] ComparativoPresupuestosViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.TiposGasto = _dataService.GetTiposGasto()
                    .Select(t => new TipoGastoViewModel { Id = t.Id, Nombre = t.Nombre })
                    .ToList();
                return View(model);
            }

            var presupuestos = _dataService.GetPresupuestos()
                .Where(p =>
                {
                    if (DateTime.TryParse(p.Mes + "-01", out var mesPresupuesto))
                    {
                        return mesPresupuesto >= model.FechaInicio && mesPresupuesto <= model.FechaFin;
                    }
                    return false;
                })
                .GroupBy(p => p.TipoGastoId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(p => p.Monto)
                );

            var gastos = _dataService.GetGastos()
                .Where(g => g.Fecha >= model.FechaInicio && g.Fecha <= model.FechaFin)
                .SelectMany(g => g.Detalles)
                .GroupBy(d => d.TipoGastoId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(d => d.Monto)
                );

            model.Comparativo = _dataService.GetTiposGasto()
                .Select(t => new ComparativoViewModel
                {
                    TipoGastoId = t.Id,
                    TipoGastoNombre = t.Nombre,
                    Presupuestado = presupuestos.GetValueOrDefault(t.Id, 0),
                    Ejecutado = gastos.GetValueOrDefault(t.Id, 0)
                })
                .ToList();

            _logger.LogInformation("Datos del comparativo: {Comparativo}", 
                JsonSerializer.Serialize(model.Comparativo));

            model.TiposGasto = _dataService.GetTiposGasto()
                .Select(t => new TipoGastoViewModel { Id = t.Id, Nombre = t.Nombre })
                .ToList();

            // Add movimientos data
            var gastosMovimientos = _dataService.GetGastos()
                .Where(g => g.Fecha.Date >= model.FechaInicio.Date && g.Fecha.Date <= model.FechaFin.Date)
                .Select(g => new MovimientoViewModel
                {
                    Fecha = g.Fecha,
                    Tipo = "Gasto",
                    Descripcion = g.Observaciones,
                    Monto = -g.Detalles.Sum(d => d.Monto),
                    FondoNombre = g.Fondo?.Nombre ?? "Desconocido"
                })
                .ToList();

            var depositosMovimientos = _dataService.GetDepositos()
                .Where(d => d.Fecha.Date >= model.FechaInicio.Date && d.Fecha.Date <= model.FechaFin.Date)
                .Select(d => new MovimientoViewModel
                {
                    Fecha = d.Fecha,
                    Tipo = "Depósito",
                    Descripcion = d.Descripcion ?? "Depósito",
                    Monto = d.Monto,
                    FondoNombre = d.FondoMonetario?.Nombre ?? "Desconocido"
                })
                .ToList();

            model.Movimientos = gastosMovimientos.Concat(depositosMovimientos)
                .OrderByDescending(m => m.Fecha)
                .ToList();

            return View(model);
        }

        public IActionResult GraficoComparativo()
        {
            return View();
        }

        public IActionResult ExportarMovimientos()
        {
            return View();
        }

        public IActionResult ExportarGrafico()
        {
            return View();
        }
    }
} 