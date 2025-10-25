using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Models.DTOs
{
    public class RecetaVersionDTO
    {
        public int RecetaVersionId { get; set; }
        public int RecetaId { get; set; }
        public string RecetaCodigo { get; set; } = string.Empty;
        public int VersionNum { get; set; }
        public int Estado { get; set; }
    }
}
