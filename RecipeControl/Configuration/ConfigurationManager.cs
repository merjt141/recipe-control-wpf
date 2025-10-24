using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Configuration
{
    /// <summary>
    /// Centralized application configuration manager
    /// </summary>
    public class ConfigurationManager
    {
        private static ConfigurationManager? _instance;
        private static readonly object _lock = new();
        private readonly IConfiguration _configuration;
        private readonly AppSettings _appSettings;

        private ConfigurationManager()
        {
            // Build configuration from appsettings.json
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{GetEnvironment()}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();

            // Mapping to class AppSettings
            _appSettings = new AppSettings();
            _configuration.Bind(_appSettings);
        }

        /// <summary>
        /// Retrieves the singleton instance of the ConfigurationManager
        /// </summary>
        public static ConfigurationManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new ConfigurationManager();
                    }
                }
                return _instance;
            }
        }

        public AppSettings Settings => _appSettings;
        public IConfiguration Configuration => _configuration;

        /// <summary>
        /// Retrieves specific configuration value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string GetValue(string key, string defaultValue = "")
        {
            return _configuration[key] ?? defaultValue;
        }

        /// <summary>
        /// Retrieves a configuration section
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public T GetSection<T>(string sectionName) where T : new()
        {
            var section = new T();
            _configuration.GetSection(sectionName).Bind(section);
            return section;
        }

        /// <summary>
        /// Determines execution environment (Production, Development)
        /// </summary>
        /// <returns></returns>
        public static string GetEnvironment()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                ?? "Production";
        }

        /// <summary>
        /// Validates that the configuration is correct
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool ValidateConfiguration(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(_appSettings.ConnectionStrings.DefaultConnection))
            {
                errorMessage = "No se ha configurado la cadena de conexión a la base de datos.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(_appSettings.SerialPort.PortName))
            {
                errorMessage = "No se ha configurado el puerto serial.";
                return false;
            }

            // Luego implementar Ruta Exportación

            return true;
        }

        /// <summary>
        /// Reload configuration from files
        /// </summary>
        public void ReloadConfiguration()
        {
            lock (_lock)
            {
                _instance = new ConfigurationManager();
            }
        }
    }
}
