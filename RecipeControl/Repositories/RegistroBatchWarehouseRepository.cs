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
    public class RegistroBatchWarehouseRepository
    {
        private readonly IDatabaseService _databaseService;

        public RegistroBatchWarehouseRepository(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<RegistroBatchDTO>> GetAllRegistroBatchDTOAsync()
        {
            var sql = @"SELECT * FROM vw_ObtenerRegistrosBatchWarehouse";
            var datos = await _databaseService.ExecuteQueryAsync(sql);
            return MapDataTableToDataGridDTOList(datos);
        }

        public Task<IEnumerable<RegistroBatchDTO>> GetBySomeCriteriaAsync(string criteria)
        {
            throw new NotImplementedException();
        }

        #region Data Modeling
        private static List<RegistroBatchDTO> MapDataTableToDataGridDTOList(DataTable data)
        {
            var list = new List<RegistroBatchDTO>();
            foreach (DataRow row in data.Rows)
            {
                list.Add(new RegistroBatchDTO
                {
                    RegistroBatchWarehouseId = Convert.ToInt32(row["RegistroBatchWarehouseId"]),
                    Lote = Convert.ToInt32(row["Lote"]),
                    FormulaId = Convert.ToInt32(row["FormulaId"]),
                    FormulaCodigo = Convert.ToString(row["FormulaCodigo"]) ?? string.Empty,
                    RecetaId = Convert.ToInt32(row["RecetaId"]),
                    RecetaCodigo = Convert.ToString(row["RecetaCodigo"]) ?? string.Empty,
                    InsumoId = Convert.ToInt32(row["InsumoId"]),
                    Usuario = Convert.ToString(row["Usuario"]) ?? string.Empty,
                    InsumoCodigo = Convert.ToString(row["InsumoCodigo"]) ?? string.Empty,
                    DetalleRecetaValor = Convert.ToDecimal(row["DetalleRecetaValor"]),
                    InsumoUnidad = Convert.ToString(row["InsumoUnidad"]) ?? string.Empty,
                    DetalleRecetaVariacion = Convert.ToDecimal(row["DetalleRecetaVariacion"]),
                    RegistroPesoId = Convert.ToInt32(row["RegistroPesoId"]),
                    RegistroPesoCodigo = Convert.ToString(row["RegistroPesoCodigo"]) ?? string.Empty,
                    RegistroPesoValor = Convert.ToDecimal(row["RegistroPesoValor"]),
                    RegistroBatchWarehouseValorReal = Convert.ToDecimal(row["RegistroBatchWarehouseValorReal"]),
                    RegistroBatchFechaPreparacion = Convert.ToDateTime(row["FechaPreparacion"])
                });
            }
            return list;
        }
        #endregion
    }
}
