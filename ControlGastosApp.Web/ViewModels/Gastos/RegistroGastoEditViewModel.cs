using System.ComponentModel.DataAnnotations;

namespace ControlGastosApp.Web.ViewModels.Gastos;

public class RegistroGastoEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La fecha es requerida")]
    [Display(Name = "Fecha")]
    public DateTime Fecha { get; set; }

    [Required(ErrorMessage = "El fondo monetario es requerido")]
    [Display(Name = "Fondo Monetario")]
    public int FondoId { get; set; }

    [Required(ErrorMessage = "El comercio es requerido")]
    [Display(Name = "Comercio")]
    public string Comercio { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo de documento es requerido")]
    [Display(Name = "Tipo de Documento")]
    public string TipoDocumento { get; set; } = string.Empty;

    [Display(Name = "Observaciones")]
    public string? Observaciones { get; set; }

    public List<DetalleGastoEditViewModel> Detalles { get; set; } = new();
} 