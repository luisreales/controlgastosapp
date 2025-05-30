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
        public int FondoId { get; set; }

        [Required(ErrorMessage = "El monto es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        [Display(Name = "Monto")]
        [DataType(DataType.Currency)]
        public decimal Monto { get; set; }
    }

    public class DepositoCreateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        [Display(Name = "Fecha")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El fondo monetario es requerido")]
        [Display(Name = "Fondo Monetario")]
        public int FondoId { get; set; }

        [Required(ErrorMessage = "El monto es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        [Display(Name = "Monto")]
        [DataType(DataType.Currency)]
        public decimal Monto { get; set; }

        public List<FondoMonetarioSelectViewModel>? FondosMonetarios { get; set; }
    }

    public class DepositoListViewModel
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string FechaFormateada => Fecha.ToString("dd/MM/yyyy");
        public required string FondoNombre { get; set; }
        public decimal Monto { get; set; }
        public string MontoFormateado => Monto.ToString("C");
    }
} 