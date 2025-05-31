using System.ComponentModel.DataAnnotations;

namespace ControlGastosApp.Web.ViewModels.Reportes
{
    public class MovimientosReporteViewModel
    {
        [Required(ErrorMessage = "La fecha de inicio es requerida")]
        [Display(Name = "Fecha Inicio")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es requerida")]
        [Display(Name = "Fecha Fin")]
        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; }

        public List<MovimientoViewModel> Movimientos { get; set; } = new();
    }
} 