using RecipeControl.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipeControl.Services.Interfaces;
using RecipeControl.Models.Entities;
using System.Data.SqlTypes;
using Microsoft.Data.SqlClient;
using System.Data;

namespace RecipeControl.Repositories
{
    public class InsumoRepository : IInsumoRepository
    {
        private readonly IDatabaseService _databaseService;

        public InsumoRepository(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<List<Insumo>> GetInsumosByTypeAsync(int tipoInsumoId)
        {
            var sql = @"SELECT * FROM Insumo WHERE TipoInsumoId = @TipoInsumoId;";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@TipoInsumoId", tipoInsumoId)
            };
            var datos = await _databaseService.ExecuteQueryAsync(sql, parameters);
            return MapDataTableToList(datos);
        }

        public async Task<List<Insumo>> GetInsumoByRecipeAndTypeAsync(int recetaVersionId, int tipoInsumoid)
        {
            var sql = @"sp_GetInsumosPorRecetaYTipo";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@RecetaVersionId", recetaVersionId),
                new SqlParameter("@TipoInsumoId", tipoInsumoid)
            };
            var datos = await _databaseService.ExecuteStoredProcedureAsync(sql, parameters);
            return MapDataTableToList(datos);
        }

        #region Data Modeling
        private static List<Insumo> MapDataTableToList(DataTable data)
        {
            var list = new List<Insumo>();
            foreach (DataRow row in data.Rows)
            {
                list.Add(new Insumo()
                {
                    InsumoId = Convert.ToInt32(row["InsumoId"]),
                    TipoInsumoId = Convert.ToInt32(row["TipoInsumoId"]),
                    Codigo = row["Codigo"].ToString() ?? "",
                    Descripcion = row["Descripcion"].ToString() ?? "",
                    FechaCreacion = Convert.ToDateTime(row["FechaCreacion"]),
                    FechaModificacion = Convert.ToDateTime(row["FechaModificacion"])
                });
            }
            return list;
        }
        #endregion
    }
}
