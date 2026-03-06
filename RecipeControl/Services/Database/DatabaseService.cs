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
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using static RecipeControl.Models.DTOs;

namespace RecipeControl.Services.Database
{
    /// <summary>
    /// Servicio para la gestión de consultas a la base de datos de pesajes, toda nueva implementación que requiera acceso a base
    /// de datos debe implementarse aquí por orden y coherencia
    /// </summary>
    public static class DatabaseService
    {
        /// <summary>
        /// String de conexión a la base de datos para la ejecución de consultas, está registrado en el archivo appsettings.json y
        /// puede ser cambiado desde el archivo por el explorador de windows
        /// </summary>
        public static readonly string _connectionString = ConfigurationManager.Instance.Settings.ConnectionStrings.DefaultConnection;

        /// <summary>
        /// Extrae listado de insumos de la base de datos en función al Id del tipo de insumo (e.g. 1001: SOLIDOS)
        /// </summary>
        /// <param name="tipoInsumoId"></param>
        /// <returns></returns>
        public static async Task<List<ComboBoxDTO>> GetInsumosByTipo(int tipoInsumoId)
        {
            var builder = new SqlConnectionStringBuilder(_connectionString)
            {
                ConnectTimeout = 5
            };

            await using var cs = new SqlConnection(builder.ConnectionString);
            await cs.OpenAsync();

            const string sql = @"
        SELECT InsumoId, Codigo, PesoFrima1
        FROM dbo.MCR_Insumo
        WHERE TipoInsumoId = @TipoInsumoId
        ORDER BY Codigo;";

            await using var cmd = new SqlCommand(sql, cs) { CommandTimeout = 5 };
            cmd.Parameters.AddWithValue("@TipoInsumoId", tipoInsumoId);

            using var adapter = new SqlDataAdapter(cmd);
            var dataTable = new DataTable();
            await Task.Run(() => adapter.Fill(dataTable));

            var list = new List<ComboBoxDTO>();

            foreach (DataRow dataRow in dataTable.Rows)
            {
                list.Add(new ComboBoxDTO()
                {
                    Id = Convert.ToInt32(dataRow["InsumoId"]),
                    Codigo = dataRow["Codigo"]?.ToString() ?? "",
                    PesoFrima1 = dataRow["PesoFrima1"] == DBNull.Value
                 ? 0m
                 : Convert.ToDecimal(dataRow["PesoFrima1"])
                });
            }

            // Opción por defecto
            list.Insert(0, new ComboBoxDTO()
            {
                Id = 1000,
                Codigo = "-- Ninguno --",
                PesoFrima1 = 0m
            });

            return list;
        }

