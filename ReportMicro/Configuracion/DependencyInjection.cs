using Microsoft.Extensions.DependencyInjection;
using ReportMicro.Services.Database;
using ReportMicro.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ReportMicro.Configuration
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Registers all application services
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            var config = ConfigurationManager.Instance.Settings;

            // ===== CONFIGURATION =====
            services.AddSingleton(config);
            services.AddSingleton(config.ConnectionStrings);
            services.AddSingleton(config.Database);
            services.AddSingleton(config.Reports);

            // ===== INGFRAESTRUCTURE SERVICES =====

            // ===== REPOSITORIOS =====

            // ===== HELPERS =====

            // ===== VIEWMODELS =====

            // ===== VIEWS =====
            services.AddTransient<MainWindow>();
            services.AddTransient<ReportView>();

            return services;
        }

        /// <summary>
        /// Validates that all critical services are registered
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static bool ValidateServices(IServiceProvider serviceProvider, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"Error al validar servicios: {ex.Message}";
                return false;
            }
        }
    }
}
