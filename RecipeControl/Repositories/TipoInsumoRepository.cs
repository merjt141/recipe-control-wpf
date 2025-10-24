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

namespace RecipeControl.Repositories
{
    public class TipoInsumoRepository : ITipoInsumoRepository
    {
        private readonly IDatabaseService _databaseService;

        public TipoInsumoRepository(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<List<TipoInsumo>> GetAllAsync()
        {
            var sql = @"SELECT * FROM TIPO_INSUMO";

            var parametro = new SqlParameter();
            var datos = await _databaseService.ExecuteQueryAsync(sql, parametro);

            return MapDataTableToArray(datos);
        }

        private List<TipoInsumo> MapDataTableToArray(DataTable data)
        {
            var list = new List<TipoInsumo>();
            foreach (DataRow row in data.Rows)
            {
                list.Add(MapDataRowToClass(row));
            }

            return list;
        }

        private TipoInsumo MapDataRowToClass(DataRow row)
        {
            return new TipoInsumo()
            {
                TipoInsumoId = Convert.ToInt32(row["id"]),
                Codigo = row["codigo"].ToString() ?? "",
                Descripcion = row["descripcion"].ToString() ?? "",
                FechaCreacion = Convert.ToDateTime(row["fecha_creacion"]),
                FechaModificacion = Convert.ToDateTime(row["fecha_modificacion"])
            };
        }
    }
}
