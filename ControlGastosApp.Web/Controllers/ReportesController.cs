using Microsoft.AspNetCore.Mvc;
using ControlGastosApp.Web.Services;
using ControlGastosApp.Web.Models;
using ControlGastosApp.Web.Models.Reportes;
using ControlGastosApp.Web.ViewModels.Reportes;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ControlGastosApp.Web.Controllers
{
    public class ReportesController : Controller
    {
        private readonly ILogger<ReportesController> _logger;
        private readonly SqlDataService _sqlDataService;

        public ReportesController(ILogger<ReportesController> logger, SqlDataService sqlDataService)
        {
            _logger = logger;
            _sqlDataService = sqlDataService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GastosPorTipo()
        {
            var gastos = await _sqlDataService.GetGastosAsync();
            var tiposGasto = await _sqlDataService.GetTiposGastoAsync();
            
            var reporte = gastos
                .SelectMany(g => g.Detalles)
                .GroupBy(d => d.TipoGastoId)
                .Select(g => new GastoPorTipoViewModel
                {
                    TipoGasto = tiposGasto.First(t => t.Id == g.Key).Nombre!,
                    Total = g.Sum(d => d.Monto)
                })
                .OrderByDescending(x => x.Total)
                .ToList();

            return View(reporte);
        }

        public async Task<IActionResult> GastosPorMes()
        {
            var gastos = await _sqlDataService.GetGastosAsync();
            
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

        public async Task<IActionResult> Presupuestos()
        {
            var presupuestos = await _sqlDataService.GetPresupuestosAsync();
            var gastos = await _sqlDataService.GetGastosAsync();
            var tiposGasto = await _sqlDataService.GetTiposGastoAsync();
            
            var reporte = presupuestos
                .Select(p => new PresupuestoReporteViewModel
                {
                    TipoGasto = tiposGasto.First(t => t.Id == p.TipoGastoId).Nombre!,
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

        public async Task<IActionResult> Fondos()
        {
            var fondos = await _sqlDataService.GetFondosAsync();
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
                FechaFin = DateTime.Today,
                Movimientos = new List<MovimientosViewModel>()
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Movimientos([FromForm] MovimientosReporteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var depositos = await _sqlDataService.GetDepositosAsync();
            var gastos = await _sqlDataService.GetGastosAsync();

            var movimientos = new List<MovimientosViewModel>();

            // Agregar depósitos filtrados
            foreach (var deposito in depositos.Where(d => d.Fecha.Date >= model.FechaInicio.Date && d.Fecha.Date <= model.FechaFin.Date))
            {
                movimientos.Add(new MovimientosViewModel
                {
                    Fecha = deposito.Fecha,
                    Descripcion = $"Depósito - {deposito.Descripcion}",
                    Entradas = deposito.Monto,
                    Salidas = null,
                    TipoMovimiento = "Depósito"
                });
            }

            // Agregar gastos filtrados
            foreach (var gasto in gastos.Where(g => g.Fecha.Date >= model.FechaInicio.Date && g.Fecha.Date <= model.FechaFin.Date))
            {
                movimientos.Add(new MovimientosViewModel
                {
                    Fecha = gasto.Fecha,
                    Descripcion = $"Gasto - {gasto.Observaciones}",
                    Entradas = null,
                    Salidas = gasto.Detalles.Sum(d => d.Monto),
                    TipoMovimiento = "Gasto"
                });
            }

            // Ordenar por fecha
            movimientos = movimientos.OrderBy(m => m.Fecha).ToList();

            // Calcular saldos acumulados
            decimal saldoAcumulado = 0;
            foreach (var movimiento in movimientos)
            {
                saldoAcumulado += (movimiento.Entradas ?? 0) - (movimiento.Salidas ?? 0);
                movimiento.Saldo = saldoAcumulado;
            }

            model.Movimientos = movimientos;
            return View(model);
        }

        public async Task<IActionResult> ComparativoPresupuestos()
        {
            var viewModel = new ComparativoPresupuestosViewModel
            {
                FechaInicio = DateTime.Today.AddMonths(-1),
                FechaFin = DateTime.Today,
                TiposGasto = (await _sqlDataService.GetTiposGastoAsync())
                    .Select(t => new TipoGastoViewModel { Id = t.Id, Nombre = t.Nombre })
                    .ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ComparativoPresupuestos([FromForm] ComparativoPresupuestosViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.TiposGasto = (await _sqlDataService.GetTiposGastoAsync())
                    .Select(t => new TipoGastoViewModel { Id = t.Id, Nombre = t.Nombre! })
                    .ToList();
                return View(model);
            }

            var presupuestosData = await _sqlDataService.GetPresupuestosAsync();
            var presupuestos = presupuestosData
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

            var gastosData = await _sqlDataService.GetGastosAsync();
            var gastos = gastosData
                .Where(g => g.Fecha >= model.FechaInicio && g.Fecha <= model.FechaFin)
                .SelectMany(g => g.Detalles)
                .GroupBy(d => d.TipoGastoId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(d => d.Monto)
                );

            var tiposGastoComparativo = await _sqlDataService.GetTiposGastoAsync();
            model.Comparativo = tiposGastoComparativo
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

            model.TiposGasto = (await _sqlDataService.GetTiposGastoAsync())
                .Select(t => new TipoGastoViewModel { Id = t.Id, Nombre = t.Nombre! })
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