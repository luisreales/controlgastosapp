using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ControlGastosApp.Web.Models.Enums;

namespace ControlGastosApp.Web.ViewModels.Gastos;

public class RegistroGastoEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La fecha es requerida")]
    [Display(Name = "Fecha")]
    public DateTime Fecha { get; set; }

    [Required(ErrorMessage = "El fondo es requerido")]
    [Display(Name = "Fondo")]
    public int FondoId { get; set; }

    [Required(ErrorMessage = "El comercio es requerido")]
    [Display(Name = "Comercio")]
    public string Comercio { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo de documento es requerido")]
    [Display(Name = "Tipo de Documento")]
    public TipoDocumento TipoDocumento { get; set; }

    [Display(Name = "Observaciones")]
    public string? Observaciones { get; set; }

    public List<DetalleGastoEditViewModel> Detalles { get; set; } = new();
} 