        /// <summary>
        /// Inserta un MacroRegistro de una nueva bolsa vacía, ingresando la fecha y hora de creación y 
        /// retornando el Id generado para la impresión de QRs preliminares
        /// </summary>
        /// <param name="fechaCreacion"></param>
        /// <returns></returns>
        public static async Task<int> InsertMacroAndReturnIdAsync(DateTime fechaCreacion)
        {
            var builder = new SqlConnectionStringBuilder(_connectionString)
            {
                ConnectTimeout = 5
            };

            await using var cs = new SqlConnection(builder.ConnectionString);

            const string sql = @"
            INSERT INTO dbo.MCR_MacroRegistro
            (Usuario, FechaCreacion)
            OUTPUT INSERTED.MacroRegistroId
            VALUES
            (@Usuario, @FechaCreacion);
            ";

            await cs.OpenAsync();
            await using var cmd = new SqlCommand(sql, cs) { CommandTimeout = 5 };
            cmd.Parameters.AddWithValue("@Usuario", "Operador");                    // **** Por definir luego de admnistración de usuarios
            cmd.Parameters.AddWithValue("@FechaCreacion", fechaCreacion);

            object? result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// Ingresa el valor del peso de un determinado insumo al registro de la bolsa determinada por
        /// el QR disparado, en caso de que el insumo ya haya existido, se actualiza el valor del pesado.
        /// </summary>
        /// <param name="macroRegistroId">Registro de la bolsa impreso en el QR</param>
        /// <param name="insumoId">Codigo del inusmo en formato 100X, en la otra PC debe haber otra tabla 
        /// para su restauración</param>
        /// <param name="insumoLote">Lote del insumo, solo se guarda, no se envía por QR</param>
        /// <param name="pesoObjetivo">Solo se guarda</param>
        /// <param name="pesoReal">Peso leído desde la balanza con 1 decimal de gramos</param>
        /// <returns></returns>
        public static async Task InsertOrUpdateMicroAsync(int macroRegistroId, int insumoId, int insumoLote, decimal pesoObjetivo, decimal pesoReal)
        {
            var builder = new SqlConnectionStringBuilder(_connectionString)
            {
                ConnectTimeout = 5
            };

            await using var cs = new SqlConnection(builder.ConnectionString);

            const string sql = @"
            IF EXISTS (
                SELECT 1 
                FROM dbo.MCR_MacroRegistroDetalle
                WHERE MacroRegistroId = @MacroRegistroId
                  AND InsumoId = @InsumoId
            )
            BEGIN
                UPDATE dbo.MCR_MacroRegistroDetalle
                SET PesoReal = @PesoReal
                WHERE MacroRegistroId = @MacroRegistroId
                  AND InsumoId = @InsumoId
            END
            ELSE
            BEGIN
                INSERT INTO dbo.MCR_MacroRegistroDetalle
                (MacroRegistroId, InsumoId, InsumoLote, PesoObjetivo, PesoReal)
                VALUES
                (@MacroRegistroId, @InsumoId, @InsumoLote, @PesoObjetivo, @PesoReal);
            END
            ";

            await cs.OpenAsync();
            await using var cmd = new SqlCommand(sql, cs) { CommandTimeout = 5 };

            cmd.Parameters.AddWithValue("@MacroRegistroId", macroRegistroId);
            cmd.Parameters.AddWithValue("@InsumoId", insumoId);
            cmd.Parameters.AddWithValue("@InsumoLote", insumoLote);
            cmd.Parameters.AddWithValue("@PesoObjetivo", pesoObjetivo);
            cmd.Parameters.AddWithValue("@PesoReal", pesoReal);

            await cmd.ExecuteScalarAsync();
        }

        // Fecha: 25 02 2026
        // inserta o actualiza datos de pantalla y datos pesado a DB tabla MCR_MacroRegistro
        public static async Task InsertOrUpdateMacroTotalAsync(int macroRegistroId, string usuario, DateTime fechaCreacion, int insumoLote, int NombreMacroId, decimal PesoTotRealGr, decimal PesoTotObjGr, int insumo1Id, int insumo2Id, int insumo3Id, int insumo4Id)
        {
            var builder = new SqlConnectionStringBuilder(_connectionString)
            {
                ConnectTimeout = 5
            };

            await using var cs = new SqlConnection(builder.ConnectionString);

            const string sql = @"
    IF EXISTS (SELECT 1 FROM dbo.MCR_MacroRegistro WHERE MacroRegistroId = @MacroRegistroId)
    BEGIN
        UPDATE dbo.MCR_MacroRegistro
        SET Usuario = @Usuario,
            FechaCreacion = @FechaCreacion,
            Lote = @InsumoLote,
            NombreMacroId = @NombreMacroId,
            PesoTotRealGr = @PesoTotRealGr,
            PesoTotObjGr = @PesoTotObjGr,
            Insumo1Id = @Insumo1Id,
            Insumo2Id = @Insumo2Id,
            Insumo3Id = @Insumo3Id,
            Insumo4Id = @Insumo4Id
        WHERE MacroRegistroId = @MacroRegistroId;
    END
    ELSE
    BEGIN
        INSERT INTO dbo.MCR_MacroRegistro
        (
            MacroRegistroId, Usuario, FechaCreacion, Lote, NombreMacroId,
            PesoTotRealGr, PesoTotObjGr,
            Insumo1Id, Insumo2Id, Insumo3Id, Insumo4Id
        )
        VALUES
        (
            @MacroRegistroId, @Usuario, @FechaCreacion, @InsumoLote, @NombreMacroId,
            @PesoTotRealGr, @PesoTotObjGr,
            @Insumo1Id, @Insumo2Id, @Insumo3Id, @Insumo4Id
        );
    END";

            await cs.OpenAsync();
            await using var cmd = new SqlCommand(sql, cs) { CommandTimeout = 5 };

            cmd.Parameters.Add("@MacroRegistroId", SqlDbType.Int).Value = macroRegistroId;
            cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 20).Value = usuario;
            cmd.Parameters.Add("@FechaCreacion", SqlDbType.DateTime).Value = fechaCreacion;
            cmd.Parameters.Add("@InsumoLote", SqlDbType.Int).Value = insumoLote;
            cmd.Parameters.Add("@NombreMacroId", SqlDbType.Int).Value = NombreMacroId;

            var pReal = cmd.Parameters.Add("@PesoTotRealGr", SqlDbType.Decimal);
            pReal.Precision = 10;
            pReal.Scale = 4;
            pReal.Value = PesoTotRealGr;

            var pObj = cmd.Parameters.Add("@PesoTotObjGr", SqlDbType.Decimal);
            pObj.Precision = 10;
            pObj.Scale = 4;
            pObj.Value = PesoTotObjGr;

            cmd.Parameters.Add("@Insumo1Id", SqlDbType.Int).Value = insumo1Id;
            cmd.Parameters.Add("@Insumo2Id", SqlDbType.Int).Value = insumo2Id;
            cmd.Parameters.Add("@Insumo3Id", SqlDbType.Int).Value = insumo3Id;
            cmd.Parameters.Add("@Insumo4Id", SqlDbType.Int).Value = insumo4Id;

            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Método para extraer el peso objetiov en función del insumo, actualmente todo referencia
        /// a frima, así que no hay filtro por área solo por insumo
        /// </summary>
        /// <param name="insumoId"></param>
        /// <returns></returns>
        public static async Task<decimal> GetPesoObjetivoAsync(int insumoId)
        {
            var builder = new SqlConnectionStringBuilder(_connectionString)
            {
                ConnectTimeout = 5
            };

            await using var cs = new SqlConnection(builder.ConnectionString);

            const string sql = @"
            SELECT PesoFrima1 FROM dbo.MCR_Insumo 
            WHERE InsumoId = @InsumoId;
            ";

            await cs.OpenAsync();
            await using var cmd = new SqlCommand(sql, cs) { CommandTimeout = 5 };
            cmd.Parameters.AddWithValue("@InsumoId", insumoId);

            object? result = await cmd.ExecuteScalarAsync();

            return result == DBNull.Value ? -1m : (decimal)Convert.ToDecimal(result);
        }

        /// <summary>
        /// Extrae la información total de una bolsa, con las referencias a otras tablas para el armado del QR
        /// </summary>
        /// <param name="macroRegistroId"></param>
        /// <returns></returns>
        public static async Task<List<DataTransferQRDTO>> GetCompiladoPesosAsync(int macroRegistroId)
        {
            var builder = new SqlConnectionStringBuilder(_connectionString)
            {
                ConnectTimeout = 5
            };

            await using var cs = new SqlConnection(builder.ConnectionString);

            const string sql = @"
            SELECT mr.MacroRegistroId, mr.FechaCreacion, mr.Usuario, mrd.InsumoId, i.Codigo, mrd.PesoReal 
            FROM dbo.MCR_MacroRegistroDetalle mrd 
            INNER JOIN MCR_MacroRegistro mr ON mrd.MacroRegistroId = mr.MacroRegistroId 
            INNER JOIN MCR_Insumo i ON mrd.InsumoId = i.InsumoId 
            WHERE mrd.MacroRegistroId = @MacroRegistroId;
            ";

            await cs.OpenAsync();
            await using var cmd = new SqlCommand(sql, cs) { CommandTimeout = 5 };
            cmd.Parameters.AddWithValue("@MacroRegistroId", macroRegistroId);

            using var adapter = new SqlDataAdapter(cmd);
            var dataTable = new DataTable();
            await Task.Run(() => adapter.Fill(dataTable));

            var list = new List<DataTransferQRDTO>();

            foreach (DataRow dataRow in dataTable.Rows)
            {
                list.Add(new DataTransferQRDTO()
                {
                    MacroRegistroId = Convert.ToInt32(dataRow["MacroRegistroId"]),
                    FechaCreacion = Convert.ToDateTime(dataRow["FechaCreacion"]),
                    Usuario = dataRow["Usuario"].ToString() ?? "",
                    InsumoId = Convert.ToInt32(dataRow["InsumoId"]),
                    Codigo = dataRow["Codigo"].ToString() ?? "",
                    PesoReal = Convert.ToDecimal(dataRow["PesoReal"])
                });
            }

            return list;
        }

        // fecha: 25 02 2026.
        // obtiene compiladod e maroingredientes nueva version final acordada.
        public static async Task<List<DataTransfer2QRDTO>> GetCompiladoMacroIngrAsync(int macroRegistroId)
        {
            var builder = new SqlConnectionStringBuilder(_connectionString)
            {
                ConnectTimeout = 5
            };

            await using var cs = new SqlConnection(builder.ConnectionString);

            const string sql = @"
        SELECT
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
        FROM dbo.MCR_MacroRegistro
        WHERE MacroRegistroId = @MacroRegistroId;
    ";

            await cs.OpenAsync();

            await using var cmd = new SqlCommand(sql, cs) { CommandTimeout = 5 };
            cmd.Parameters.Add("@MacroRegistroId", SqlDbType.Int).Value = macroRegistroId;

            var list = new List<DataTransfer2QRDTO>();

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var dto = new DataTransfer2QRDTO
                {
                    MacroRegistroId = macroRegistroId,
                    Usuario = reader["Usuario"]?.ToString() ?? string.Empty,
                    FechaCreacion = reader["FechaCreacion"] == DBNull.Value
                        ? DateTime.MinValue
                        : Convert.ToDateTime(reader["FechaCreacion"]),

                    // Si Lote fuera nullable en DB, aquí puedes dejar 0 por defecto
                    Lote = reader["Lote"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Lote"]),
                    NombreMacroId = reader["NombreMacroId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["NombreMacroId"]),

                    PesoTotRealGr = reader["PesoTotRealGr"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["PesoTotRealGr"]),
                    PesoTotObjGr = reader["PesoTotObjGr"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["PesoTotObjGr"]),

                    Insumo1Id = reader["Insumo1Id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Insumo1Id"]),
                    Insumo2Id = reader["Insumo2Id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Insumo2Id"]),
                    Insumo3Id = reader["Insumo3Id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Insumo3Id"]),
                    Insumo4Id = reader["Insumo4Id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Insumo4Id"]),
                };
                
