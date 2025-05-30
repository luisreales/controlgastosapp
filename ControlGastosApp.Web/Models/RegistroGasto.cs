using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ControlGastosApp.Web.Models
{
    public class RegistroGasto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El fondo monetario es requerido")]
        [JsonPropertyName("fondoId")]
        public int FondoId { get; set; }
        public FondoMonetario? FondoMonetario { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }

        [Required(ErrorMessage = "El nombre del comercio es requerido")]
        [StringLength(100)]
        [JsonPropertyName("comercio")]
        public required string Comercio { get; set; }

        [Required(ErrorMessage = "El tipo de documento es requerido")]
        [JsonPropertyName("tipoDocumento")]
        public required string TipoDocumento { get; set; } // "Comprobante", "Factura", "Otro"

        public List<DetalleGasto> Detalles { get; set; } = new List<DetalleGasto>();
    }
} 