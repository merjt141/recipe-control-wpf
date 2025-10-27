using RecipeControl.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipeControl.Models.Entities;
using System.Data.SqlTypes;
using Microsoft.Data.SqlClient;
using System.Data;
using RecipeControl.Services.Database;

namespace RecipeControl.Repositories
{
    public class InsumoRepository : IInsumoRepository
    {
        private readonly IDatabaseService _databaseService;

        public InsumoRepository(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<Insumo> GetByIdAsync(int id)
        {
            var sql = @"SELECT * FROM Insumo WHERE InsumoId = @InsumoId;";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@InsumoId", id)
            };
            var datos = await _databaseService.ExecuteQueryAsync(sql, parameters);
            var list = MapDataTableToList(datos);
            return list.FirstOrDefault() ?? new Insumo();
        }

        public async Task<IEnumerable<Insumo>> GetAllAsync()
        {
            var sql = @"SELECT * FROM Insumo;";
            var datos = await _databaseService.ExecuteQueryAsync(sql);
            return MapDataTableToList(datos);
        }

        public async Task<Insumo> InsertAsync(Insumo entity)
        {
            var sql = @"INSERT INTO Insumo (Codigo, Descripcion, TipoInsumoId, Unidad)
                        VALUES (@Codigo, @Descripcion, @TipoInsumoId, @Unidad);
                        SELECT SCOPE_IDENTITY();";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Codigo", entity.Codigo),
                new SqlParameter("@Descripcion", entity.Descripcion ?? (object)SqlString.Null),
                new SqlParameter("@TipoInsumoId", entity.TipoInsumoId),
                new SqlParameter("@Unidad", entity.Unidad),
            };
            var result = await _databaseService.ExecuteCommandAsync(sql, parameters);
            entity.InsumoId = Convert.ToInt32(result);
            return entity;
        }

        public async Task<bool> UpdateAsync(Insumo entity)
        {
            var sql = @"UPDATE Insumo
                        SET Codigo = @Codigo,
                            Descripcion = @Descripcion,
                            TipoInsumoId = @TipoInsumoId,
                            Unidad = @Unidad,
                            FechaModificacion = GETDATE()
                        WHERE InsumoId = @InsumoId;";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Codigo", entity.Codigo),
                new SqlParameter("@Descripcion", entity.Descripcion ?? (object)SqlString.Null),
                new SqlParameter("@TipoInsumoId", entity.TipoInsumoId),
                new SqlParameter("@Unidad", entity.Unidad),
                new SqlParameter("@InsumoId", entity.InsumoId)
            };
            var result = await _databaseService.ExecuteCommandAsync(sql, parameters);
            return result > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sql = @"DELETE FROM Insumo WHERE InsumoId = @InsumoId;";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@InsumoId", id)
            };
            var result = await _databaseService.ExecuteCommandAsync(sql, parameters);
            return result > 0;
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
