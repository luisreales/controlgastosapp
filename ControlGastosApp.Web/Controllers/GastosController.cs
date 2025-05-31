using Microsoft.AspNetCore.Mvc;
using ControlGastosApp.Web.Services;
using ControlGastosApp.Web.Models;
using ControlGastosApp.Web.ViewModels.Gastos;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Globalization;

namespace ControlGastosApp.Web.Controllers
{
    public class GastosController : Controller
    {
        private readonly JsonDataService _dataService;
        private readonly GastosService _gastosService;

        public GastosController(JsonDataService dataService, GastosService gastosService)
        {
            _dataService = dataService;
            _gastosService = gastosService;
        }

        public IActionResult Index()
        {
            var gastos = _dataService.GetGastos();
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

        public IActionResult Create()
        {
            var tiposGasto = _dataService.GetTiposGasto();
            var fondos = _dataService.GetFondos();

            ViewBag.TiposGasto = new SelectList(tiposGasto, "Id", "Nombre");
            ViewBag.Fondos = new SelectList(fondos, "Id", "Nombre");

            return View(new RegistroGastoCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RegistroGastoCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var tiposGasto = _dataService.GetTiposGasto();
                var fondos = _dataService.GetFondos();

                ViewBag.TiposGasto = new SelectList(tiposGasto, "Id", "Nombre");
                ViewBag.Fondos = new SelectList(fondos, "Id", "Nombre");

                return View(model);
            }

            var gasto = new RegistroGasto
            {
                Fecha = model.Fecha,
                FondoId = model.FondoId,
                Comercio = model.Comercio,
                TipoDocumento = model.TipoDocumento,
                Observaciones = model.Observaciones,
                Detalles = model.Detalles.Select(d => new DetalleGasto
                {
                    TipoGastoId = d.TipoGastoId,
                    Monto = d.Monto
                }).ToList()
            };

            var (success, message, presupuestosExcedidos) = _gastosService.ValidarYGuardarGasto(gasto);

            if (!success)
            {
                if (presupuestosExcedidos != null)
                {
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

                var tiposGasto = _dataService.GetTiposGasto();
                var fondos = _dataService.GetFondos();

                ViewBag.TiposGasto = new SelectList(tiposGasto, "Id", "Nombre");
                ViewBag.Fondos = new SelectList(fondos, "Id", "Nombre");

                return View(model);
            }

            TempData["SuccessMessage"] = message;
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var gasto = _dataService.GetGasto(id);
            if (gasto == null)
            {
                return NotFound();
            }

            var tiposGasto = _dataService.GetTiposGasto();
            var fondos = _dataService.GetFondos();

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
                FondoId = gasto.FondoId,
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
        public IActionResult Edit(int id, RegistroGastoEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var tiposGasto = _dataService.GetTiposGasto();
                var fondos = _dataService.GetFondos();

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
                FondoId = model.FondoId,
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

            var (success, message, presupuestosExcedidos) = _gastosService.ValidarYActualizarGasto(gasto);

            if (!success)
            {
                if (presupuestosExcedidos != null)
                {
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

                var tiposGasto = _dataService.GetTiposGasto();
                var fondos = _dataService.GetFondos();

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

        public IActionResult Delete(int id)
        {
            var gasto = _dataService.GetGasto(id);
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
                Detalles = gasto.Detalles.Select(d => new DetalleGastoViewModel
                {
                    TipoGastoNombre = d.TipoGasto?.Nombre ?? "Desconocido",
                    Monto = d.Monto,
                    MontoFormateado = d.Monto.ToString("C", new CultureInfo("es-CL"))
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _dataService.DeleteGasto(id);
            TempData["SuccessMessage"] = "Gasto eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Detalle(int id)
        {
            var gasto = _dataService.GetGasto(id);
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