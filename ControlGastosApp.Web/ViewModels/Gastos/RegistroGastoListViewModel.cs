namespace ControlGastosApp.Web.ViewModels.Gastos;

public class RegistroGastoListViewModel
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string FechaFormateada { get; set; } = string.Empty;
    public string Comercio { get; set; } = string.Empty;
    public string TipoDocumento { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string TotalFormateado { get; set; } = string.Empty;
    public string FondoNombre { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public List<DetalleGastoViewModel> Detalles { get; set; } = new();
} 