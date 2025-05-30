using System.ComponentModel.DataAnnotations;

namespace ControlGastosApp.Web.Models
{
    public class Presupuesto
    {
        public int Id { get; set; }

        [Required]
        public int TipoGastoId { get; set; }
        public TipoGasto TipoGasto { get; set; }

        [Required]
        [StringLength(20)]
        public string Mes { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Monto { get; set; }
    }
} 