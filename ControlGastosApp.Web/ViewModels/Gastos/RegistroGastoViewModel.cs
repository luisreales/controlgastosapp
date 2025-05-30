using System.ComponentModel.DataAnnotations;
using ControlGastosApp.Web.ViewModels.FondosMonetarios;
using ControlGastosApp.Web.ViewModels.TiposGasto;

namespace ControlGastosApp.Web.ViewModels.Gastos
{
    public class RegistroGastoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        [Display(Name = "Fecha")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El fondo monetario es requerido")]
        [Display(Name = "Fondo Monetario")]
        public int FondoId { get; set; }

        [StringLength(500)]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        [Required(ErrorMessage = "El nombre del comercio es requerido")]
        [StringLength(100)]
        [Display(Name = "Comercio")]
        public required string Comercio { get; set; }

        [Required(ErrorMessage = "El tipo de documento es requerido")]
        [Display(Name = "Tipo de Documento")]
        public required string TipoDocumento { get; set; }

        public List<DetalleGastoViewModel> Detalles { get; set; } = new List<DetalleGastoViewModel>();
    }

    public class DetalleGastoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El tipo de gasto es requerido")]
        [Display(Name = "Tipo de Gasto")]
        public int TipoGastoId { get; set; }

        [Required(ErrorMessage = "El monto es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        [Display(Name = "Monto")]
        [DataType(DataType.Currency)]
        public decimal Monto { get; set; }
    }

    public class RegistroGastoCreateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        [Display(Name = "Fecha")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El fondo monetario es requerido")]
        [Display(Name = "Fondo Monetario")]
        public int FondoId { get; set; }

        [StringLength(500)]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        [Required(ErrorMessage = "El nombre del comercio es requerido")]
        [StringLength(100)]
        [Display(Name = "Comercio")]
        public required string Comercio { get; set; }

        [Required(ErrorMessage = "El tipo de documento es requerido")]
        [Display(Name = "Tipo de Documento")]
        public required string TipoDocumento { get; set; }

        public List<DetalleGastoViewModel> Detalles { get; set; } = new List<DetalleGastoViewModel>();

        public List<FondoMonetarioSelectViewModel>? FondosMonetarios { get; set; }
        public List<TipoGastoSelectViewModel>? TiposGasto { get; set; }
    }

    public class RegistroGastoListViewModel
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string FechaFormateada => Fecha.ToString("dd/MM/yyyy");
        public required string Comercio { get; set; }
        public required string TipoDocumento { get; set; }
        public decimal Total => Detalles.Sum(d => d.Monto);
        public string TotalFormateado => Total.ToString("C");
        public required List<DetalleGastoListViewModel> Detalles { get; set; }
    }

    public class DetalleGastoListViewModel
    {
        public required string TipoGastoNombre { get; set; }
        public decimal Monto { get; set; }
        public string MontoFormateado => Monto.ToString("C");
    }
} 