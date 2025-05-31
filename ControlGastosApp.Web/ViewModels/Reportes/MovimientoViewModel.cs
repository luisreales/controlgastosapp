namespace ControlGastosApp.Web.ViewModels.Reportes
{
    public class MovimientoViewModel
    {
        public DateTime Fecha { get; set; }
        public string FechaFormateada { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public string FondoNombre { get; set; } = string.Empty;
        public string MontoFormateado => Monto.ToString("C");
    }
} 