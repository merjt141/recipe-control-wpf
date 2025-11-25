using RecipeControl.Models.Entities.Base;
using RecipeControl.Models.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Models.Entities
{
    public class TipoInsumo : BaseEntity, IBaseEntity
    {
        public int TipoInsumoId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string? Descripcion { get; set; } = string.Empty;
        public bool EstadoRegistro { get; set; }
    }
}
