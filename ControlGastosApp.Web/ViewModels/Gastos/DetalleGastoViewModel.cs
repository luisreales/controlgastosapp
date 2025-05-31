namespace ControlGastosApp.Web.ViewModels.Gastos;

public class DetalleGastoViewModel
{
    public int TipoGastoId { get; set; }
    public string TipoGastoNombre { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public string MontoFormateado { get; set; } = string.Empty;
} 