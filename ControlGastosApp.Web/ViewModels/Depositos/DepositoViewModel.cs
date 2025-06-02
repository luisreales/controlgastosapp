using System.ComponentModel.DataAnnotations;
using ControlGastosApp.Web.ViewModels.FondosMonetarios;

namespace ControlGastosApp.Web.ViewModels.Depositos
{
    public class DepositoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        [Display(Name = "Fecha")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El fondo monetario es requerido")]
        [Display(Name = "Fondo Monetario")]
        public int FondoMonetarioId { get; set; }

        [Required(ErrorMessage = "El monto es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "El monto debe ser un número válido")]
        [DataType(DataType.Currency)]
        public decimal Monto { get; set; }
    }
} 