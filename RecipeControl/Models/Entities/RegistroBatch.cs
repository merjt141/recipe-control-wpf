using Microsoft.Identity.Client;
using RecipeControl.Models.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Models.Entities
{
    public class RegistroBatch : IBaseEntity
    {
        public int RegistroBatchId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int Lote { get; set; }
        public int DetalleRecetaId { get; set; }
        public int RegistroPesoId { get; set; }
        public DateTime FechaPreparacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
    }
}
