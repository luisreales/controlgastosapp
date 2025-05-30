using System.ComponentModel.DataAnnotations;

namespace ControlGastosApp.Web.Models
{
    public class FondoMonetario
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Nombre { get; set; }

        public string? Descripcion { get; set; }

        public ICollection<Deposito>? Depositos { get; set; }
    }
} 