using Microsoft.AspNetCore.Mvc;
using ControlGastosApp.Web.Services;
using ControlGastosApp.Web.Models;
using ControlGastosApp.Web.ViewModels.Gastos;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System;
using ControlGastosApp.Web.Models.Enums;
using Microsoft.AspNetCore.Authorization;

namespace ControlGastosApp.Web.Controllers
{
    [Authorize]
    public class GastosController : Controller
    {
        private readonly GastosService _gastosService;
        private readonly SqlDataService _sqlDataService;

        public GastosController(GastosService gastosService, SqlDataService sqlDataService)
        {
            _gastosService = gastosService;
            _sqlDataService = sqlDataService;
        }

        public async Task<IActionResult> Index()
        {
            var gastos = await _sqlDataService.GetGastosAsync();
            var viewModel = gastos.Select(g => new RegistroGastoListViewModel
            {
                Id = g.Id,
                Fecha = g.Fecha,
                FechaFormateada = g.Fecha.ToString("dd/MM/yyyy"),
                Comercio = g.Comercio,
                TipoDocumento = g.TipoDocumento,
                Total = g.Total,
                TotalFormateado = g.Total.ToString("C", new CultureInfo("es-CL")),
                FondoNombre = g.Fondo?.Nombre ?? "Desconocido"
            }).ToList();

            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            var tiposGasto = await _sqlDataService.GetTiposGastoAsync();
            var fondos = await _sqlDataService.GetFondosAsync();

            var viewModel = new RegistroGastoCreateViewModel
            {
                Fecha = DateTime.Now,
                TipoDocumento = null
            };

            ViewBag.TiposGasto = new SelectList(tiposGasto, "Id", "Nombre");
            ViewBag.Fondos = new SelectList(fondos, "Id", "Nombre");
            ViewBag.TiposDocumento = new SelectList(
                new List<SelectListItem>
                {
                    new SelectListItem { Text = "Comprobante", Value = "Comprobante" },
                    new SelectListItem { Text = "Factura", Value = "Factura" },
                    new SelectListItem { Text = "Otro", Value = "Otro" }
                }, "Value", "Text");

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegistroGastoCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var tiposGasto = await _sqlDataService.GetTiposGastoAsync();
                var fondos = await _sqlDataService.GetFondosAsync();

                ViewBag.TiposGasto = new SelectList(tiposGasto, "Id", "Nombre");
                ViewBag.Fondos = new SelectList(fondos, "Id", "Nombre");
                ViewBag.TiposDocumento = new SelectList(Enum.GetValues(typeof(TipoDocumento))
                    .Cast<TipoDocumento>()
                    .Select(t => new { Value = t.ToString(), Text = t.ToString() }), "Value", "Text");

                var detallesSelectList = new Dictionary<int, SelectList>();
                for (int i = 0; i < model.Detalles.Count; i++)
                {
                    detallesSelectList[i] = new SelectList(tiposGasto, "Id", "Nombre", model.Detalles[i].TipoGastoId);
                }
                ViewBag.DetallesSelectList = detallesSelectList;

                return View(model);
            }

