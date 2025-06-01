using Microsoft.AspNetCore.Mvc;
using ControlGastosApp.Web.Services;
using ControlGastosApp.Web.Models;
using ControlGastosApp.Web.ViewModels.Presupuestos;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ControlGastosApp.Web.Controllers
{
    public class PresupuestosController : Controller
    {
        private readonly SqlDataService _sqlDataService;

        public PresupuestosController(SqlDataService sqlDataService)
        {
            _sqlDataService = sqlDataService;
        }

        public async Task<IActionResult> Index()
        {
            var presupuestos = await _sqlDataService.GetPresupuestosAsync();
            var tiposGasto = await _sqlDataService.GetTiposGastoAsync();
            var gastos = await _sqlDataService.GetGastosAsync();

            var viewModel = presupuestos.Select(p => new PresupuestoListViewModel
            {
                Id = p.Id,
                TipoGastoNombre = p.TipoGasto?.Nombre ?? "Desconocido",
                Mes = p.Mes,
                Monto = p.Monto,
                MontoGastado = gastos
                    .Where(g => g.Fecha.ToString("yyyy-MM") == p.Mes)
                    .SelectMany(g => g.Detalles)
                    .Where(d => d.TipoGastoId == p.TipoGastoId)
                    .Sum(d => d.Monto)
            }).ToList();

            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            var tiposGasto = await _sqlDataService.GetTiposGastoAsync();
            var model = new PresupuestoCreateViewModel
            {
                Mes = DateTime.Now.ToString("yyyy-MM"),
                TiposGasto = tiposGasto.Select(t => new TipoGastoSelectViewModel
                {
                    Id = t.Id,
                    Nombre = t.Nombre
                }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PresupuestoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var nuevoPresupuesto = new Presupuesto
                {
                    TipoGastoId = model.TipoGastoId,
                    Mes = model.Mes,
                    Monto = model.Monto
                };
                await _sqlDataService.AddPresupuestoAsync(nuevoPresupuesto);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var presupuesto = await _sqlDataService.GetPresupuestoByIdAsync(id);
            if (presupuesto == null)
            {
                return NotFound();
            }

            var tiposGasto = await _sqlDataService.GetTiposGastoAsync();
            var model = new PresupuestoViewModel
            {
                Id = presupuesto.Id,
                TipoGastoId = presupuesto.TipoGastoId,
                Mes = presupuesto.Mes,
                Monto = presupuesto.Monto
            };

            ViewBag.TiposGasto = tiposGasto.Select(t => new TipoGastoSelectViewModel
            {
                Id = t.Id,
                Nombre = t.Nombre
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PresupuestoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var presupuesto = await _sqlDataService.GetPresupuestoByIdAsync(model.Id);
                if (presupuesto == null)
                {
                    return NotFound();
                }

                presupuesto.TipoGastoId = model.TipoGastoId;
                presupuesto.Mes = model.Mes;
                presupuesto.Monto = model.Monto;

                await _sqlDataService.UpdatePresupuestoAsync(presupuesto);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var presupuesto = await _sqlDataService.GetPresupuestoByIdAsync(id);
            if (presupuesto == null)
            {
                return NotFound();
            }

            var tiposGasto = await _sqlDataService.GetTiposGastoAsync();
            presupuesto.TipoGasto = tiposGasto.First(t => t.Id == presupuesto.TipoGastoId);

            return View(presupuesto);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _sqlDataService.DeletePresupuestoAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var presupuesto = await _sqlDataService.GetPresupuestoByIdAsync(id);
            if (presupuesto == null)
            {
                return NotFound();
            }

            var tiposGasto = await _sqlDataService.GetTiposGastoAsync();
            var gastos = await _sqlDataService.GetGastosAsync();
            var fondos = await _sqlDataService.GetFondosAsync();

            var montoGastado = gastos
                .SelectMany(g => g.Detalles)
                .Where(d => d.TipoGastoId == presupuesto.TipoGastoId)
                .Sum(d => d.Monto);

            var gastosDetalle = gastos
                .SelectMany(g => g.Detalles.Select(d => new { Gasto = g, Detalle = d }))
                .Where(x => x.Detalle.TipoGastoId == presupuesto.TipoGastoId)
                .Select(x => new GastoDetalleViewModel
                {
                    Fecha = x.Gasto.Fecha,
                    Monto = x.Detalle.Monto,
                    FondoNombre = fondos.First(f => f.Id == x.Gasto.FondoId).Nombre
                })
                .ToList();

            var model = new PresupuestoDetalleViewModel
            {
                Id = presupuesto.Id,
                TipoGasto = tiposGasto.First(t => t.Id == presupuesto.TipoGastoId),
                Mes = presupuesto.Mes,
                MontoPresupuestado = presupuesto.Monto,
                MontoGastado = montoGastado,
                Gastos = gastosDetalle
            };

            return View(model);
        }
    }
} 