                list.Add(dto);
            }

            return list;
        }

        /// <summary>
        /// Función de prueba para validar la escritura del QR compilado en la base da datos, este método NO se usa
        /// aquí, sino en la aplicación que va a correr en el SCADA
        /// </summary>
        /// <param name="qRDataTransferDTOList"></param>
        /// <returns></returns>
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
                cmd.Parameters.AddWithValue($"@BatchRegistroId{i}", 1001);
                cmd.Parameters.AddWithValue($"@MacroRegistroId{i}", qRDataTransferDTOList[i].MacroRegistroId);
                cmd.Parameters.AddWithValue($"@MacroRegistroFechaCreacion{i}", qRDataTransferDTOList[i].FechaCreacion);
                cmd.Parameters.AddWithValue($"@MacroRegistroInsumoId{i}", qRDataTransferDTOList[i].InsumoId);
                cmd.Parameters.AddWithValue($"@MacroRegistroPesoReal{i}", qRDataTransferDTOList[i].PesoReal);
            }

            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task<List<InsumoPesoDTO>> LoadInsumosPesoAsync()
        {
            var list = new List<InsumoPesoDTO>();

            const string sql = @"
            SELECT
                InsumoId,
                Codigo,
                Descripcion,
                Unidad,
                PesoFrima1
            FROM dbo.MCR_Insumo
            WHERE TipoInsumoId = 1001
            ORDER BY InsumoId; ";

            await using var cn = new SqlConnection(_connectionString);
            await cn.OpenAsync();

            await using var cmd = new SqlCommand(sql, cn) { CommandTimeout = 10 };
            await using var rd = await cmd.ExecuteReaderAsync();

            while (await rd.ReadAsync())
            {
                list.Add(new InsumoPesoDTO
                {
                    InsumoId = rd.GetInt32(rd.GetOrdinal("InsumoId")),
                    Codigo = rd["Codigo"]?.ToString() ?? "",
                    Descripcion = rd["Descripcion"]?.ToString() ?? "",
                    Unidad = rd["Unidad"]?.ToString() ?? "",
                    PesoFrima1 = rd["PesoFrima1"] == DBNull.Value ? 0m : (decimal)rd["PesoFrima1"]
                });
            }

            return list;
        }

        public static async Task UpdatePesoFrima1Async(IEnumerable<InsumoPesoDTO> insumos)
        {
            const string sql = @"
UPDATE dbo.MCR_Insumo
SET PesoFrima1 = @PesoFrima1,
    FechaModificacion = GETDATE()
WHERE InsumoId = @InsumoId;";

            await using var cn = new SqlConnection(_connectionString);
            await cn.OpenAsync();
            await using var tx = await cn.BeginTransactionAsync();

            try
            {
                await using var cmd = new SqlCommand(sql, cn, (SqlTransaction)tx) { CommandTimeout = 10 };

                var pId = cmd.Parameters.Add("@InsumoId", SqlDbType.Int);
                var pPeso = cmd.Parameters.Add("@PesoFrima1", SqlDbType.Decimal);
                pPeso.Precision = 7;  // decimal(7,4)
                pPeso.Scale = 4;

                foreach (var i in insumos)
                {
                    pId.Value = i.InsumoId;
                    pPeso.Value = i.PesoFrima1;

                    await cmd.ExecuteNonQueryAsync();
                }

                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
    }
}