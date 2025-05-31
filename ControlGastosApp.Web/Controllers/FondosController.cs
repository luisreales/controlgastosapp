using Microsoft.AspNetCore.Mvc;
using ControlGastosApp.Web.Services;
using ControlGastosApp.Web.Models;
using ControlGastosApp.Web.ViewModels.FondosMonetarios;

namespace ControlGastosApp.Web.Controllers
{
    public class FondosController : Controller
    {
        private readonly JsonDataService _dataService;

        public FondosController(JsonDataService dataService)
        {
            _dataService = dataService;
        }

        public IActionResult Index()
        {
            var fondos = _dataService.GetFondos();
            return View(fondos);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(FondoMonetarioViewModel model)
        {
            if (ModelState.IsValid)
            {
                var fondos = _dataService.GetFondos();
                var nuevoFondo = new FondoMonetario
                {
                    Id = fondos.Count > 0 ? fondos.Max(f => f.Id) + 1 : 1,
                    Nombre = model.Nombre,
                    Tipo = model.Tipo,
                    Saldo = model.Saldo
                };
                fondos.Add(nuevoFondo);
                _dataService.SaveFondos(fondos);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            var fondos = _dataService.GetFondos();
            var fondo = fondos.FirstOrDefault(f => f.Id == id);
            if (fondo == null)
            {
                return NotFound();
            }

            var model = new FondoMonetarioViewModel
            {
                Id = fondo.Id,
                Nombre = fondo.Nombre,
                Tipo = fondo.Tipo ?? string.Empty,
                Saldo = fondo.Saldo
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(FondoMonetarioViewModel model)
        {
            if (ModelState.IsValid)
            {
                var fondos = _dataService.GetFondos();
                var fondo = fondos.FirstOrDefault(f => f.Id == model.Id);
                if (fondo == null)
                {
                    return NotFound();
                }

                fondo.Nombre = model.Nombre;
                fondo.Tipo = model.Tipo;
                fondo.Saldo = model.Saldo;

                _dataService.SaveFondos(fondos);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public IActionResult Delete(int id)
        {
            var fondos = _dataService.GetFondos();
            var fondo = fondos.FirstOrDefault(f => f.Id == id);
            if (fondo == null)
            {
                return NotFound();
            }

            return View(fondo);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var fondos = _dataService.GetFondos();
            var fondo = fondos.FirstOrDefault(f => f.Id == id);
            if (fondo == null)
            {
                return NotFound();
            }

            fondos.Remove(fondo);
            _dataService.SaveFondos(fondos);
            return RedirectToAction(nameof(Index));
        }
    }
} 