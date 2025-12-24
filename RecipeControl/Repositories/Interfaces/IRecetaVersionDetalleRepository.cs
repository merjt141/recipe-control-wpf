using RecipeControl.Models.Entities;
using RecipeControl.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Repositories.Interfaces
{
    public interface IRecetaVersionDetalleRepository : IBaseRepository<RecetaVersionDetalle>
    {

        Task<RecetaVersionDetalle> GetRecetaVersionDetalleByRecetaAndInsumoAsync(int recetaVersionId, int insumoId);
    }
}
