using System;

namespace ControlGastosApp.Web.Models.Reportes
{
    public class MovimientosViewModel
    {
        public DateTime Fecha { get; set; }
        public required string Descripcion { get; set; }
        public decimal? Entradas { get; set; }
        public decimal? Salidas { get; set; }
        public decimal Saldo { get; set; }
        public required string TipoMovimiento { get; set; } // "Depósito" o "Gasto"
    }
} 