
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReportMicro.Configuration;
using System.Diagnostics;
using System.DirectoryServices;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Windows;

using static ReportMicro.Models.DTOs;


namespace ReportMicro.Services.Database
{
    public static class DatabaseService
    {
        public static readonly string _connectionString = ConfigurationManager.Instance.Settings.ConnectionStrings.DefaultConnection;

        public static async Task<List<ReportDataGridDTO>> BuildReportDataTablePreview(DateTime fechaInicial, DateTime fechaFinal)
        {
            var builder = new SqlConnectionStringBuilder(_connectionString)
            {
                ConnectTimeout = 5
            };

            await using var cs = new SqlConnection(builder.ConnectionString);

            await cs.OpenAsync();

            const string sql = @"
            SELECT
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
            WHERE FechaPreparacion > @FechaInicial AND FechaPreparacion < DATEADD(DAY, 1, @FechaFinal) 
            ORDER BY brd.BatchRegistroDetalleId DESC;";

            await using var cmd = new SqlCommand(sql, cs) { CommandTimeout = 5 };
            cmd.Parameters.Add("@FechaInicial", SqlDbType.DateTime).Value = fechaInicial;
            cmd.Parameters.Add("@FechaFinal", SqlDbType.DateTime).Value = fechaFinal;

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

        public static async Task<List<ReportDataGrid2DTO>> BuildReportDataTable2Preview(DateTime fechaInicial, DateTime fechaFinal)
        {
            // Ajustar rango para incluir todo el día
            fechaInicial = fechaInicial.Date;
            fechaFinal = fechaFinal.Date.AddDays(1).AddTicks(-1);

            var lista = new List<ReportDataGrid2DTO>();

            string query = @"
    SELECT  
        b.BatchRegistroId,
        b.MacroRegistroId,

        b.FechaCreacion      AS Fecha,          
        b.FechaPistoleo      AS FechaPistoleo,

        b.Usuario            AS UsuarioStr,
        b.Lote               AS LoteStr,

        b.PesoTotObjGr       AS PesoObj,
        b.PesoTotRealGr      AS PesoReal,

        b.Insumo1Id          AS Insumo1Strid,
        i1.Codigo            AS Insumo1StrSAP,
        i1.Descripcion       AS Insumo1StrDescr,

        b.Insumo2Id          AS Insumo2Strid,
        i2.Codigo            AS Insumo2StrSAP,
        i2.Descripcion       AS Insumo2StrDescr,

        b.Insumo3Id          AS Insumo3Strid,
        i3.Codigo            AS Insumo3StrSAP,
        i3.Descripcion       AS Insumo3StrDescr,

        b.Insumo4Id          AS Insumo4Strid,
        i4.Codigo            AS Insumo4StrSAP,
        i4.Descripcion       AS Insumo4StrDescr

    FROM dbo.MCR_BatchRegistroBolsas b

    LEFT JOIN dbo.MCR_Insumo i1 ON b.Insumo1Id = i1.InsumoId
    LEFT JOIN dbo.MCR_Insumo i2 ON b.Insumo2Id = i2.InsumoId
    LEFT JOIN dbo.MCR_Insumo i3 ON b.Insumo3Id = i3.InsumoId
    LEFT JOIN dbo.MCR_Insumo i4 ON b.Insumo4Id = i4.InsumoId

    WHERE 
        b.FechaPistoleo IS NOT NULL
        AND b.FechaPistoleo BETWEEN @FechaInicial AND @FechaFinal

    ORDER BY b.BatchRegistroDetalleId DESC;
    ";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.CommandType = CommandType.Text;

                command.Parameters.Add("@FechaInicial", SqlDbType.DateTime).Value = fechaInicial;
                command.Parameters.Add("@FechaFinal", SqlDbType.DateTime).Value = fechaFinal;

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var dto = new ReportDataGrid2DTO
                        {
                            BatchRegistroId = reader["BatchRegistroId"] != DBNull.Value ? Convert.ToInt32(reader["BatchRegistroId"]) : 0,
                            MacroRegistroId = reader["MacroRegistroId"] != DBNull.Value ? Convert.ToInt32(reader["MacroRegistroId"]) : 0,

                            Fecha = reader["Fecha"] != DBNull.Value ? Convert.ToDateTime(reader["Fecha"]) : DateTime.MinValue,
                            FechaPistoleo = reader["FechaPistoleo"] != DBNull.Value ? Convert.ToDateTime(reader["FechaPistoleo"]) : (DateTime?)null,

                            UsuarioStr = reader["UsuarioStr"]?.ToString() ?? string.Empty,
                            LoteStr = reader["LoteStr"]?.ToString() ?? string.Empty,

                            PesoObj = reader["PesoObj"] != DBNull.Value ? Convert.ToDecimal(reader["PesoObj"]) : 0m,
                            PesoReal = reader["PesoReal"] != DBNull.Value ? Convert.ToDecimal(reader["PesoReal"]) : 0m,

                            Insumo1Strid = reader["Insumo1Strid"]?.ToString() ?? string.Empty,
                            Insumo1StrSAP = reader["Insumo1StrSAP"]?.ToString() ?? string.Empty,
                            Insumo1StrDescr = reader["Insumo1StrDescr"]?.ToString() ?? string.Empty,

                            Insumo2Strid = reader["Insumo2Strid"]?.ToString() ?? string.Empty,
                            Insumo2StrSAP = reader["Insumo2StrSAP"]?.ToString() ?? string.Empty,
                            Insumo2StrDescr = reader["Insumo2StrDescr"]?.ToString() ?? string.Empty,

                            Insumo3Strid = reader["Insumo3Strid"]?.ToString() ?? string.Empty,
                            Insumo3StrSAP = reader["Insumo3StrSAP"]?.ToString() ?? string.Empty,
                            Insumo3StrDescr = reader["Insumo3StrDescr"]?.ToString() ?? string.Empty,

                            Insumo4Strid = reader["Insumo4Strid"]?.ToString() ?? string.Empty,
                            Insumo4StrSAP = reader["Insumo4StrSAP"]?.ToString() ?? string.Empty,
                            Insumo4StrDescr = reader["Insumo4StrDescr"]?.ToString() ?? string.Empty
                        };

                        lista.Add(dto);
                    }
                }
            }

            return lista;
        }
    }
}