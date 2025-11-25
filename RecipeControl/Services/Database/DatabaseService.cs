using Microsoft.Data.SqlClient;
using RecipeControl.Configuration;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Windows;

namespace RecipeControl.Services.Database
{
    /// <summary>
    /// Database service using ADO.NET
    /// </summary>
    public class DatabaseService : IDatabaseService
    {
        private readonly ConnectionStrings _connectionStrings;
        private readonly DatabaseSettings _databaseSettings;
        private string _connectionString;

        public DatabaseService(ConnectionStrings connectionStrings, DatabaseSettings databaseSettings)
        {
            _connectionStrings = connectionStrings;
            _databaseSettings = databaseSettings;
            _connectionString = _connectionStrings.DefaultConnection;
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using var connection = GetConnection();
                await connection.OpenAsync();
                return connection.State == ConnectionState.Open;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> ExecuteCommandAsync(string sql, params SqlParameter[] parameters)
        {
            int retries = 0;
            Exception? lastException = null;

            while (retries < _databaseSettings.MaxRetryCount)
            {
                try
                {
                    using var connection = GetConnection();
                    await connection.OpenAsync();

                    using var command = new SqlCommand(sql, connection)
                    {
                        CommandTimeout = _databaseSettings.CommandTimeout,
                        CommandType = CommandType.Text,
                    };

                    if (parameters != null && parameters.Length > 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    return await command.ExecuteNonQueryAsync();
                }
                catch (SqlException ex)
                {
                    lastException = ex;
                    retries++;

                    Debug.WriteLine($"Retry number {retries}");

                    if (retries >= _databaseSettings.MaxRetryCount)
                    {
                        await Task.Delay(_databaseSettings.RetryDelay);
                    }
                }
            }

            throw new Exception(
                $"Error al ejecutar el comando despues de {_databaseSettings.MaxRetryCount} intentos",
                lastException);
        }

        /// <summary>
        /// Executes the specified SQL query asynchronously and returns the result as a <see cref="DataTable"/>.
        /// </summary>
        /// <remarks>This method retries the query execution up to the maximum number of retries specified
        /// in the database settings if a <see cref="SqlException"/> occurs. The retry delay and maximum retry count are
        /// also determined by the database settings.</remarks>
        /// <param name="sql">The SQL query to execute. This must be a valid SQL statement.</param>
        /// <param name="parameters">An optional array of <see cref="SqlParameter"/> objects to include with the query. If <see langword="null"/>
        /// or empty, no parameters are added.</param>
        /// <returns>A <see cref="DataTable"/> containing the results of the query. The table will be empty if the query does not
        /// return any rows.</returns>
        /// <exception cref="Exception">Thrown if the query fails after the maximum number of retry attempts.</exception>
        public async Task<DataTable> ExecuteQueryAsync(string sql, SqlParameter[]? parameters)
        {
            int retries = 0;
            Exception? lastException = null;

            // Retry logic
            while (retries < _databaseSettings.MaxRetryCount)
            {
                try
                {
                    // Create and open connection
                    using var connection = GetConnection();

                    // Open connection
                    await connection.OpenAsync();

                    // Create command
                    using var command = new SqlCommand(sql, connection)
                    {
                        CommandTimeout = _databaseSettings.CommandTimeout,
                        CommandType = CommandType.Text,
                    };

                    // Add parameters if provided
                    if (parameters != null && parameters.Length > 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    // Execute query and fill DataTable
                    using var adapter = new SqlDataAdapter(command);
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    return dataTable;
                }
                catch (SqlException ex)
                {
                    // Log exception and increment retry count
                    lastException = ex;
                    retries++;

                    Debug.WriteLine($"Retry number {retries}");

                    // Wait before retrying
                    if (retries >= _databaseSettings.MaxRetryCount)
                    {
                        await Task.Delay(_databaseSettings.RetryDelay);
                    }
                }
            }

            MessageBox.Show("Falla de conexión a base de datos");

            // Throw exception after max retries
            throw new Exception(
                $"Error al ejecutar la consulta despues de {_databaseSettings.MaxRetryCount} intentos",
                lastException);
        }

        public async Task<DataTable> ExecuteStoredProcedureAsync(string nameSP, SqlParameter[] parameters)
        {
            try
            {
                using var connection = GetConnection();
                await connection.OpenAsync();

                using var command = new SqlCommand(nameSP, connection)
                {
                    CommandTimeout = _databaseSettings.CommandTimeout,
                    CommandType = CommandType.StoredProcedure
                };

                if (parameters != null && parameters.Length > 0)
                {
                    command.Parameters.AddRange(parameters);
                }

                using var adapter = new SqlDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);

                return dataTable;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al ejecutar el procedimiento almacenado {nameSP}: {ex.Message}",
                    ex);
            }
        }

        public async Task<object?> ExecuteScalarAsync(string slq, params SqlParameter[] parameters)
        {
            try
            {
                using var connection = GetConnection();
                await connection.OpenAsync();

                using var command = new SqlCommand(slq, connection)
                {
                    CommandTimeout = _databaseSettings.CommandTimeout,
                    CommandType = CommandType.Text
                };

                if (parameters != null && parameters.Length > 0)
                {
                    command.Parameters.AddRange(parameters);
                }

                return await command.ExecuteScalarAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al ejecutar escalar: {ex.Message}", ex);
            }
        }

        public async Task<DatabaseInfo> GetDBInformationAsync()
        {
            var info = new DatabaseInfo();

            try
            {
                using var connection = GetConnection();
                await connection.OpenAsync();

                info.SuccesfullConnection = true;
                info.ServerName = connection.DataSource;
                info.DatabaseName = connection.Database;

                // Retrieve SQL Server version
                using var command = new SqlCommand("SELECT @@VERSION", connection);
                var version = await command.ExecuteScalarAsync();
                info.Version = version?.ToString()?.Split('\n')[0] ?? "Desconocido";

                // Retrieve server datetime
                using var commandDate = new SqlCommand("SELECT GETDATE()", connection);
                var date = await commandDate.ExecuteScalarAsync();
                info.ServerDateTime = date != null ? Convert.ToDateTime(date) : DateTime.Now;
            }
            catch (Exception ex)
            {
                info.SuccesfullConnection = false;
                info.ErrorMessage = ex.Message;
            }

            return info;
        }

        public void UseBackupConnection()
        {
            if (!string.IsNullOrEmpty(_connectionStrings.BackupConnection))
            {
                _connectionString = _connectionStrings.BackupConnection;
            }
        }

        public void UseMainConnection()
        {
            _connectionString = _connectionStrings.DefaultConnection;
        }
    }
}
