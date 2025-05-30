using System.ComponentModel.DataAnnotations;

namespace ControlGastosApp.Web.Models
{
    public class DetalleGasto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El registro de gasto es requerido")]
        public int RegistroGastoId { get; set; }
        public RegistroGasto RegistroGasto { get; set; }

        [Required(ErrorMessage = "El tipo de gasto es requerido")]
        public int TipoGastoId { get; set; }
        public TipoGasto TipoGasto { get; set; }

        [Required(ErrorMessage = "El monto es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal Monto { get; set; }
    }
} 