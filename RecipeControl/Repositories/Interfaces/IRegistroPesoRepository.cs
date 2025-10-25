using RecipeControl.Models.DTOs;
using RecipeControl.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Repositories.Interfaces
{
    public interface IRegistroPesoRepository
    {
        Task<List<RegistroPeso>> GetAllAsync();
        Task<List<RegistroPeso>> GetAllActiveAsync();
        Task<List<RegisterWeightDataGridDTO>> GetAllDataGridDTO();
        Task<RegistroPeso> InsertAsync(RegistroPeso registroPeso);
    }
}
