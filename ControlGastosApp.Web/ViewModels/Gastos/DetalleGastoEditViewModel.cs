using System.ComponentModel.DataAnnotations;

namespace ControlGastosApp.Web.ViewModels.Gastos;

public class DetalleGastoEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El tipo de gasto es requerido")]
    [Display(Name = "Tipo de Gasto")]
    public int TipoGastoId { get; set; }

    [Required(ErrorMessage = "El monto es requerido")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
    [Display(Name = "Monto")]
    public decimal Monto { get; set; }
} 