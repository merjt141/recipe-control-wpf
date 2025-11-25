using Microsoft.Data.SqlClient;
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

namespace RecipeControl.Repositories
{
    public class RegistroPesoRepository : IRegistroPesoRepository
    {
        private readonly IDatabaseService _databaseService;

        public RegistroPesoRepository(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public Task<RegistroPeso> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<RegistroPeso>> GetAllAsync()
        {
            var sql = @"SELECT * FROM RegistroPeso;";
            var datos = await _databaseService.ExecuteQueryAsync(sql);
            return MapDataTableToList(datos);
        }

        public async Task<RegistroPeso> InsertAsync(RegistroPeso registroPeso)
        {
            var sql = @"sp_InsertarNuevoRegistroPeso";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Descripcion", registroPeso.Descripcion),
                new SqlParameter("@RecetaVersionId", registroPeso.RecetaVersionId),
                new SqlParameter("@InsumoId", registroPeso.InsumoId),
                new SqlParameter("@BalanzaId", registroPeso.BalanzaId),
                new SqlParameter("@UsuarioId", registroPeso.UsuarioId),
                new SqlParameter("@FechaPesado", registroPeso.FechaPesado),
                new SqlParameter("@CantidadPesada", registroPeso.CantidadPesada),
            };
            var result = await _databaseService.ExecuteStoredProcedureAsync(sql, parameters);
            registroPeso = MapDataRowToClass(result.Rows[0]);
            return registroPeso;
        }

        public Task<bool> UpdateAsync(RegistroPeso entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<RegistroPeso>> GetAllActiveAsync()
        {
            var sql = @"SELECT * FROM RegistroPeso WHERE Estado = 1;";
            var datos = await _databaseService.ExecuteQueryAsync(sql);
            return MapDataTableToList(datos);
        }

        public async Task<List<RegisterWeightDataGridDTO>> GetAllDataGridDTOAsync()
        {
            var sql = @"SELECT * FROM vw_RegistroPesoDataGrid;";
            var datos = await _databaseService.ExecuteQueryAsync(sql);
            return MapDataTableToDataGridDTOList(datos);
        }

        #region Data Modeling

        private static List<RegistroPeso> MapDataTableToList(DataTable data)
        {
            var list = new List<RegistroPeso>();
            foreach (DataRow row in data.Rows)
            {
                list.Add(MapDataRowToClass(row));
            }

            return list;
        }

        private static RegistroPeso MapDataRowToClass(DataRow row)
        {
            return new RegistroPeso()
            {
                RegistroPesoId = Convert.ToInt32(row["UsuarioId"]),
                Codigo = row["Codigo"].ToString() ?? "",
                Descripcion = row["Descripcion"].ToString() ?? "",
                RecetaVersionId = Convert.ToInt32(row["RecetaVersionId"]),
                InsumoId = Convert.ToInt32(row["InsumoId"]),
                BalanzaId = Convert.ToInt32(row["BalanzaId"]),
                UsuarioId = Convert.ToInt32(row["UsuarioId"]),
                FechaPesado = Convert.ToDateTime(row["FechaPesado"]),
                CantidadPesada = Convert.ToDecimal(row["CantidadPesada"]),
                Estado = Convert.ToBoolean(row["Estado"]),
                FechaCreacion = Convert.ToDateTime(row["FechaCreacion"]),
                FechaModificacion = Convert.ToDateTime(row["FechaModificacion"])
            };
        }

        private static List<RegisterWeightDataGridDTO> MapDataTableToDataGridDTOList(DataTable data)
        {
            var list = new List<RegisterWeightDataGridDTO>();
            foreach (DataRow row in data.Rows)
            {
                list.Add(new RegisterWeightDataGridDTO()
                {
                    RegistroPesoId = Convert.ToInt32(row["RegistroPesoId"]),
                    InsumoCodigo = row["InsumoCodigo"].ToString() ?? string.Empty,
                    TipoInsumoCodigo = row["TipoInsumoCodigo"].ToString() ?? string.Empty,
                    FechaPesado = Convert.ToDateTime(row["FechaPesado"]),
                    Valor = Convert.ToDecimal(row["Valor"]),
                    UsuarioNombre = row["UsuarioNombre"].ToString() ?? string.Empty,
                    Codigo = row["Codigo"].ToString() ?? string.Empty,
                    Estado = Convert.ToInt32(row["Estado"])
                });
            }
            return list;
        }

        #endregion
    }
}
