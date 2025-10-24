using Microsoft.Data.SqlClient;
using RecipeControl.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Services.Interfaces
{
    /// <summary>
    /// Interface for database service
    /// </summary>
    public interface IDatabaseService
    {
        SqlConnection GetConnection();

        Task<bool> TestConnectionAsync();

        Task<int> ExecuteCommandAsync(string sql, params SqlParameter[] parameters);

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
