using Microsoft.EntityFrameworkCore;
using ControlGastosApp.Web.Models;

namespace ControlGastosApp.Web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<RegistroGasto> RegistrosGastos { get; set; }
        public DbSet<DetalleGasto> DetallesGastos { get; set; }
        public DbSet<Deposito> Depositos { get; set; }
        public DbSet<Presupuesto> Presupuestos { get; set; }
        public DbSet<TipoGasto> TiposGasto { get; set; }
        public DbSet<FondoMonetario> FondosMonetarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships if needed (Entity Framework Core often infers based on conventions)
            // For example:
            // modelBuilder.Entity<RegistroGasto>()
            //     .HasOne(r => r.Fondo)
            //     .WithMany()
            //     .HasForeignKey(r => r.FondoId);

            // modelBuilder.Entity<DetalleGasto>()
            //     .HasOne(d => d.RegistroGasto)
            //     .WithMany(r => r.Detalles)
            //     .HasForeignKey(d => d.RegistroGastoId);

            // modelBuilder.Entity<DetalleGasto>()
            //     .HasOne(d => d.TipoGasto)
            //     .WithMany()
            //     .HasForeignKey(d => d.TipoGastoId);

            // modelBuilder.Entity<Deposito>()
            //     .HasOne(d => d.FondoMonetario)
            //     .WithMany(f => f.Depositos)
            //     .HasForeignKey(d => d.FondoId);

            // modelBuilder.Entity<Presupuesto>()
            //     .HasOne(p => p.TipoGasto)
            //     .WithMany(t => t.Presupuestos)
            //     .HasForeignKey(p => p.TipoGastoId);

            // modelBuilder.Entity<TipoGasto>()
            //     .HasMany(t => t.Gastos)
            //     .WithOne(g => g.TipoGasto);
            
            // Note: Entity Framework Core conventions can often handle these relationships automatically 
            // if the navigation properties and foreign key names follow standard patterns (e.g., `EntityNameId` for FK).
            // We might need to explicitly configure if the conventions don't match your model exactly.
        }
    }
} 