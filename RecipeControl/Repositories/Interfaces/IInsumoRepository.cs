using RecipeControl.Models.Entities;
using RecipeControl.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Repositories.Interfaces
{
    public interface IInsumoRepository : IBaseRepository<Insumo>
    {
        Task<List<Insumo>> GetInsumosByTypeAsync(int tipoInsumoId);
        Task<List<Insumo>> GetInsumoByRecetaAndTipoAsync(int recetaVersionId, int tipoInsumoid);
    }
}
