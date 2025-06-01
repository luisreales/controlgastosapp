using ControlGastosApp.Web.Data;
using ControlGastosApp.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace ControlGastosApp.Web.Services
{
    public class SqlDataService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SqlDataService> _logger;

        public SqlDataService(AppDbContext context, ILogger<SqlDataService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<TipoGasto>> GetTiposGastoAsync()
        {
            return await _context.TiposGasto.ToListAsync();
        }

        public async Task<TipoGasto?> GetTipoGastoByIdAsync(int id)
        {
            return await _context.TiposGasto.FindAsync(id);
        }

        public async Task AddTipoGastoAsync(TipoGasto tipoGasto)
        {
            _context.TiposGasto.Add(tipoGasto);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTipoGastoAsync(TipoGasto tipoGasto)
        {
            _context.TiposGasto.Update(tipoGasto);
            await _context.SaveChangesAsync();
        }

        public async Task<(bool success, string message)> DeleteTipoGastoAsync(int id)
        {
            _logger.LogInformation("Attempting to delete TipoGasto with ID: {Id}", id);

            var tipoGasto = await _context.TiposGasto.FindAsync(id);
            if (tipoGasto == null)
            {
                _logger.LogWarning("TipoGasto with ID {Id} not found for deletion.", id);
                return (false, "Tipo de Gasto no encontrado.");
            }

            // Check for related DetalleGasto records
            var hasRelatedDetallesGasto = await _context.DetallesGastos.AnyAsync(d => d.TipoGastoId == id);
            if (hasRelatedDetallesGasto)
            {
                _logger.LogInformation("TipoGasto with ID {Id} has related DetalleGasto records. Deletion blocked.", id);
                return (false, "No se puede eliminar el tipo de gasto porque tiene gastos relacionados.");
            }

            // Check for related Presupuesto records
            var hasRelatedPresupuestos = await _context.Presupuestos.AnyAsync(p => p.TipoGastoId == id);
            if (hasRelatedPresupuestos)
            {
                _logger.LogInformation("TipoGasto with ID {Id} has related Presupuesto records. Deletion blocked.", id);
                return (false, "No se puede eliminar el tipo de gasto porque tiene presupuestos relacionados.");
            }

            // If no related records, proceed with deletion
            _logger.LogInformation("No related records found for TipoGasto with ID {Id}. Proceeding with deletion.", id);
            _context.TiposGasto.Remove(tipoGasto);
            await _context.SaveChangesAsync();

            _logger.LogInformation("TipoGasto with ID {Id} deleted successfully.", id);
            return (true, "Tipo de Gasto eliminado correctamente.");
        }

        public async Task<List<FondoMonetario>> GetFondosAsync()
        {
            return await _context.FondosMonetarios.ToListAsync();
        }

        public async Task<FondoMonetario?> GetFondoByIdAsync(int id)
        {
            return await _context.FondosMonetarios.FindAsync(id);
        }

        public async Task AddFondoAsync(FondoMonetario fondo)
        {
            _context.FondosMonetarios.Add(fondo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateFondoAsync(FondoMonetario fondo)
        {
            _context.FondosMonetarios.Update(fondo);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteFondoAsync(int id)
        {
            var fondo = await _context.FondosMonetarios.FindAsync(id);
            if (fondo != null)
            {
                _context.FondosMonetarios.Remove(fondo);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Presupuesto>> GetPresupuestosAsync()
        {
            return await _context.Presupuestos.Include(p => p.TipoGasto).ToListAsync();
        }

        public async Task<Presupuesto?> GetPresupuestoByIdAsync(int id)
        {
            return await _context.Presupuestos.FindAsync(id);
        }

        public async Task AddPresupuestoAsync(Presupuesto presupuesto)
        {
            _context.Presupuestos.Add(presupuesto);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePresupuestoAsync(Presupuesto presupuesto)
        {
            _context.Presupuestos.Update(presupuesto);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePresupuestoAsync(int id)
        {
            var presupuesto = await _context.Presupuestos.FindAsync(id);
            if (presupuesto != null)
            {
                _context.Presupuestos.Remove(presupuesto);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<RegistroGasto>> GetGastosAsync()
        {
            return await _context.RegistrosGastos
                .Include(r => r.Detalles)
                .ThenInclude(d => d.TipoGasto)
                .Include(r => r.Fondo)
                .ToListAsync();
        }

        public async Task<List<Deposito>> GetDepositosAsync()
        {
            return await _context.Depositos.Include(d => d.FondoMonetario).ToListAsync();
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

        public async Task<Deposito?> GetDepositoByIdAsync(int id)
        {
            return await _context.Depositos
                .Include(d => d.FondoMonetario)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task AddDepositoAsync(Deposito deposito)
        {
            _context.Depositos.Add(deposito);

            // Update the "saldo" field in FondoMonetario
            var fondo = await _context.FondosMonetarios.FindAsync(deposito.FondoMonetarioId);
            if (fondo != null)
            {
            fondo.Saldo += deposito.Monto;
            _context.FondosMonetarios.Update(fondo);
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateDepositoAsync(Deposito deposito)
        {
            var existingDeposito = await _context.Depositos.AsNoTracking().FirstOrDefaultAsync(d => d.Id == deposito.Id);
            if (existingDeposito != null)
            {
            var fondo = await _context.FondosMonetarios.FindAsync(deposito.FondoMonetarioId);
            if (fondo != null)
            {
                // Adjust the "saldo" field in FondoMonetario
                fondo.Saldo -= existingDeposito.Monto; // Subtract the old amount
                fondo.Saldo += deposito.Monto; // Add the new amount
                _context.FondosMonetarios.Update(fondo);
            }
            }

            _context.Depositos.Update(deposito);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDepositoAsync(int id)
        {
            var deposito = await _context.Depositos.FindAsync(id);
            if (deposito != null)
            {
            var fondo = await _context.FondosMonetarios.FindAsync(deposito.FondoMonetarioId);
            if (fondo != null)
            {
                // Adjust the "saldo" field in FondoMonetario
                fondo.Saldo -= deposito.Monto;
                _context.FondosMonetarios.Update(fondo);
            }

            _context.Depositos.Remove(deposito);
            await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Genera el siguiente código de tipo de gasto siguiendo el formato 'A + número'.
        /// Ejemplo: A10, A11, A12, etc.
        /// </summary>
        /// <returns>El siguiente código disponible</returns>
        public async Task<string> GenerarSiguienteCodigoTipoGastoAsync()
        {
            // Obtener todos los códigos existentes
            var codigosExistentes = await _context.TiposGasto
                .Where(t => t.Codigo != null && t.Codigo.StartsWith("A"))
                .Select(t => t.Codigo)
                .ToListAsync();

            if (!codigosExistentes.Any())
            {
                return "A1"; // Comenzar desde A1
            }

            // Extraer los números de los códigos existentes
            var numeros = codigosExistentes
                .Select(c => int.TryParse(c.Substring(1), out int num) ? num : 0)
                .ToList();

            // Obtener el número más alto y sumar 1
            var siguienteNumero = numeros.Max() + 1;

            // Generar el nuevo código
            var nuevoCodigo = $"A{siguienteNumero}";

            // Verificar que el código no exista (por si acaso)
            while (codigosExistentes.Contains(nuevoCodigo))
            {
                siguienteNumero++;
                nuevoCodigo = $"A{siguienteNumero}";
            }

            return nuevoCodigo;
        }

        // Métodos para interactuar con la base de datos irán aquí
        // Por ejemplo:
        // public async Task<List<Gasto>> GetGastosAsync() { ... }
        // public async Task AddGastoAsync(Gasto gasto) { ... }
    }
} 