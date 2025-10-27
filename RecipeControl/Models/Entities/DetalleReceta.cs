using RecipeControl.Models.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Models.Entities
{
    public class DetalleReceta : IBaseEntity
    {
        public int DetalleRecetaId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int FormulaId { get; set; }
        public int RecetaVersionId { get; set; }
        public int InsumoId { get; set; }
        public decimal Valor { get; set; }
        public decimal Variacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
    }
}
