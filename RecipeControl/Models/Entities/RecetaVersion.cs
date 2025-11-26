using RecipeControl.Models.Entities.Base;
using RecipeControl.Models.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Models.Entities
{
    public class RecetaVersion : BaseEntity, IBaseEntity
    {
        public int RecetaVersionId { get; set; }
        public int RecetaId { get; set; }
        public string NumeroVersion { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int Estado { get; set; }
        public bool EstadoRegistro { get; set; }
    }
}
