using System.ComponentModel.DataAnnotations;

namespace ControlGastosApp.Web.ViewModels.Presupuestos
{
    public class PresupuestoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El tipo de gasto es requerido")]
        [Display(Name = "Tipo de Gasto")]
        public int TipoGastoId { get; set; }

        [Required(ErrorMessage = "El mes es requerido")]
        [Display(Name = "Mes")]
        public required string Mes { get; set; }

        [Required(ErrorMessage = "El monto es requerido")]
        [Range(0, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        [Display(Name = "Monto")]
        [DataType(DataType.Currency)]
        public decimal Monto { get; set; }
    }
} 