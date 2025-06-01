using ControlGastosApp.Web.Models;
using ControlGastosApp.Web.ViewModels.Gastos;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
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
            var mesActual = gasto.Fecha.ToString("MMMM yyyy", new CultureInfo("es-ES"));

            foreach (var detalle in gasto.Detalles)
            {
                var presupuesto = presupuestos.FirstOrDefault(p => 
                    p.TipoGastoId == detalle.TipoGastoId && 
                    p.Mes == mesActual);

                if (presupuesto != null)
                {
                    // Calcular el total gastado hasta ahora (solo gastos existentes para ese tipo de gasto en el mes)
                    var gastosExistentes = await _context.DetallesGastos
                        .Where(d => d.TipoGastoId == detalle.TipoGastoId && d.RegistroGasto.Fecha.ToString("MMMM yyyy", new CultureInfo("es-ES")) == mesActual)
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

            // Si todo está bien, guardar el gasto
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
            var mesActual = gasto.Fecha.ToString("MMMM yyyy", new CultureInfo("es-ES"));

            foreach (var detalle in gasto.Detalles)
            {
                var presupuesto = presupuestos.FirstOrDefault(p => 
                    p.TipoGastoId == detalle.TipoGastoId && 
                    p.Mes == mesActual);

                if (presupuesto != null)
                {
                    // Calcular el total gastado hasta ahora (excluyendo el gasto que se está actualizando)
                    var gastosExistentes = await _context.DetallesGastos
                        .Where(d => d.TipoGastoId == detalle.TipoGastoId && 
                                 d.RegistroGasto.Fecha.ToString("MMMM yyyy", new CultureInfo("es-ES")) == mesActual && 
                                 d.RegistroGastoId != gasto.Id) // Excluir el gasto actual
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

            // Si todo está bien, actualizar el gasto
            _context.Update(gasto); // EF Core rastreará los cambios en gasto y sus detalles
            await _context.SaveChangesAsync();
            return (true, "Gasto actualizado correctamente.", null);
        }

        public async Task<RegistroGasto?> GetRegistroGastoAsync(int id)
        {
            return await _context.RegistrosGastos
                               .Include(r => r.Detalles)
                               .Include(r => r.Fondo)
                               .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task DeleteGastoAsync(int id)
        {
            var gasto = await _context.RegistrosGastos.FindAsync(id);
            if (gasto != null)
            {
                _context.RegistrosGastos.Remove(gasto);
                await _context.SaveChangesAsync();
            }
        }

        // Assuming this method is still needed and should fetch from DB
        public RegistroGasto? GetGasto(int id)
        {
             // This method is synchronous. Consider making calling code asynchronous or handle sync over async carefully.
             return _context.RegistrosGastos
                            .Include(r => r.Detalles)
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