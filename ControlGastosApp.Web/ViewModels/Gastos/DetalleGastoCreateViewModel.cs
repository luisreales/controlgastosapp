using System.ComponentModel.DataAnnotations;

namespace ControlGastosApp.Web.ViewModels.Gastos;

public class DetalleGastoCreateViewModel
{
    [Required(ErrorMessage = "El tipo de gasto es requerido")]
    [Display(Name = "Tipo de Gasto")]
    public int TipoGastoId { get; set; }

    [Required(ErrorMessage = "El monto es requerido")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
    [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "El monto debe ser un número válido")]
    [Display(Name = "Monto")]
    public decimal Monto { get; set; }
} 