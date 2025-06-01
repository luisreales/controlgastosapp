using ControlGastosApp.Web.Models;
using ControlGastosApp.Web.ViewModels.Gastos;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using ControlGastosApp.Web.Data;

namespace ControlGastosApp.Web.Services
{
    public class GastosService
    {
        private readonly SqlDataService _sqlDataService;
        private readonly AppDbContext _context;

        public GastosService(SqlDataService sqlDataService, AppDbContext context)
        {
            _sqlDataService = sqlDataService;
            _context = context;
        }

        public async Task<(bool Success, string Message, List<PresupuestoExcedido>? PresupuestosExcedidos)> ValidarYGuardarGastoAsync(RegistroGasto gasto)
        {
            if (!gasto.TieneDetalles)
            {
                return (false, "El gasto debe tener al menos un detalle.", null);
            }

            // Validar presupuestos
            var presupuestosExcedidos = new List<PresupuestoExcedido>();
            var presupuestos = await _sqlDataService.GetPresupuestosAsync();
            var mesPresupuesto = gasto.Fecha.ToString("yyyy-MM");
            var year = gasto.Fecha.Year;
            var month = gasto.Fecha.Month;

            foreach (var detalle in gasto.Detalles)
            {
                var presupuesto = presupuestos.FirstOrDefault(p =>
                    p.TipoGastoId == detalle.TipoGastoId &&
                    p.Mes == mesPresupuesto);

                if (presupuesto != null)
                {
                    var gastosExistentes = await _context.DetallesGastos
                        .Where(d => d.TipoGastoId == detalle.TipoGastoId &&
                                    d.RegistroGasto.Fecha.Year == year &&
                                    d.RegistroGasto.Fecha.Month == month)
                        .SumAsync(d => d.Monto);

                    var totalGastado = gastosExistentes + detalle.Monto;

                    if (totalGastado > presupuesto.Monto)
                    {
                        presupuestosExcedidos.Add(new PresupuestoExcedido
                        {
                            TipoGastoId = detalle.TipoGastoId,
                            TipoGastoNombre = presupuesto.TipoGasto?.Nombre ?? "Desconocido",
                            MontoPresupuestado = presupuesto.Monto,
                            MontoGastado = totalGastado,
                            Excedente = totalGastado - presupuesto.Monto
                        });
                    }
                }
            }

            if (presupuestosExcedidos.Any())
            {
                return (false, "Se han excedido los presupuestos en algunos tipos de gasto.", presupuestosExcedidos);
            }

            // Validar saldo del fondo monetario
            var fondo = await _context.FondosMonetarios.FindAsync(gasto.FondoMonetarioId);
            if (fondo == null)
            {
                return (false, "No se encontró el fondo monetario especificado.", null);
            }

            var totalGasto = gasto.Detalles.Sum(d => d.Monto);
            if (fondo.Saldo < totalGasto)
            {
                return (false, "El fondo no tiene saldo suficiente para cubrir el gasto.", null);
            }

            // Descontar del fondo
            fondo.Saldo -= totalGasto;

            // Guardar el gasto
            _context.RegistrosGastos.Add(gasto);
            await _context.SaveChangesAsync();
            return (true, "Gasto guardado correctamente.", null);
        }

        public async Task<(bool Success, string Message, List<PresupuestoExcedido>? PresupuestosExcedidos)> ValidarYActualizarGastoAsync(RegistroGasto gasto)
        {
            if (!gasto.TieneDetalles)
            {
                return (false, "El gasto debe tener al menos un detalle.", null);
            }

            // Validar presupuestos
            var presupuestosExcedidos = new List<PresupuestoExcedido>();
            var presupuestos = await _sqlDataService.GetPresupuestosAsync();
            var mesPresupuesto = gasto.Fecha.ToString("yyyy-MM");
            var year = gasto.Fecha.Year;
            var month = gasto.Fecha.Month;

            foreach (var detalle in gasto.Detalles)
            {
                var presupuesto = presupuestos.FirstOrDefault(p =>
                    p.TipoGastoId == detalle.TipoGastoId &&
                    p.Mes == mesPresupuesto);

                if (presupuesto != null)
                {
                    var gastosExistentes = await _context.DetallesGastos
                        .Where(d => d.TipoGastoId == detalle.TipoGastoId &&
                                    d.RegistroGasto.Fecha.Year == year &&
                                    d.RegistroGasto.Fecha.Month == month &&
                                    d.RegistroGastoId != gasto.Id)
                        .SumAsync(d => d.Monto);

                    var totalGastado = gastosExistentes + detalle.Monto;

                    if (totalGastado > presupuesto.Monto)
                    {
                        presupuestosExcedidos.Add(new PresupuestoExcedido
                        {
                            TipoGastoId = detalle.TipoGastoId,
                            TipoGastoNombre = presupuesto.TipoGasto?.Nombre ?? "Desconocido",
                            MontoPresupuestado = presupuesto.Monto,
                            MontoGastado = totalGastado,
                            Excedente = totalGastado - presupuesto.Monto
                        });
                    }
                }
            }

            if (presupuestosExcedidos.Any())
            {
                return (false, "Se han excedido los presupuestos en algunos tipos de gasto.", presupuestosExcedidos);
            }

            // Actualizar el gasto (la lógica para ajustar el saldo del fondo dependerá de cómo manejes la diferencia entre valores anteriores y nuevos)
            _context.Update(gasto);
            await _context.SaveChangesAsync();
            return (true, "Gasto actualizado correctamente.", null);
        }

        public async Task<RegistroGasto?> GetRegistroGastoAsync(int id)
        {
            return await _context.RegistrosGastos
                .Include(r => r.Detalles)
                    .ThenInclude(d => d.TipoGasto)
                .Include(r => r.Fondo)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task DeleteGastoAsync(int id)
        {
            var gasto = await _context.RegistrosGastos
                .Include(g => g.Detalles)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (gasto != null)
            {
                var fondo = await _context.FondosMonetarios.FindAsync(gasto.FondoMonetarioId);
                if (fondo != null)
                {
                    var total = gasto.Detalles.Sum(d => d.Monto);
                    fondo.Saldo += total; // Reintegrar el monto al fondo
                }

                _context.RegistrosGastos.Remove(gasto);
                await _context.SaveChangesAsync();
            }
        }


        public RegistroGasto? GetGasto(int id)
        {
            return _context.RegistrosGastos
                .Include(r => r.Detalles)
                    .ThenInclude(d => d.TipoGasto)
                .Include(r => r.Fondo)
                .FirstOrDefault(r => r.Id == id);
        }
    }

    public class PresupuestoExcedido
    {
        public int TipoGastoId { get; set; }
        public string TipoGastoNombre { get; set; } = string.Empty;
        public decimal MontoPresupuestado { get; set; }
        public decimal MontoGastado { get; set; }
        public decimal Excedente { get; set; }
    }
}