            try
            {
                var gasto = new RegistroGasto
                {
                    Fecha = model.Fecha,
                    FondoMonetarioId = model.FondoMonetarioId,
                    Comercio = model.Comercio,
                    TipoDocumento = model.TipoDocumento ?? throw new InvalidOperationException("El tipo de documento es requerido"),
                    Observaciones = model.Observaciones,
                    Detalles = model.Detalles.Select(d => new DetalleGasto
                    {
                        TipoGastoId = d.TipoGastoId,
                        Monto = d.Monto
                    }).ToList()
                };

                var (success, message, presupuestosExcedidos) = await _gastosService.ValidarYGuardarGastoAsync(gasto);

                if (!success)
                {
                    if (presupuestosExcedidos != null)
                    {
                        ViewBag.TiposGastoExcedidos = presupuestosExcedidos.Select(p => p.TipoGastoId).ToList();

                        foreach (var excedido in presupuestosExcedidos)
                        {
                            ModelState.AddModelError("",
                                $"El presupuesto para el tipo de gasto {excedido.TipoGastoNombre} se ha excedido por {excedido.Excedente.ToString("C", new CultureInfo("es-CL"))}. " +
                                $"Presupuesto: {excedido.MontoPresupuestado.ToString("C", new CultureInfo("es-CL"))}, " +
                                $"Gastado: {excedido.MontoGastado.ToString("C", new CultureInfo("es-CL"))}");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", message);
                    }

                    var tiposGasto = await _sqlDataService.GetTiposGastoAsync();
                    var fondos = await _sqlDataService.GetFondosAsync();

                    ViewBag.TiposGasto = new SelectList(tiposGasto, "Id", "Nombre");
                    ViewBag.Fondos = new SelectList(fondos, "Id", "Nombre");
                    ViewBag.TiposDocumento = new SelectList(Enum.GetValues(typeof(TipoDocumento))
                        .Cast<TipoDocumento>()
                        .Select(t => new { Value = t.ToString(), Text = t.ToString() }), "Value", "Text");

                    return View(model);
                }

                TempData["SuccessMessage"] = message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al procesar la solicitud: " + ex.Message);
                var tiposGasto = await _sqlDataService.GetTiposGastoAsync();
                var fondos = await _sqlDataService.GetFondosAsync();

                ViewBag.TiposGasto = new SelectList(tiposGasto, "Id", "Nombre");
                ViewBag.Fondos = new SelectList(fondos, "Id", "Nombre");
                ViewBag.TiposDocumento = new SelectList(Enum.GetValues(typeof(TipoDocumento))
                    .Cast<TipoDocumento>()
                    .Select(t => new { Value = t.ToString(), Text = t.ToString() }), "Value", "Text");

                return View(model);
            }
        }


