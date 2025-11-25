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
    public class BalanzaRepository : IBalanzaRepository
    {
        private readonly IDatabaseService _databaseService;

        public BalanzaRepository(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public Task<Balanza> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Balanza>> GetAllAsync()
        {
            var sql = @"SELECT * FROM Balanza;";

            var datos = await _databaseService.ExecuteQueryAsync(sql);

            return MapDataTableToList(datos);
        }

        public Task<Balanza> InsertAsync(Balanza entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Balanza entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        #region Data Modeling

        private static IEnumerable<Balanza> MapDataTableToList(DataTable data)
        {
            var list = new List<Balanza>();
            foreach (DataRow row in data.Rows)
            {
                list.Add(MapDataRowToClass(row));
            }

            return list;
        }

        private static Balanza MapDataRowToClass(DataRow row)
        {
            return new Balanza()
            {
                BalanzaId = Convert.ToInt32(row["BalanzaId"]),
                Codigo = row["Codigo"].ToString() ?? "",
                Descripcion = row["Descripcion"].ToString() ?? "",
                EstadoRegistro = Convert.ToBoolean(row["EstadoRegistro"]),
                FechaCreacion = Convert.ToDateTime(row["FechaCreacion"]),
                FechaModificacion = Convert.ToDateTime(row["FechaModificacion"]),
                UsuarioCreacionId = Convert.ToInt32(row["UsuarioCreacionId"]),
                UsuarioModificacionId = Convert.ToInt32(row["UsuarioModificacionId"]),
            };
        }

        #endregion
    }
}
