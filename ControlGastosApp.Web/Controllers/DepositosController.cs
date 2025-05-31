using Microsoft.AspNetCore.Mvc;
using ControlGastosApp.Web.Services;
using ControlGastosApp.Web.Models;
using ControlGastosApp.Web.ViewModels.Depositos;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ControlGastosApp.Web.Controllers
{
    public class DepositosController : Controller
    {
        private readonly JsonDataService _dataService;

        public DepositosController(JsonDataService dataService)
        {
            _dataService = dataService;
        }

        public IActionResult Index()
        {
            var depositos = _dataService.GetDepositos();
            var viewModel = depositos.Select(d => new DepositoListViewModel
            {
                Id = d.Id,
                Fecha = d.Fecha,
                FechaFormateada = d.Fecha.ToString("dd/MM/yyyy"),
                FondoNombre = d.FondoMonetario?.Nombre ?? "Desconocido",
                Monto = d.Monto,
                MontoFormateado = d.Monto.ToString("C")
            }).ToList();

            return View(viewModel);
        }

        public IActionResult Create()
        {
            var fondos = _dataService.GetFondos();
            ViewBag.Fondos = new SelectList(fondos, "Id", "Nombre");
            return View(new DepositoCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DepositoCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var fondos = _dataService.GetFondos();
                ViewBag.Fondos = new SelectList(fondos, "Id", "Nombre");
                return View(model);
            }

            var deposito = new Deposito
            {
                Id = _dataService.GetNextDepositoId(),
                Fecha = model.Fecha,
                FondoId = model.FondoId,
                Monto = model.Monto,
                Descripcion = model.Descripcion
            };

            _dataService.AddDeposito(deposito);
            TempData["SuccessMessage"] = "Depósito creado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var deposito = _dataService.GetDeposito(id);
            if (deposito == null)
            {
                return NotFound();
            }

            var fondos = _dataService.GetFondos();
            ViewBag.Fondos = new SelectList(fondos, "Id", "Nombre", deposito.FondoId);

            var viewModel = new DepositoCreateViewModel
            {
                Id = deposito.Id,
                Fecha = deposito.Fecha,
                FondoId = deposito.FondoId,
                Monto = deposito.Monto,
                Descripcion = deposito.Descripcion
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, DepositoCreateViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var fondos = _dataService.GetFondos();
                ViewBag.Fondos = new SelectList(fondos, "Id", "Nombre", model.FondoId);
                return View(model);
            }

            var deposito = new Deposito
            {
                Id = model.Id,
                Fecha = model.Fecha,
                FondoId = model.FondoId,
                Monto = model.Monto,
                Descripcion = model.Descripcion
            };

            _dataService.UpdateDeposito(deposito);
            TempData["SuccessMessage"] = "Depósito actualizado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var deposito = _dataService.GetDeposito(id);
            if (deposito == null)
            {
                return NotFound();
            }

            return View(deposito);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var deposito = _dataService.GetDeposito(id);
            if (deposito == null)
            {
                return NotFound();
            }

            _dataService.DeleteDeposito(id);
            TempData["SuccessMessage"] = "Depósito eliminado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Detalle(int id)
        {
            var deposito = _dataService.GetDeposito(id);
            if (deposito == null)
            {
                return NotFound();
            }

            return View(deposito);
        }
    }
} 