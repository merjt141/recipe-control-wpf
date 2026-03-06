using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using RecipeControl.Configuration; // o System.Data.SqlClient
using RecipeControl.Models;
using System;
using System.Data;
using System.Diagnostics;
using System.DirectoryServices;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static RecipeControl.Models.DTOs;

namespace RegMicro.Services.Database
{
    public static class DatabaseService
    {
        public static readonly string _connectionString = ConfigurationManager.Instance.Settings.ConnectionStrings.DefaultConnection;

        public static async Task<int?> GetLastRegistroBatchIdAsync()
        {
            var builder = new SqlConnectionStringBuilder(_connectionString)
            {
                ConnectTimeout = 5
            };

            await using var cs = new SqlConnection(builder.ConnectionString);

            const string sql = @"
            SELECT TOP (1) BatchRegistroId
            FROM dbo.MCR_BatchRegistro
            ORDER BY BatchRegistroId DESC;";

            await cs.OpenAsync();
            await using var cmd = new SqlCommand(sql, cs) { CommandTimeout = 5 };

            object? result = await cmd.ExecuteScalarAsync();
            if (result == null || result == DBNull.Value)
            {
                MessageBox.Show("No se ha encontrado ningún batch registrado", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            return Convert.ToInt32(result);
        }

        // fecha: 26 02 2026, inserta datos de registro pistola
        public static async Task InsertBatchRegistroBolsa(DataTransferQR2DTO qRDataTransferDTO2)
        {
            if (qRDataTransferDTO2 == null)
                return;

            string sql = @"
IF EXISTS (SELECT 1 FROM dbo.MCR_BatchRegistroBolsas WITH (UPDLOCK, HOLDLOCK) WHERE MacroRegistroId = @MacroRegistroId)
BEGIN
    UPDATE dbo.MCR_BatchRegistroBolsas
    SET
        BatchRegistroId = @BatchRegistroId,
        FechaPistoleo   = @FechaPistoleo,
        Usuario         = @Usuario,
        FechaCreacion   = @FechaCreacion,
        Lote            = @Lote,
        NombreMacroId   = @NombreMacroId,
        PesoTotRealGr   = @PesoTotRealGr,
        PesoTotObjGr    = @PesoTotObjGr,
        Insumo1Id       = @Insumo1Id,
        Insumo2Id       = @Insumo2Id,
        Insumo3Id       = @Insumo3Id,
        Insumo4Id       = @Insumo4Id
    WHERE MacroRegistroId = @MacroRegistroId;
END
ELSE
BEGIN
    INSERT INTO dbo.MCR_BatchRegistroBolsas
    (
        BatchRegistroId,
        MacroRegistroId,
        FechaPistoleo,
        Usuario,
        FechaCreacion,
        Lote,
        NombreMacroId,
        PesoTotRealGr,
        PesoTotObjGr,
        Insumo1Id,
        Insumo2Id,
        Insumo3Id,
        Insumo4Id
    )
    VALUES
    (
        @BatchRegistroId,
        @MacroRegistroId,
        @FechaPistoleo,
        @Usuario,
        @FechaCreacion,
        @Lote,
        @NombreMacroId,
        @PesoTotRealGr,
        @PesoTotObjGr,
        @Insumo1Id,
        @Insumo2Id,
        @Insumo3Id,
        @Insumo4Id
    );
END;";

            var builder = new SqlConnectionStringBuilder(_connectionString)
            {
                ConnectTimeout = 5
            };

            await using var cs = new SqlConnection(builder.ConnectionString);
            await cs.OpenAsync();

            await using var tx = await cs.BeginTransactionAsync();

            try
            {
                await using var cmd = new SqlCommand(sql, cs, (SqlTransaction)tx);

                cmd.Parameters.Add("@BatchRegistroId", SqlDbType.Int).Value = qRDataTransferDTO2.BatchRegistroId;
                cmd.Parameters.Add("@MacroRegistroId", SqlDbType.Int).Value = qRDataTransferDTO2.MacroRegistroId;

                cmd.Parameters.Add("@FechaPistoleo", SqlDbType.DateTime).Value = qRDataTransferDTO2.FechaPistoleo;
                cmd.Parameters.Add("@FechaCreacion", SqlDbType.DateTime).Value = qRDataTransferDTO2.Fecha;

                cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 20).Value = qRDataTransferDTO2.UsuarioStr.ToString();

                cmd.Parameters.Add("@Lote", SqlDbType.Int).Value = qRDataTransferDTO2.LoteStr;
                cmd.Parameters.Add("@NombreMacroId", SqlDbType.Int).Value = qRDataTransferDTO2.NombreMacroIdStr;

                var pReal = cmd.Parameters.Add("@PesoTotRealGr", SqlDbType.Decimal);
                pReal.Precision = 10; pReal.Scale = 4;
                pReal.Value = qRDataTransferDTO2.PesoRealStr;

                var pObj = cmd.Parameters.Add("@PesoTotObjGr", SqlDbType.Decimal);
                pObj.Precision = 10; pObj.Scale = 4;
                pObj.Value = qRDataTransferDTO2.PesoObjStr;

                cmd.Parameters.Add("@Insumo1Id", SqlDbType.Int).Value = qRDataTransferDTO2.Insumo1Str;
                cmd.Parameters.Add("@Insumo2Id", SqlDbType.Int).Value = qRDataTransferDTO2.Insumo2Str;
                cmd.Parameters.Add("@Insumo3Id", SqlDbType.Int).Value = qRDataTransferDTO2.Insumo3Str;
                cmd.Parameters.Add("@Insumo4Id", SqlDbType.Int).Value = qRDataTransferDTO2.Insumo4Str;

                await cmd.ExecuteNonQueryAsync();

                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public static async Task InsertOrUpdateBatchRegistroDetalle(List<DataTransferQRDTO> qRDataTransferDTOList)
        {
            var builder = new SqlConnectionStringBuilder(_connectionString)
            {
                ConnectTimeout = 5
            };

            await using var cs = new SqlConnection(builder.ConnectionString);

            var sqlBuilder = new StringBuilder();
            sqlBuilder.AppendLine("MERGE dbo.MCR_BatchRegistroDetalle AS target");
            sqlBuilder.AppendLine("USING (VALUES");

            for (int i = 0; i < qRDataTransferDTOList.Count; i++)
            {
                sqlBuilder.Append($"(@BatchRegistroId{i}, @MacroRegistroId{i}, @MacroRegistroFechaCreacion{i}, @MacroRegistroInsumoId{i}, @MacroRegistroPesoReal{i})");
                if (i < qRDataTransferDTOList.Count - 1) sqlBuilder.AppendLine(",");
            }

            sqlBuilder.AppendLine(") AS source (BatchRegistroId, MacroRegistroId, MacroRegistroFechaCreacion, MacroRegistroInsumoId, MacroRegistroPesoReal)");
            sqlBuilder.AppendLine("ON target.BatchRegistroId = source.BatchRegistroId AND target.MacroRegistroId = source.MacroRegistroId AND target.MacroRegistroInsumoId = source.MacroRegistroInsumoId");
            sqlBuilder.AppendLine("WHEN MATCHED THEN UPDATE SET target.MacroRegistroPesoReal = source.MacroRegistroPesoReal");
            sqlBuilder.AppendLine("WHEN NOT MATCHED THEN INSERT (BatchRegistroId, MacroRegistroId, MacroRegistroFechaCreacion, MacroRegistroInsumoId, MacroRegistroPesoReal)");
            sqlBuilder.AppendLine("VALUES (source.BatchRegistroId, source.MacroRegistroId, source.MacroRegistroFechaCreacion, source.MacroRegistroInsumoId, source.MacroRegistroPesoReal);");

            await cs.OpenAsync();
            await using var cmd = new SqlCommand(sqlBuilder.ToString(), cs) { CommandTimeout = 5 };

            for (int i = 0; i < qRDataTransferDTOList.Count; i++)
            {
                cmd.Parameters.AddWithValue($"@BatchRegistroId{i}", 1000001);
                cmd.Parameters.AddWithValue($"@MacroRegistroId{i}", qRDataTransferDTOList[i].MacroRegistroId);
                cmd.Parameters.AddWithValue($"@MacroRegistroFechaCreacion{i}", qRDataTransferDTOList[i].FechaCreacion);
                cmd.Parameters.AddWithValue($"@MacroRegistroInsumoId{i}", qRDataTransferDTOList[i].InsumoId);
                cmd.Parameters.AddWithValue($"@MacroRegistroPesoReal{i}", qRDataTransferDTOList[i].PesoReal);
            }

            await cmd.ExecuteNonQueryAsync();
        }
        
        public static async Task<List<ReportDataGridDTO>> LoadLast20BatchRegistroDetalle()
        {
            var builder = new SqlConnectionStringBuilder(_connectionString)
            {
                ConnectTimeout = 5
            };

            await using var cs = new SqlConnection(builder.ConnectionString);

            const string sql = @"
            SELECT TOP(20)
            brd.BatchRegistroId AS BatchRegistroId,
            r.Codigo AS RecetaCodigo,
            br.Lote AS Lote,
            br.Usuario AS Usuario,
            br.FechaPreparacion AS FechaPreparacion,
            brd.MacroRegistroId AS MacroRegistroId,
            brd.MacroRegistroFechaCreacion AS MacroRegistroFechaCreacion,
            i.Codigo AS InsumoCodigo,
            i.PesoFrima1 AS PesoObjetivo,
            brd.MacroRegistroPesoReal AS PesoReal
            FROM dbo.MCR_BatchRegistro br
            INNER JOIN dbo.MCR_BatchRegistroDetalle brd ON br.BatchRegistroId = brd.BatchRegistroId 
            INNER JOIN dbo.MCR_Insumo i ON brd.MacroRegistroInsumoId = i.InsumoId 
            INNER JOIN dbo.MCR_Receta r ON br.RecetaId = r.RecetaId
            ORDER BY brd.BatchRegistroDetalleId DESC;";

            await cs.OpenAsync();
            await using var cmd = new SqlCommand(sql, cs) { CommandTimeout = 5 };

            using var adapter = new SqlDataAdapter(cmd);
            var dataTable = new DataTable();
            await Task.Run(() => adapter.Fill(dataTable));

            var list = new List<ReportDataGridDTO>();

            foreach (DataRow row in dataTable.Rows)
            {
                list.Add(new ReportDataGridDTO()
                {
                    BatchRegistroId = row.Field<int>("BatchRegistroId"),
                    RecetaCodigo = row.Field<string>("RecetaCodigo") ?? string.Empty,
                    Lote = row.Field<int>("Lote"),
                    Usuario = row.Field<string>("Usuario") ?? string.Empty,
                    FechaPreparacion = row.Field<DateTime>("FechaPreparacion"),
                    MacroRegistroId = row.Field<int>("MacroRegistroId"),
                    MacroRegistroFechaCreacion = row.Field<DateTime>("MacroRegistroFechaCreacion"),
                    InsumoCodigo = row.Field<string>("InsumoCodigo"),
                    PesoObjetivo = row.Field<decimal>("PesoObjetivo") * 1000m,
                    PesoReal = row.Field<decimal>("PesoReal")
                });
            }

            return list;
        }

        // fecha 26 02 2026 para extraer data cargada del ultimopistoleo y verificar las ultimos rgistros.
        public static async Task<List<ReportDataGrid2DTO>> LoadLast20BatchRegistroDetalle2()
        {
            var lista = new List<ReportDataGrid2DTO>();

            var builder = new SqlConnectionStringBuilder(_connectionString)
            {
                ConnectTimeout = 5
            };

            await using var cs = new SqlConnection(builder.ConnectionString);
            await cs.OpenAsync();

            string sql = @"
    SELECT TOP (20)
        BatchRegistroId,
        MacroRegistroId,
        FechaCreacion,
        FechaPistoleo,
        Usuario,
        Lote,
        NombreMacroId,
        PesoTotRealGr,
        PesoTotObjGr,
        Insumo1Id,
        Insumo2Id,
        Insumo3Id,
        Insumo4Id
    FROM dbo.MCR_BatchRegistroBolsas
    ORDER BY BatchRegistroDetalleId DESC;";

            await using var cmd = new SqlCommand(sql, cs);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                int.TryParse(reader["Usuario"]?.ToString(), out int usuarioInt);

                var dto = new ReportDataGrid2DTO
                {
                    BatchRegistroId = reader["BatchRegistroId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["BatchRegistroId"]),
                    MacroRegistroId = reader["MacroRegistroId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["MacroRegistroId"]),
                    UsuarioStr = usuarioInt,

                    Fecha = reader["FechaCreacion"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["FechaCreacion"]),
                    FechaPistoleo = reader["FechaPistoleo"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["FechaPistoleo"]),

                    LoteStr = reader["Lote"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Lote"]),
                    NombreMacroIdStr = reader["NombreMacroId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["NombreMacroId"]),

                    PesoRealStr = reader["PesoTotRealGr"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["PesoTotRealGr"]),
                    PesoObjStr = reader["PesoTotObjGr"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["PesoTotObjGr"]),

                    Insumo1Str = reader["Insumo1Id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Insumo1Id"]),
                    Insumo2Str = reader["Insumo2Id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Insumo2Id"]),
                    Insumo3Str = reader["Insumo3Id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Insumo3Id"]),
                    Insumo4Str = reader["Insumo4Id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Insumo4Id"])
                };

                lista.Add(dto);
            }

            return lista;
        }
    }
}