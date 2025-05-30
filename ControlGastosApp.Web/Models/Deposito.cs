using System.ComponentModel.DataAnnotations;

namespace ControlGastosApp.Web.Models
{
    public class Deposito
    {
        public int Id { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public int FondoId { get; set; }
        public FondoMonetario FondoMonetario { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Monto { get; set; }

        public string Descripcion { get; set; }
    }
} 