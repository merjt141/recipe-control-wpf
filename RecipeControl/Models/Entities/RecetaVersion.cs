using RecipeControl.Models.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Models.Entities
{
    public class RecetaVersion : IBaseEntity
    {
        public int RecetaVersionId { get; set; }
        public int RecetaId { get; set; }
        public string VersionNum { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
    }
}
