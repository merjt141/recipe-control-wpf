using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Models.DTOs
{
    public class RegistroBatchDTO
    {
        public int RegistroBatchWarehouseId { get; set; }
        public int Lote { get; set; }
        public int FormulaId { get; set; }
        public string FormulaCodigo { get; set; } = string.Empty;
        public int RecetaId { get; set; }
        public string RecetaCodigo { get; set; } = string.Empty;
        public int InsumoId { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string InsumoCodigo { get; set; } = string.Empty;
        public decimal DetalleRecetaValor { get; set; }
        public string InsumoUnidad { get; set; } = string.Empty;
        public decimal DetalleRecetaVariacion { get; set; }
        public int RegistroPesoId { get; set; }
        public string RegistroPesoCodigo { get; set; } = string.Empty;
        public decimal RegistroPesoValor { get; set; }
        public decimal RegistroBatchWarehouseValorReal { get; set; }
        public DateTime RegistroBatchFechaPreparacion { get; set; }
    }
}
