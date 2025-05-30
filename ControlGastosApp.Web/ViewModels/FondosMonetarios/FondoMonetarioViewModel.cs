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

        [Required(ErrorMessage = "El tipo es requerido")]
        [Display(Name = "Tipo")]
        public required string Tipo { get; set; }

        [Display(Name = "Saldo")]
        [DataType(DataType.Currency)]
        public decimal Saldo { get; set; }
    }

    public class FondoMonetarioListViewModel
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public required string Tipo { get; set; }
        public decimal Saldo { get; set; }
        public string SaldoFormateado => Saldo.ToString("C");
    }
} 