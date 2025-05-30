using System.ComponentModel.DataAnnotations;

namespace ControlGastosApp.Web.Models
{
    public class Gasto
    {
        public int Id { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public int TipoGastoId { get; set; }
        public TipoGasto TipoGasto { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Monto { get; set; }

        public string Descripcion { get; set; }
    }
} 