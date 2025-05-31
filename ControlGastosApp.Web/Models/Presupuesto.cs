using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ControlGastosApp.Web.Models
{
    public class Presupuesto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required]
        [JsonPropertyName("tipoGastoId")]
        public int TipoGastoId { get; set; }
        public TipoGasto? TipoGasto { get; set; }

        [Required]
        [StringLength(20)]
        [JsonPropertyName("mes")]
        public required string Mes { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [JsonPropertyName("monto")]
        public decimal Monto { get; set; }
    }
} 