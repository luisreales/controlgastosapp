namespace ControlGastosApp.Web.ViewModels.Shared
{
    public class TipoGastoSelectViewModel
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
    }

    public class FondoMonetarioSelectViewModel
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
    }
} 