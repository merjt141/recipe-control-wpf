using RecipeControl.Models.Entities.Base;
using RecipeControl.Models.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Models.Entities
{
    public class RecetaVersionDetalle : BaseEntity, IBaseEntity
    {
        public int RecetaVersionDetalleId { get; set; }
        public int RecetaVersionId { get; set; }
        public int InsumoId { get; set; }
        public decimal CantidadRequerida { get; set; }
        public decimal ToleranciaMaxima { get; set; }
    }
}
