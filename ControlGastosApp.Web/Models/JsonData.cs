using System.Text.Json.Serialization;

namespace ControlGastosApp.Web.Models
{
    public class JsonData
    {
        [JsonPropertyName("tiposGasto")]
        public List<TipoGasto> TiposGasto { get; set; } = new();

        [JsonPropertyName("fondosMonetarios")]
        public List<FondoMonetario> Fondos { get; set; } = new();

        [JsonPropertyName("gastos")]
        public List<RegistroGasto> Gastos { get; set; } = new();

        [JsonPropertyName("presupuestos")]
        public List<Presupuesto> Presupuestos { get; set; } = new();

        [JsonPropertyName("depositos")]
        public List<Deposito> Depositos { get; set; } = new();
    }
} 