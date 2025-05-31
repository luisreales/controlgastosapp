using Microsoft.AspNetCore.Mvc;
using ControlGastosApp.Web.Services;
using ControlGastosApp.Web.Models;
using ControlGastosApp.Web.ViewModels.Presupuestos;

namespace ControlGastosApp.Web.Controllers
{
    public class PresupuestosController : Controller
    {
        private readonly JsonDataService _dataService;

        public PresupuestosController(JsonDataService dataService)
        {
            _dataService = dataService;
        }

        public IActionResult Index()
        {
            var presupuestos = _dataService.GetPresupuestos();
            var tiposGasto = _dataService.GetTiposGasto();
            var gastos = _dataService.GetGastos();

            var viewModel = presupuestos.Select(p => new PresupuestoListViewModel
            {
                Id = p.Id,
                TipoGastoNombre = tiposGasto.First(t => t.Id == p.TipoGastoId).Nombre,
                Mes = p.Mes,
                Monto = p.Monto,
                MontoGastado = gastos
                    .SelectMany(g => g.Detalles)
                    .Where(d => d.TipoGastoId == p.TipoGastoId)
                    .Sum(d => d.Monto)
            }).ToList();

            return View(viewModel);
        }

        public IActionResult Create()
        {
            var tiposGasto = _dataService.GetTiposGasto();
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
        public IActionResult Create(PresupuestoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var presupuestos = _dataService.GetPresupuestos();
                var nuevoPresupuesto = new Presupuesto
                {
                    Id = presupuestos.Count > 0 ? presupuestos.Max(p => p.Id) + 1 : 1,
                    TipoGastoId = model.TipoGastoId,
                    Mes = model.Mes,
                    Monto = model.Monto
                };
                presupuestos.Add(nuevoPresupuesto);
                _dataService.SavePresupuestos(presupuestos);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            var presupuestos = _dataService.GetPresupuestos();
            var presupuesto = presupuestos.FirstOrDefault(p => p.Id == id);
            if (presupuesto == null)
            {
                return NotFound();
            }

            var tiposGasto = _dataService.GetTiposGasto();
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
        public IActionResult Edit(PresupuestoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var presupuestos = _dataService.GetPresupuestos();
                var presupuesto = presupuestos.FirstOrDefault(p => p.Id == model.Id);
                if (presupuesto == null)
                {
                    return NotFound();
                }

                presupuesto.TipoGastoId = model.TipoGastoId;
                presupuesto.Mes = model.Mes;
                presupuesto.Monto = model.Monto;

                _dataService.SavePresupuestos(presupuestos);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public IActionResult Delete(int id)
        {
            var presupuestos = _dataService.GetPresupuestos();
            var presupuesto = presupuestos.FirstOrDefault(p => p.Id == id);
            if (presupuesto == null)
            {
                return NotFound();
            }

            var tiposGasto = _dataService.GetTiposGasto();
            presupuesto.TipoGasto = tiposGasto.First(t => t.Id == presupuesto.TipoGastoId);

            return View(presupuesto);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var presupuestos = _dataService.GetPresupuestos();
            var presupuesto = presupuestos.FirstOrDefault(p => p.Id == id);
            if (presupuesto == null)
            {
                return NotFound();
            }

            presupuestos.Remove(presupuesto);
            _dataService.SavePresupuestos(presupuestos);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Detalle(int id)
        {
            var presupuestos = _dataService.GetPresupuestos();
            var presupuesto = presupuestos.FirstOrDefault(p => p.Id == id);
            if (presupuesto == null)
            {
                return NotFound();
            }

            var tiposGasto = _dataService.GetTiposGasto();
            var gastos = _dataService.GetGastos();
            var fondos = _dataService.GetFondos();

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