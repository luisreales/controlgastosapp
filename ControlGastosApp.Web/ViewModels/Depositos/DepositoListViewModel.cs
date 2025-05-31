using System.ComponentModel.DataAnnotations;

namespace ControlGastosApp.Web.ViewModels.Depositos
{
    public class DepositoListViewModel
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string FechaFormateada { get; set; } = string.Empty;
        public string FondoNombre { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public string MontoFormateado { get; set; } = string.Empty;
    }
} 