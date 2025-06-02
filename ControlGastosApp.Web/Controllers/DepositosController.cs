using Microsoft.AspNetCore.Mvc;
using ControlGastosApp.Web.Services;
using ControlGastosApp.Web.Models;
using ControlGastosApp.Web.ViewModels.Depositos;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace ControlGastosApp.Web.Controllers
{
    public class DepositosController : Controller
    {
        private readonly SqlDataService _sqlDataService;

        public DepositosController(SqlDataService sqlDataService)
        {
            _sqlDataService = sqlDataService;
        }

        public async Task<IActionResult> Index()
        {
            var depositos = await _sqlDataService.GetDepositosAsync();
            var viewModel = depositos.Select(d => new DepositoListViewModel
            {
                Id = d.Id,
                Fecha = d.Fecha,
                FechaFormateada = d.Fecha.ToString("dd/MM/yyyy"),
                FondoNombre = d.FondoMonetario?.Nombre ?? "Desconocido",
                Monto = d.Monto,
                MontoFormateado = d.Monto.ToString("C"),                
            }).ToList();

            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            var fondos = await _sqlDataService.GetFondosAsync();
            ViewBag.Fondos = new SelectList(fondos, "Id", "Nombre");

            var viewModel = new DepositoCreateViewModel
            {
                Fecha = DateTime.Now.Date
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepositoCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var fondos = await _sqlDataService.GetFondosAsync();
                ViewBag.Fondos = new SelectList(fondos, "Id", "Nombre");
                return View(model);
            }

            var deposito = new Deposito
            {
                Fecha = model.Fecha,
                FondoMonetarioId = model.FondoMonetarioId,
                Monto = model.Monto ?? throw new InvalidOperationException("El monto es requerido"),
                Descripcion = model.Descripcion
            };

            await _sqlDataService.AddDepositoAsync(deposito);
            TempData["SuccessMessage"] = "Depósito creado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var deposito = await _sqlDataService.GetDepositoByIdAsync(id);
            if (deposito == null)
            {
                return NotFound();
            }

            var fondos = await _sqlDataService.GetFondosAsync();
            ViewBag.Fondos = new SelectList(fondos, "Id", "Nombre", deposito.FondoMonetarioId);

            var viewModel = new DepositoCreateViewModel
            {
                Id = deposito.Id,
                Fecha = deposito.Fecha,
                FondoMonetarioId = deposito.FondoMonetarioId,
                Monto = deposito.Monto,
                Descripcion = deposito.Descripcion
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DepositoCreateViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var fondos = await _sqlDataService.GetFondosAsync();
                ViewBag.Fondos = new SelectList(fondos, "Id", "Nombre", model.FondoMonetarioId);
                return View(model);
            }

            var deposito = new Deposito
            {
                Id = model.Id,
                Fecha = model.Fecha,
                FondoMonetarioId = model.FondoMonetarioId,
                Monto = model.Monto ?? throw new InvalidOperationException("El monto es requerido"),
                Descripcion = model.Descripcion
            };

            await _sqlDataService.UpdateDepositoAsync(deposito);
            TempData["SuccessMessage"] = "Depósito actualizado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var deposito = await _sqlDataService.GetDepositoByIdAsync(id);
            if (deposito == null)
            {
                return NotFound();
            }

            return View(deposito);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _sqlDataService.DeleteDepositoAsync(id);
            TempData["SuccessMessage"] = "Depósito eliminado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var deposito = await _sqlDataService.GetDepositoByIdAsync(id);
            if (deposito == null)
            {
                return NotFound();
            }

            return View(deposito);
        }
    }
} 