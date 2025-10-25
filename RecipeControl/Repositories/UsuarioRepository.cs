using Microsoft.Data.SqlClient;
using RecipeControl.Models.Entities;
using RecipeControl.Repositories.Interfaces;
using RecipeControl.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RecipeControl.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IDatabaseService _databaseService;

        public UsuarioRepository(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<List<Usuario>> GetAllAsync()
        {
            var sql = @"SELECT * FROM Usuario;";
            var parametro = new SqlParameter();

            var datos = await _databaseService.ExecuteQueryAsync(sql, parametro);

            return MapDataTableToList(datos);
        }

        public async Task<Usuario?> GetByIdAsync(int usuarioId)
        {
            var sql = @"SELECT * FROM Usuario WHERE UsuarioId = @UsuarioId;";
            var parametro = new SqlParameter("@UsuarioId", SqlDbType.Int) { Value = usuarioId };
            var datos = await _databaseService.ExecuteQueryAsync(sql, parametro);
            var lista = MapDataTableToList(datos);
            return lista.FirstOrDefault();
        }

        #region Data Modeling

        private static List<Usuario> MapDataTableToList(DataTable data)
        {
            var list = new List<Usuario>();
            foreach (DataRow row in data.Rows)
            {
                list.Add(MapDataRowToClass(row));
            }

            return list;
        }

        private static Usuario MapDataRowToClass(DataRow row)
        {
            return new Usuario()
            {
                UsuarioId = Convert.ToInt32(row["UsuarioId"]),
                Nombre = row["Nombre"].ToString() ?? "",
                ClaveHash = row["ClaveHash"].ToString() ?? "",
                FechaCreacion = Convert.ToDateTime(row["FechaCreacion"]),
                FechaModificacion = Convert.ToDateTime(row["FechaModificacion"])
            };
        }

        #endregion
    }
}
