using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ControlGastosApp.Web.ViewModels.Presupuestos
{
    public class PresupuestoCreateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El tipo de gasto es requerido")]
        [Display(Name = "Tipo de Gasto")]
        public int TipoGastoId { get; set; }

        [Required(ErrorMessage = "El mes es requerido")]
        [Display(Name = "Mes")]
        public required string Mes { get; set; }

        [Required(ErrorMessage = "El monto es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "El monto debe ser un número válido")]
        [Display(Name = "Monto")]
        [DataType(DataType.Currency)]
        public decimal Monto { get; set; }

        public List<TipoGastoSelectViewModel>? TiposGasto { get; set; }
    }
} 