using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using RecipeControl.Models.Entities;
using RecipeControl.Repositories.Interfaces;
using RecipeControl.Services.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RecipeControl.Repositories
{
    /// <summary>
    /// Provides methods for managing and accessing user data in the database.
    /// </summary>
    /// <remarks>This repository implements the <see cref="IUsuarioRepository"/> interface and provides
    /// asynchronous methods to retrieve, insert, update, and delete user records. It relies on an <see
    /// cref="IDatabaseService"/> to execute database queries.</remarks>
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IDatabaseService _databaseService;

        public UsuarioRepository(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<Usuario> GetByIdAsync(int usuarioId)
        {
            var sql = @"SELECT * FROM Usuario WHERE UsuarioId = @UsuarioId;";
            var parametro = new SqlParameter("@UsuarioId", SqlDbType.Int) { Value = usuarioId };

            var datos = await _databaseService.ExecuteQueryAsync(sql, parametro);

            var lista = MapDataTableToList(datos);

            return lista.FirstOrDefault() ?? new Usuario();
        }

        public Task<IEnumerable<Usuario>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Usuario> InsertAsync(Usuario entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Usuario entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int usuarioId)
        {
            throw new NotImplementedException();
        }

        public async Task<Usuario> GetByNameAsync(string nombre)
        {
            var sql = @"SELECT * FROM Usuario WHERE Nombre = @Nombre;";
            var parametro = new SqlParameter("@Nombre", SqlDbType.NVarChar) { Value = nombre };

            var datos = await _databaseService.ExecuteQueryAsync(sql, parametro);

            var lista = MapDataTableToList(datos);

            return lista.FirstOrDefault() ?? new Usuario();
        }

        #region Data Modeling

        private static IEnumerable<Usuario> MapDataTableToList(DataTable data)
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
