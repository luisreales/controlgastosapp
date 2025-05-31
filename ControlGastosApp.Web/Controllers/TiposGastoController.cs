using Microsoft.AspNetCore.Mvc;
using ControlGastosApp.Web.Services;
using ControlGastosApp.Web.Models;
using ControlGastosApp.Web.ViewModels.TiposGasto;

namespace ControlGastosApp.Web.Controllers
{
    public class TiposGastoController : Controller
    {
        private readonly JsonDataService _dataService;

        public TiposGastoController(JsonDataService dataService)
        {
            _dataService = dataService;
        }

        public IActionResult Index()
        {
            var tiposGasto = _dataService.GetTiposGasto();
            return View(tiposGasto);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(TipoGastoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var tiposGasto = _dataService.GetTiposGasto();
                var nuevoTipoGasto = new TipoGasto
                {
                    Id = tiposGasto.Count > 0 ? tiposGasto.Max(t => t.Id) + 1 : 1,
                    Nombre = model.Nombre,
                    Codigo = model.Codigo
                };
                tiposGasto.Add(nuevoTipoGasto);
                _dataService.SaveTiposGasto(tiposGasto);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            var tiposGasto = _dataService.GetTiposGasto();
            var tipoGasto = tiposGasto.FirstOrDefault(t => t.Id == id);
            if (tipoGasto == null)
            {
                return NotFound();
            }

            var model = new TipoGastoViewModel
            {
                Id = tipoGasto.Id,
                Nombre = tipoGasto.Nombre,
                Codigo = tipoGasto.Codigo
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(TipoGastoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var tiposGasto = _dataService.GetTiposGasto();
                var tipoGasto = tiposGasto.FirstOrDefault(t => t.Id == model.Id);
                if (tipoGasto == null)
                {
                    return NotFound();
                }

                tipoGasto.Nombre = model.Nombre;
                tipoGasto.Codigo = model.Codigo;

                _dataService.SaveTiposGasto(tiposGasto);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public IActionResult Delete(int id)
        {
            var tiposGasto = _dataService.GetTiposGasto();
            var tipoGasto = tiposGasto.FirstOrDefault(t => t.Id == id);
            if (tipoGasto == null)
            {
                return NotFound();
            }

            return View(tipoGasto);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var tiposGasto = _dataService.GetTiposGasto();
            var tipoGasto = tiposGasto.FirstOrDefault(t => t.Id == id);
            if (tipoGasto == null)
            {
                return NotFound();
            }

            tiposGasto.Remove(tipoGasto);
            _dataService.SaveTiposGasto(tiposGasto);
            return RedirectToAction(nameof(Index));
        }
    }
} 