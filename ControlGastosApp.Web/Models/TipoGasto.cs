using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ControlGastosApp.Web.Models
{
    public class TipoGasto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [JsonPropertyName("nombre")]
        public required string Nombre { get; set; }

        [JsonPropertyName("codigo")]
        public string? Codigo { get; set; }

        public ICollection<Gasto>? Gastos { get; set; }
        public ICollection<Presupuesto>? Presupuestos { get; set; }
    }
} 