using RecipeControl.Services.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Helpers.Interfaces
{
    public class ConnectionTestResult
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public TimeSpan TiempoRespuesta { get; set; }
        public DatabaseInfo? InformacionBD { get; set; }
        public Exception? Excepcion { get; set; }
    }

    public interface IConnectionHelper
    {
        Task<ConnectionTestResult> TestDBConnectionAsync();
        Task<ConnectionTestResult> TestSerialConnectionAsync();
        Task<bool> CheckTablesExistanceAsync();
    }
}
