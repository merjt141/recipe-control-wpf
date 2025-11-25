using RecipeControl.Models.Entities.Base;
using RecipeControl.Models.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Models.Entities
{
    public class Usuario : BaseEntity, IBaseEntity
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string ClaveHash { get; set; } = string.Empty;
        public bool EstadoRegistro { get; set; }
    }
}