        public async Task<IActionResult> Edit(int id)
        {
            var gasto = await _gastosService.GetRegistroGastoAsync(id);
            if (gasto == null)
            {
                return NotFound();
            }

            var tiposGasto = await _sqlDataService.GetTiposGastoAsync();
            var fondos = await _sqlDataService.GetFondosAsync();

            ViewBag.TiposGasto = new SelectList(tiposGasto, "Id", "Nombre");
            ViewBag.Fondos = new SelectList(fondos, "Id", "Nombre");

            var detallesSelectList = new Dictionary<int, SelectList>();
            for (int i = 0; i < gasto.Detalles.Count; i++)
            {
                detallesSelectList[i] = new SelectList(tiposGasto, "Id", "Nombre", gasto.Detalles[i].TipoGastoId);
            }
            ViewBag.DetallesSelectList = detallesSelectList;

            var viewModel = new RegistroGastoEditViewModel
            {
                Id = gasto.Id,
                Fecha = gasto.Fecha,
                FondoMonetarioId = gasto.FondoMonetarioId,
                Comercio = gasto.Comercio,
                TipoDocumento = gasto.TipoDocumento,
                Observaciones = gasto.Observaciones,
                Detalles = gasto.Detalles.Select(d => new DetalleGastoEditViewModel
                {
                    Id = d.Id,
                    TipoGastoId = d.TipoGastoId,
                    Monto = d.Monto
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RegistroGastoEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var tiposGasto = await _sqlDataService.GetTiposGastoAsync();
                var fondos = await _sqlDataService.GetFondosAsync();

                ViewBag.TiposGasto = new SelectList(tiposGasto, "Id", "Nombre");
                ViewBag.Fondos = new SelectList(fondos, "Id", "Nombre");

                var detallesSelectList = new Dictionary<int, SelectList>();
                for (int i = 0; i < model.Detalles.Count; i++)
                {
                    detallesSelectList[i] = new SelectList(tiposGasto, "Id", "Nombre", model.Detalles[i].TipoGastoId);
                }
                ViewBag.DetallesSelectList = detallesSelectList;

                return View(model);
            }

            var gasto = new RegistroGasto
            {
                Id = model.Id,
                Fecha = model.Fecha,
                FondoMonetarioId = model.FondoMonetarioId,
                Comercio = model.Comercio,
                TipoDocumento = model.TipoDocumento,
                Observaciones = model.Observaciones,
                Detalles = model.Detalles.Select(d => new DetalleGasto
                {
                    Id = d.Id,
                    TipoGastoId = d.TipoGastoId,
                    Monto = d.Monto
                }).ToList()
            };

            var (success, message, presupuestosExcedidos) = await _gastosService.ValidarYActualizarGastoAsync(gasto);

            if (!success)
            {
                if (presupuestosExcedidos != null)
                {
                    ViewBag.TiposGastoExcedidos = presupuestosExcedidos.Select(p => p.TipoGastoId).ToList();

                    foreach (var excedido in presupuestosExcedidos)
                    {
                        ModelState.AddModelError("",
                            $"El presupuesto para {excedido.TipoGastoNombre} se ha excedido por {excedido.Excedente.ToString("C", new CultureInfo("es-CL"))}. " +
                            $"Presupuesto: {excedido.MontoPresupuestado.ToString("C", new CultureInfo("es-CL"))}, " +
                            $"Gastado: {excedido.MontoGastado.ToString("C", new CultureInfo("es-CL"))}");
                    }
                }
                else
                {
                    ModelState.AddModelError("", message);
                }

                var tiposGasto = await _sqlDataService.GetTiposGastoAsync();
                var fondos = await _sqlDataService.GetFondosAsync();

                ViewBag.TiposGasto = new SelectList(tiposGasto, "Id", "Nombre");
                ViewBag.Fondos = new SelectList(fondos, "Id", "Nombre");

                var detallesSelectList = new Dictionary<int, SelectList>();
                for (int i = 0; i < model.Detalles.Count; i++)
                {
                    detallesSelectList[i] = new SelectList(tiposGasto, "Id", "Nombre", model.Detalles[i].TipoGastoId);
                }
                ViewBag.DetallesSelectList = detallesSelectList;

                return View(model);
            }

            TempData["SuccessMessage"] = message;
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int id)
        {
            var gasto = await _gastosService.GetRegistroGastoAsync(id);
            if (gasto == null)
            {
                return NotFound();
            }

            return View(gasto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gasto = await _gastosService.GetRegistroGastoAsync(id);
            if (gasto == null)
            {
                return NotFound();
            }

            await _gastosService.DeleteGastoAsync(id);
            TempData["SuccessMessage"] = "Gasto eliminado correctamente. El saldo del fondo fue actualizado.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var gasto = await _gastosService.GetRegistroGastoAsync(id);
            if (gasto == null)
            {
                return NotFound();
            }

            var viewModel = new RegistroGastoListViewModel
            {
                Id = gasto.Id,
                Fecha = gasto.Fecha,
                FechaFormateada = gasto.Fecha.ToString("dd/MM/yyyy"),
                Comercio = gasto.Comercio,
                TipoDocumento = gasto.TipoDocumento,
                Total = gasto.Total,
                TotalFormateado = gasto.Total.ToString("C", new CultureInfo("es-CL")),
                FondoNombre = gasto.Fondo?.Nombre ?? "Desconocido",
                Observaciones = gasto.Observaciones,
                Detalles = gasto.Detalles.Select(d => new DetalleGastoViewModel
                {
                    TipoGastoNombre = d.TipoGasto?.Nombre ?? "Desconocido",
                    Monto = d.Monto,
                    MontoFormateado = d.Monto.ToString("C", new CultureInfo("es-CL"))
                }).ToList()
            };

            return View(viewModel);
        }
    }
} 