using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ControlGastosApp.Web.Models
{
    public class Deposito
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required]
        [JsonPropertyName("fecha")]
        public DateTime Fecha { get; set; }

        [Required]
        [JsonPropertyName("fondoMonetarioId")]
        public int FondoMonetarioId { get; set; }

        [JsonPropertyName("fondoMonetario")]
        public FondoMonetario? FondoMonetario { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        [JsonPropertyName("monto")]
        public decimal Monto { get; set; }

        [JsonPropertyName("descripcion")]
        public string? Descripcion { get; set; }
    }
} 