using RecipeControl.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Repositories.Interfaces
{
    public interface ITipoInsumoRepository
    {
        Task<List<TipoInsumo>> GetAllAsync();
    }
}
