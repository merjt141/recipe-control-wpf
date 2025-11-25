using RecipeControl.Models.Entities.Base;
using RecipeControl.Models.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Models.Entities
{
    public class RegistroBatchDetalle: BaseEntity, IBaseEntity
    {
        public int RegistroBatchDetalleId { get; set; }
        public int RegistroBatchId { get; set; }
        public int InsumoId { get; set; }
        public int RegistroPesoId { get; set; }
    }
}
