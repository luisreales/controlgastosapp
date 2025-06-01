using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ControlGastosApp.Web.Models.Enums;

namespace ControlGastosApp.Web.Models
{
    public class RegistroGasto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        [JsonPropertyName("fecha")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El fondo monetario es requerido")]
        [JsonPropertyName("fondoMonetarioId")]
        public int FondoMonetarioId { get; set; }

        [JsonPropertyName("fondo")]
        public FondoMonetario? Fondo { get; set; }

        [Required(ErrorMessage = "El comercio es requerido")]
        [StringLength(100)]
        [JsonPropertyName("comercio")]
        public required string Comercio { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de documento es requerido")]
        [JsonPropertyName("tipoDocumento")]
        public TipoDocumento TipoDocumento { get; set; }

        [StringLength(500)]
        [JsonPropertyName("observaciones")]
        public string? Observaciones { get; set; }

        [JsonPropertyName("detalles")]
        public List<DetalleGasto> Detalles { get; set; } = new List<DetalleGasto>();

        [JsonPropertyName("total")]
        public decimal Total => Detalles.Sum(d => d.Monto);

        public bool TieneDetalles => Detalles != null && Detalles.Any();
    }
} 