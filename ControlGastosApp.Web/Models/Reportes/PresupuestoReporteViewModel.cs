namespace ControlGastosApp.Web.Models.Reportes
{
    public class PresupuestoReporteViewModel
    {
        public required string TipoGasto { get; set; }
        public decimal Presupuestado { get; set; }
        public decimal Gastado { get; set; }
        public decimal Diferencia { get; set; }
    }
} 