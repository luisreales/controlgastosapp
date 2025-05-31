using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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
        [JsonPropertyName("fondoId")]
        public int FondoId { get; set; }

        [Required(ErrorMessage = "El comercio es requerido")]
        [StringLength(100)]
        [JsonPropertyName("comercio")]
        public required string Comercio { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de documento es requerido")]
        [JsonPropertyName("tipoDocumento")]
        public required string TipoDocumento { get; set; } = string.Empty; // "Comprobante", "Factura", "Otro"

        [StringLength(500)]
        [JsonPropertyName("observaciones")]
        public string? Observaciones { get; set; }

        [JsonPropertyName("detalles")]
        public List<DetalleGasto> Detalles { get; set; } = new List<DetalleGasto>();

        public decimal Total => Detalles?.Sum(d => d.Monto) ?? 0;

        public bool TieneDetalles => Detalles != null && Detalles.Any();

        public FondoMonetario? Fondo { get; set; }
    }
} 