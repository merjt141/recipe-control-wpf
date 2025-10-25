using RecipeControl.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Repositories.Interfaces
{
    public interface IRecetaVersionRepository
    {
        Task<List<RecetaVersionDTO>> GetAllActiveAsync();
    }
}
