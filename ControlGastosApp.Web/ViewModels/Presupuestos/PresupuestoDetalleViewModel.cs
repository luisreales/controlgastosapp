using System;
using System.Collections.Generic;
using ControlGastosApp.Web.Models;

namespace ControlGastosApp.Web.ViewModels.Presupuestos
{
    public class PresupuestoDetalleViewModel
    {
        public int Id { get; set; }
        public TipoGasto? TipoGasto { get; set; }
        public string? Mes { get; set; }
        public decimal MontoPresupuestado { get; set; }
        public decimal MontoGastado { get; set; }
        public decimal Diferencia => MontoPresupuestado - MontoGastado;
        public List<GastoDetalleViewModel> Gastos { get; set; } = new();
    }

    public class GastoDetalleViewModel
    {
        public DateTime Fecha { get; set; }
        public string? Descripcion { get; set; }
        public decimal Monto { get; set; }
        public string? FondoNombre { get; set; }
    }
} 