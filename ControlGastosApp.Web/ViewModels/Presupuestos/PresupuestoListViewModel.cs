using System.ComponentModel.DataAnnotations;

namespace ControlGastosApp.Web.ViewModels.Presupuestos
{
    public class PresupuestoListViewModel
    {
        public int Id { get; set; }
        public required string TipoGastoNombre { get; set; }
        public required string Mes { get; set; }
        public decimal Monto { get; set; }
        public decimal MontoGastado { get; set; }
        public string MontoFormateado => Monto.ToString("C");
        public string MontoGastadoFormateado => MontoGastado.ToString("C");
    }
} 