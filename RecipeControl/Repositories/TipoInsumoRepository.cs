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
            var sql = @"SELECT * FROM TipoInsumo;";

            var datos = await _databaseService.ExecuteQueryAsync(sql);

            return MapDataTableToList(datos);
        }

        #region Data Modeling

        private static List<TipoInsumo> MapDataTableToList(DataTable data)
        {
            var list = new List<TipoInsumo>();
            foreach (DataRow row in data.Rows)
            {
                list.Add(new TipoInsumo()
                {
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
