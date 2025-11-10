using RecipeControl.Models.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace RecipeControl.Models.Entities
{
    public class RegistroBatchWarehouse : IBaseEntity
    {
        public int RegistroBatchWarehouseId { get; set; }
        public int Lote { get; set; }
        public int FormulaId { get; set; }
        public int RecetaVersionId { get; set; }
        public int InsumoId { get; set; }
        public decimal ValorSetpoint { get; set; }
        public decimal Variacion { get; set; }
        public decimal ValorReal { get; set; }
        public int RegistroPesoId { get; set; }
        public DateTime FechaPreparacion { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime FechaModificacion { get; set; } = DateTime.UtcNow;
    }
}
