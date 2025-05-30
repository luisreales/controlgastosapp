using System.ComponentModel.DataAnnotations;

namespace ControlGastosApp.Web.ViewModels.TiposGasto
{
    public class TipoGastoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El código es requerido")]
        [StringLength(10)]
        [Display(Name = "Código")]
        public required string Codigo { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100)]
        [Display(Name = "Nombre")]
        public required string Nombre { get; set; }

        [StringLength(500)]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }
    }
} 