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
    public class FormulaRepository : IFormulaRepository
    {
        private readonly IDatabaseService _databaseService;

        public FormulaRepository(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public Task<Formula> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Formula>> GetAllAsync()
        {
            var sql = @"SELECT * FROM Formula;";

            var datos = await _databaseService.ExecuteQueryAsync(sql);

            return MapDataTableToList(datos);
        }

        public Task<Formula> InsertAsync(Formula entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Formula entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        #region Data Modeling

        private static IEnumerable<Formula> MapDataTableToList(DataTable data)
        {
            var list = new List<Formula>();
            foreach (DataRow row in data.Rows)
            {
                list.Add(MapDataRowToClass(row));
            }

            return list;
        }

        private static Formula MapDataRowToClass(DataRow row)
        {
            return new Formula()
            {
                FormulaId = Convert.ToInt32(row["FormulaId"]),
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
