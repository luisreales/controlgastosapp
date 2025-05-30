using System.ComponentModel.DataAnnotations;

namespace ControlGastosApp.Web.Models
{
    public class TipoGasto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Nombre { get; set; }

        public string? Descripcion { get; set; }

        public ICollection<Gasto>? Gastos { get; set; }
        public ICollection<Presupuesto>? Presupuestos { get; set; }
    }
} 