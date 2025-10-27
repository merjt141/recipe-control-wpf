using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Models.Entities.Interfaces
{
    public interface IBaseEntity
    {
        DateTime FechaCreacion { get; set; }
        DateTime FechaModificacion { get; set; }
    }
}
