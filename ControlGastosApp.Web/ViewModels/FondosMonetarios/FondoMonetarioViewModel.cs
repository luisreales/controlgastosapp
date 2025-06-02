using System.ComponentModel.DataAnnotations;

namespace ControlGastosApp.Web.ViewModels.FondosMonetarios
{
    public class FondoMonetarioViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100)]
        [Display(Name = "Nombre")]
        public required string Nombre { get; set; }

        [Required(ErrorMessage = "El saldo es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El saldo debe ser un número mayor a 0")]
        [Display(Name = "Saldo")]
        [DataType(DataType.Currency)]
        public decimal Saldo { get; set; }
    }

    public class FondoMonetarioListViewModel
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public decimal Saldo { get; set; }
        public string SaldoFormateado => Saldo.ToString("C");
    }
} 