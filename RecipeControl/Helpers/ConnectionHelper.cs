using RecipeControl.Helpers.Interfaces;
using RecipeControl.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Helpers
{
    public class ConnectionHelper : IConnectionHelper
    {
        private readonly IDatabaseService _databaseService;
        private readonly ISerialService _serialService;

        public ConnectionHelper(IDatabaseService databaseService, ISerialService serialService)
        {
            _databaseService = databaseService;
            _serialService = serialService;
        }

        public async Task<ConnectionTestResult> TestDBConnectionAsync()
        {
            var result = new ConnectionTestResult();
            var start = DateTime.Now;

            try
            {
                var info = await _databaseService.GetDBInformationAsync();
                var end = DateTime.Now;

                result.TiempoRespuesta = end - start;
                result.InformacionBD = info;
                result.Exitoso = info.SuccesfullConnection;

                if (info.SuccesfullConnection)
                {
                    result.Mensaje = $"✓ Conexión exitosa a '{info.DatabaseName}' en '{info.ServerName}'\n" +
                                      $"  Versión: {info.Version}\n" +
                                      $"  Tiempo de respuesta: {result.TiempoRespuesta.TotalMilliseconds:F0} ms\n" +
                                      $"  Fecha servidor: {info.ServerDateTime:dd/MM/yyyy HH:mm:ss}";
                }
                else
                {
                    result.Mensaje = $"✗ Error de conexión: {info.ErrorMessage}";
                }
            }
            catch (Exception ex)
            {
                result.Exitoso = false;
                result.Mensaje = $"✗ Error al probar conexión: {ex.Message}";
                result.Excepcion = ex;
            }

            return result;
        }

        public async Task<ConnectionTestResult> TestSerialConnectionAsync()
        {
            var result = new ConnectionTestResult();
            var start = DateTime.Now;

            await Task.Delay(500);

            result.Exitoso = true;
            result.Mensaje = "Método no implementado";

            return result;
        }
        public async Task<bool> CheckTablesExistanceAsync()
        {
            // Método no implementado
            await Task.Delay(500);
            return true;
        }
    }
}
