using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using ControlGastosApp.Web.Models.Enums;

namespace ControlGastosApp.Web.ViewModels.Gastos;

public class RegistroGastoCreateViewModel
{
    [Required(ErrorMessage = "La fecha es requerida")]
    [Display(Name = "Fecha")]
    [DataType(DataType.Date)]
    public DateTime Fecha { get; set; }

    [Required(ErrorMessage = "El fondo es requerido")]
    [Display(Name = "Fondo")]
    [DataType(DataType.Currency)]
    public int FondoId { get; set; }

    [Required(ErrorMessage = "El comercio es requerido")]
    [Display(Name = "Comercio")]
    public string Comercio { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo de documento es requerido")]
    [Display(Name = "Tipo de Documento")]
    [EnumDataType(typeof(TipoDocumento))]
    public TipoDocumento TipoDocumento { get; set; }

    [Display(Name = "Observaciones")]
    public string? Observaciones { get; set; }

    [Required(ErrorMessage = "Debe agregar al menos un detalle")]
    [MinLength(1, ErrorMessage = "Debe agregar al menos un detalle")]
    public List<DetalleGastoCreateViewModel> Detalles { get; set; } = new();

    public List<SelectListItem> TiposDocumento { get; set; } = new();
} 