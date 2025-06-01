using Microsoft.EntityFrameworkCore;
using ControlGastosApp.Web.Models;

namespace ControlGastosApp.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<FondoMonetario> FondosMonetarios { get; set; }
        public DbSet<TipoGasto> TiposGasto { get; set; }
        public DbSet<Gasto> Gastos { get; set; }
        public DbSet<Deposito> Depositos { get; set; }
        public DbSet<Presupuesto> Presupuestos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships and constraints here
            modelBuilder.Entity<FondoMonetario>()
                .HasMany(f => f.Depositos)
                .WithOne(d => d.FondoMonetario)
                .HasForeignKey(d => d.FondoMonetarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TipoGasto>()
                .HasMany(t => t.Gastos)
                .WithOne(g => g.TipoGasto)
                .HasForeignKey(g => g.TipoGastoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TipoGasto>()
                .HasMany(t => t.Presupuestos)
                .WithOne(p => p.TipoGasto)
                .HasForeignKey(p => p.TipoGastoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 