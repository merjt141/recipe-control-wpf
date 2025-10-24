using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Models.Entities
{
    public class Insumo
    {
        public int InsumoId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int TipoInsumoId { get; set; }
        public string Unidad { get; set; } = string.Empty;
        public DateTime FechaCreacion {  get; set; }
        public DateTime FechaModificacion { get; set; }
    }
}
