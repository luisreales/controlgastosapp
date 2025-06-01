using Microsoft.AspNetCore.Mvc;
using ControlGastosApp.Web.Services;
using ControlGastosApp.Web.Models;
using ControlGastosApp.Web.ViewModels.TiposGasto;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ControlGastosApp.Web.Controllers
{
    public class TiposGastoController : Controller
    {
        private readonly SqlDataService _sqlDataService;
        private readonly ILogger<TiposGastoController> _logger;

        public TiposGastoController(SqlDataService sqlDataService, ILogger<TiposGastoController> logger)
        {
            _sqlDataService = sqlDataService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var tiposGasto = await _sqlDataService.GetTiposGastoAsync();
            return View(tiposGasto);
        }

        public async Task<IActionResult> Create()
        {
            var codigo = await _sqlDataService.GenerarSiguienteCodigoTipoGastoAsync();
            var model = new TipoGastoViewModel
            {
                Codigo = codigo,
                Nombre = string.Empty
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TipoGastoViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Codigo = await _sqlDataService.GenerarSiguienteCodigoTipoGastoAsync();
                
                var nuevoTipoGasto = new TipoGasto
                {
                    Nombre = model.Nombre,
                    Codigo = model.Codigo,
                    Descripcion = model.Descripcion
                };
                await _sqlDataService.AddTipoGastoAsync(nuevoTipoGasto);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var tipoGasto = await _sqlDataService.GetTipoGastoByIdAsync(id);
            if (tipoGasto == null)
            {
                return NotFound();
            }

            var model = new TipoGastoViewModel
            {
                Id = tipoGasto.Id,
                Nombre = tipoGasto.Nombre!,
                Codigo = tipoGasto.Codigo,
                Descripcion = tipoGasto.Descripcion
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TipoGastoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var tipoGasto = await _sqlDataService.GetTipoGastoByIdAsync(model.Id);
                if (tipoGasto == null)
                {
                    return NotFound();
                }

                tipoGasto.Nombre = model.Nombre;
                tipoGasto.Codigo = model.Codigo;
                tipoGasto.Descripcion = model.Descripcion;

                await _sqlDataService.UpdateTipoGastoAsync(tipoGasto);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var tipoGasto = await _sqlDataService.GetTipoGastoByIdAsync(id);
            if (tipoGasto == null)
            {
                return NotFound();
            }

            return View(tipoGasto);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation("Attempting to confirm deletion for TipoGasto with ID: {Id}", id);
            var result = await _sqlDataService.DeleteTipoGastoAsync(id);
            
            if (!result.success)
            {
                _logger.LogWarning("Deletion of TipoGasto with ID {Id} failed: {Message}", id, result.message);
                TempData["ErrorMessage"] = result.message;
            }
            else
            {
                 _logger.LogInformation("Deletion of TipoGasto with ID {Id} succeeded.", id);
                 TempData["SuccessMessage"] = result.message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
} 