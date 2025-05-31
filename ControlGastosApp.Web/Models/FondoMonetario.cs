using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ControlGastosApp.Web.Models
{
    public class FondoMonetario
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [JsonPropertyName("nombre")]
        public required string Nombre { get; set; }

        [JsonPropertyName("descripcion")]
        public string? Descripcion { get; set; }

        [JsonPropertyName("tipo")]
        public string? Tipo { get; set; }

        [JsonPropertyName("saldo")]
        public decimal Saldo { get; set; }

        public ICollection<Deposito>? Depositos { get; set; }
    }
} 