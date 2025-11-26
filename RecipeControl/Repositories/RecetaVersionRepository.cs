using RecipeControl.Models.DTOs;
using RecipeControl.Models.Entities;
using RecipeControl.Repositories.Interfaces;
using RecipeControl.Services.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace RecipeControl.Repositories
{
    public class RecetaVersionRepository : IRecetaVersionRepository
    {
        private readonly IDatabaseService _databaseService;

        public Task<RecetaVersion> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RecetaVersion>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<RecetaVersion> InsertAsync(RecetaVersion entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(RecetaVersion entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public RecetaVersionRepository(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<IEnumerable<RecetaVersionDTO>> GetAllActiveAsync()
        {
            var sql = @"SELECT * FROM vw_GetAllRecetasActivas;";
            var datos = await _databaseService.ExecuteQueryAsync(sql);
            return MapDataTableToList(datos);
        }

        #region Data Modeling

        private static IEnumerable<RecetaVersionDTO> MapDataTableToList(DataTable data)
        {
            var list = new List<RecetaVersionDTO>();
            foreach (DataRow row in data.Rows)
            {
                list.Add(MapDataRowToClass(row));
            }

            return list;
        }

        private static RecetaVersionDTO MapDataRowToClass(DataRow row)
        {
            return new RecetaVersionDTO()
            {
                RecetaVersionId = Convert.ToInt32(row["RecetaVersionId"]),
                RecetaId = Convert.ToInt32(row["RecetaId"]),
                RecetaCodigo = row["RecetaCodigo"].ToString() ?? string.Empty,
                NumeroVersion = Convert.ToInt32(row["NumeroVersion"]),
                Estado = Convert.ToInt32(row["Estado"])
            };
        }

        #endregion
    }
}
