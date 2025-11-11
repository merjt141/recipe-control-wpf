using RecipeControl.Models.DTOs;
using RecipeControl.Models.Entities;
using RecipeControl.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Repositories.Interfaces
{
    public interface IRegistroBatchWarehouseRepository : IBaseRepository<RegistroBatchWarehouse>
    {
        Task<IEnumerable<RegistroBatchDTO>> GetAllRegistroBatchDTOAsync();
    }
}
