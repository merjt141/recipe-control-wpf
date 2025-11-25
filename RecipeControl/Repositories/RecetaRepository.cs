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
using System.Windows.Media.Animation;

namespace RecipeControl.Repositories
{
    public class RecetaRepository : IRecetaRepository
    {
        private readonly IDatabaseService _databaseService;
        
        public RecetaRepository(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<Receta> GetByIdAsync(int receatId)
        {
            var sql = @"SELECT * FROM Receta WHERE RecetaId = @RecetaId;";
            var parametro = new SqlParameter("@RecetaId", SqlDbType.Int) { Value = receatId };

            var datos = await _databaseService.ExecuteQueryAsync(sql, parametro);

            var lista = MapDataTableToList(datos);

            return lista.FirstOrDefault() ?? new Receta();
        }

        public Task<IEnumerable<Receta>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Receta> InsertAsync(Receta entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Receta entity)
        {
            // Update Receta
            var sql = @"UPDATE Receta
                        SET Codigo = @Codigo,
                            Descripcion = @Descripcion,
                            EstadoRegistro = @EstadoRegistro,
                            FechaModificacion = @FechaModificacion,
                            UsuarioModificacionId = @UsuarioModificacionId
                      WHERE RecetaId = @RecetaId;";

            // Define parameters
            var parametros = new SqlParameter[]
            {
                new SqlParameter("@Codigo", SqlDbType.NVarChar) { Value = entity.Codigo },
                new SqlParameter("@Descripcion", SqlDbType.NVarChar) { Value = entity.Descripcion ?? (object)DBNull.Value },
                new SqlParameter("@EstadoRegistro", SqlDbType.Bit) { Value = entity.EstadoRegistro },
                new SqlParameter("@FechaModificacion", SqlDbType.DateTime) { Value = entity.FechaModificacion },
                new SqlParameter("@UsuarioModificacionId", SqlDbType.Int) { Value = entity.UsuarioModificacionId },
                new SqlParameter("@RecetaId", SqlDbType.Int) { Value = entity.RecetaId }
            };

            // Execute update
            var result = _databaseService.ExecuteScalarAsync(sql, parametros);

            return Task.FromResult(true);
        }

        public Task<bool> DeleteAsync(int receatId)
        {
            throw new NotImplementedException();
        }

        public async Task<Receta> GetByCodeAsync(string codigo)
        {
            var sql = @"SELECT * FROM Receta WHERE Codigo = @Codigo;";
            var parametro = new SqlParameter("@Codigo", SqlDbType.NVarChar) { Value = codigo };

            var datos = await _databaseService.ExecuteQueryAsync(sql, parametro);

            var lista = MapDataTableToList(datos);

            return lista.FirstOrDefault() ?? new Receta();
        }

        #region Data Modeling

        private static IEnumerable<Receta> MapDataTableToList(DataTable data)
        {
            var list = new List<Receta>();
            foreach (DataRow row in data.Rows)
            {
                list.Add(MapDataRowToClass(row));
            }

            return list;
        }

        private static Receta MapDataRowToClass(DataRow row)
        {
            return new Receta()
            {
                RecetaId = Convert.ToInt32(row["RecetaId"]),
                Codigo = row["Codigo"].ToString() ?? "",
                Descripcion = row["Descripcion"].ToString() ?? "",
                EstadoRegistro = Convert.ToBoolean(row["EstadoRegistro"]),
                FechaCreacion = Convert.ToDateTime(row["FechaCreacion"]),
                FechaModificacion = Convert.ToDateTime(row["FechaModificacion"]),
                UsuarioCreacionId = Convert.ToInt32(row["UsuarioCreacionId"]),
                UsuarioModificacionId = Convert.ToInt32(row["UsuarioModificacionId"])
            };
        }

        #endregion
    }
}
