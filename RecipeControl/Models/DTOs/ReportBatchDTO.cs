using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Models.DTOs
{
    public class ReportBatchDTO
    {
        public int RegistroBatchWarehouseId { get; set; }
        public int Lote { get; set; }
        public string FormulaCodigo { get; set; } = string.Empty;
        public string RecetaCodigo { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public string InsumoCodigo { get; set; } = string.Empty;
        public decimal DetalleRecetaValor { get; set; }
        public string InsumoUnidad { get; set; } = string.Empty;
        public decimal DetalleRecetaVariacion { get; set; }
        public string RegistroPesoCodigo { get; set; } = string.Empty;
        public decimal RegistroPesoValor { get; set; }
        public decimal RegistroBatchWarehouseValorReal { get; set; }
        public DateTime RegistroBatchFechaPreparacion { get; set; }
    }
}
