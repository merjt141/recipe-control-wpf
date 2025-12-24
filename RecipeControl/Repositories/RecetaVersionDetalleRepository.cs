using Microsoft.Data.SqlClient;
using RecipeControl.Models.Entities;
using RecipeControl.Repositories.Interfaces;
using RecipeControl.Services.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Repositories
{
    public class RecetaVersionDetalleRepository : IRecetaVersionDetalleRepository
    {
        private readonly IDatabaseService _databaseService;

        public RecetaVersionDetalleRepository(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<RecetaVersionDetalle>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
        public Task<RecetaVersionDetalle> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public Task<RecetaVersionDetalle> InsertAsync(RecetaVersionDetalle entity)
        {
            throw new NotImplementedException();
        }
        public Task<bool> UpdateAsync(RecetaVersionDetalle entity)
        {
            throw new NotImplementedException();
        }
        public async Task<RecetaVersionDetalle> GetRecetaVersionDetalleByRecetaAndInsumoAsync(int recetaVersionId, int insumoId)
        {
            var sql = @"SELECT * FROM RecetaVersionDetalle WHERE RecetaVersionId = @RecetaVersionId AND InsumoId = @InsumoId;";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@RecetaVersionId", recetaVersionId),
                new SqlParameter("@InsumoId", insumoId)
            };
            var datos = await _databaseService.ExecuteQueryAsync(sql, parameters);
            return MapDataTableToList(datos).FirstOrDefault() ?? new RecetaVersionDetalle();
        }

        #region Data Modeling

        private static IEnumerable<RecetaVersionDetalle> MapDataTableToList(DataTable data)
        {
            var list = new List<RecetaVersionDetalle>();
            foreach (DataRow row in data.Rows)
            {
                list.Add(MapDataRowToClass(row));
            }

            return list;
        }

        private static RecetaVersionDetalle MapDataRowToClass(DataRow row)
        {
            return new RecetaVersionDetalle()
            {
                RecetaVersionDetalleId = Convert.ToInt32(row["RecetaVersionDetalleId"]),
                RecetaVersionId = Convert.ToInt32(row["RecetaVersionId"]),
                InsumoId = Convert.ToInt32(row["InsumoId"]),
                CantidadRequerida = Convert.ToDecimal(row["CantidadRequerida"]),
                ToleranciaMaxima = Convert.ToDecimal(row["ToleranciaMaxima"]),
                FechaCreacion = Convert.ToDateTime(row["FechaCreacion"]),
                FechaModificacion = Convert.ToDateTime(row["FechaModificacion"]),
                UsuarioCreacionId = Convert.ToInt32(row["UsuarioCreacionId"]),
                UsuarioModificacionId = Convert.ToInt32(row["UsuarioModificacionId"]),
            };
        }

        #endregion
    }
}
