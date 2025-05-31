using Microsoft.AspNetCore.Mvc;
using ControlGastosApp.Web.Services;
using ControlGastosApp.Web.Models;
using ControlGastosApp.Web.Models.Reportes;
using ControlGastosApp.Web.ViewModels.Reportes;
using System.Linq;

namespace ControlGastosApp.Web.Controllers
{
    public class ReportesController : Controller
    {
        private readonly JsonDataService _dataService;

        public ReportesController(JsonDataService dataService)
        {
            _dataService = dataService;
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
                return View(model);
            }

            var gastos = _dataService.GetGastos()
                .Where(g => g.Fecha >= model.FechaInicio && g.Fecha <= model.FechaFin)
                .Select(g => new MovimientoViewModel
                {
                    Fecha = g.Fecha,
                    FechaFormateada = g.Fecha.ToString("dd/MM/yyyy"),
                    Tipo = "Gasto",
                    Descripcion = g.Observaciones,
                    Monto = -g.Detalles.Sum(d => d.Monto),
                    MontoFormateado = g.Detalles.Sum(d => d.Monto).ToString("C"),
                    FondoNombre = g.Fondo?.Nombre ?? "Desconocido"
                });

            var depositos = _dataService.GetDepositos()
                .Where(d => d.Fecha >= model.FechaInicio && d.Fecha <= model.FechaFin)
                .Select(d => new MovimientoViewModel
                {
                    Fecha = d.Fecha,
                    FechaFormateada = d.Fecha.ToString("dd/MM/yyyy"),
                    Tipo = "Depósito",
                    Descripcion = d.Descripcion ?? "Depósito",
                    Monto = d.Monto,
                    MontoFormateado = d.Monto.ToString("C"),
                    FondoNombre = d.FondoMonetario?.Nombre ?? "Desconocido"
                });

            model.Movimientos = gastos.Concat(depositos)
                .OrderByDescending(m => m.Fecha)
                .ToList();

            return View(model);
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

            model.TiposGasto = _dataService.GetTiposGasto()
                .Select(t => new TipoGastoViewModel { Id = t.Id, Nombre = t.Nombre })
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