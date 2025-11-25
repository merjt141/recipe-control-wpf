using Microsoft.Identity.Client;
using RecipeControl.Models.Entities.Base;
using RecipeControl.Models.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Models.Entities
{
    public class RegistroBatch : BaseEntity, IBaseEntity
    {
        public int RegistroBatchId { get; set; }
        public string CodigoLote { get; set; } = string.Empty;
        public int RecetaVersionId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string UsuarioBatch { get; set; } = string.Empty;
        public int Estado { get; set; }
        public int EstadoRegistro { get; set; }
    }
}
