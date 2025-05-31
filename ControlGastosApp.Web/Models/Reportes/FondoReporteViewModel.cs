namespace ControlGastosApp.Web.Models.Reportes
{
    public class FondoReporteViewModel
    {
        public required string Nombre { get; set; }
        public decimal Saldo { get; set; }
        public decimal Porcentaje { get; set; }
    }
} 