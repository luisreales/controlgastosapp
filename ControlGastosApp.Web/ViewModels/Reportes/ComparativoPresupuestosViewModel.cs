using System.ComponentModel.DataAnnotations;

namespace ControlGastosApp.Web.ViewModels.Reportes
{
    public class ComparativoPresupuestosViewModel
    {
        [Required(ErrorMessage = "La fecha de inicio es requerida")]
        [Display(Name = "Fecha Inicio")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es requerida")]
        [Display(Name = "Fecha Fin")]
        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; }

        public List<TipoGastoViewModel> TiposGasto { get; set; } = new();
        public List<ComparativoViewModel> Comparativo { get; set; } = new();
    }

    public class TipoGastoViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }

    public class ComparativoViewModel
    {
        public int TipoGastoId { get; set; }
        public string TipoGastoNombre { get; set; } = string.Empty;
        public decimal Presupuestado { get; set; }
        public decimal Ejecutado { get; set; }
        public decimal Diferencia => Presupuestado - Ejecutado;
        public string PresupuestadoFormateado => Presupuestado.ToString("C");
        public string EjecutadoFormateado => Ejecutado.ToString("C");
        public string DiferenciaFormateada => Diferencia.ToString("C");
    }
} 