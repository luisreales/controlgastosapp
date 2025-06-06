using Microsoft.AspNetCore.Mvc;
using ControlGastosApp.Web.Services;
using ControlGastosApp.Web.Models;
using ControlGastosApp.Web.ViewModels.FondosMonetarios;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ControlGastosApp.Web.Controllers
{
    [Authorize]
    public class FondosController : Controller
    {
        private readonly SqlDataService _sqlDataService;

        public FondosController(SqlDataService sqlDataService)
        {
            _sqlDataService = sqlDataService;
        }

        public async Task<IActionResult> Index()
        {
            var fondos = await _sqlDataService.GetFondosAsync();
            return View(fondos);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(FondoMonetarioViewModel model)
        {
            if (ModelState.IsValid)
            {
                var nuevoFondo = new FondoMonetario
                {
                    Nombre = model.Nombre,
                    Saldo = model.Saldo
                };
                await _sqlDataService.AddFondoAsync(nuevoFondo);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var fondo = await _sqlDataService.GetFondoByIdAsync(id);
            if (fondo == null)
            {
                return NotFound();
            }

            var model = new FondoMonetarioViewModel
            {
                Id = fondo.Id,
                Nombre = fondo.Nombre,
                Saldo = fondo.Saldo
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(FondoMonetarioViewModel model)
        {
            if (ModelState.IsValid)
            {
                var fondo = await _sqlDataService.GetFondoByIdAsync(model.Id);
                if (fondo == null)
                {
                    return NotFound();
                }

                fondo.Nombre = model.Nombre;
                fondo.Saldo = model.Saldo;

                await _sqlDataService.UpdateFondoAsync(fondo);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var fondo = await _sqlDataService.GetFondoByIdAsync(id);
            if (fondo == null)
            {
                return NotFound();
            }

            return View(fondo);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _sqlDataService.DeleteFondoAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
} 