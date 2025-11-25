using Microsoft.Data.SqlClient;
using RecipeControl.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Services.Database
{
    /// <summary>
    /// Interface for database service
    /// </summary>
    public interface IDatabaseService
    {
        /// <summary>
        /// Retrieves an open SQL database connection.
        /// </summary>
        /// <remarks>The returned <see cref="SqlConnection"/> is ready for use and must be properly
        /// disposed of by the caller          to release database resources. Ensure that the connection is closed when
        /// no longer needed.</remarks>
        /// <returns>An instance of <see cref="SqlConnection"/> representing an open connection to the database.</returns>
        SqlConnection GetConnection();

        /// <summary>
        /// Tests the connection to the underlying service or resource asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the
        /// connection is successful; otherwise, <see langword="false"/>.</returns>
        Task<bool> TestConnectionAsync();

        /// <summary>
        /// Executes the specified SQL command asynchronously and returns the number of rows affected.
        /// </summary>
        /// <param name="sql">The SQL command text to execute. This must be a valid SQL statement.</param>
        /// <param name="parameters">An array of <see cref="SqlParameter"/> objects to be used as parameters for the SQL command. Can be empty if
        /// no parameters are required.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of rows affected by
        /// the command.</returns>
        Task<int> ExecuteCommandAsync(string sql, params SqlParameter[] parameters);

        /// <summary>
        /// Executes the specified SQL query asynchronously and returns the result as a <see cref="DataTable"/>.
        /// </summary>
        /// <param name="sql">The SQL query to execute. This must be a valid SQL statement.</param>
        /// <param name="parameters">An optional array of <see cref="SqlParameter"/> objects to include in the query. These parameters are used
        /// to prevent SQL injection and should match the placeholders in the query.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="DataTable"/> with
        /// the query results. If the query returns no rows, the <see cref="DataTable"/> will be empty.</returns>
        Task<DataTable> ExecuteQueryAsync(string sql, params SqlParameter[] parameters);

        Task<DataTable> ExecuteStoredProcedureAsync(string nameSP, params SqlParameter[] parameters);

        Task<object?> ExecuteScalarAsync(string sql, params SqlParameter[] parameters);

        void UseBackupConnection();

        void UseMainConnection();

        Task<DatabaseInfo> GetDBInformationAsync();
    }

    public class DatabaseInfo
    {
        public string ServerName { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public DateTime ServerDateTime { get; set; }
        public bool SuccesfullConnection { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
