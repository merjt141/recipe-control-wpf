using Microsoft.Extensions.DependencyInjection;
using RecipeControl.Services.Serial;
using RecipeControl.Views;
using RecipeControl.Views.RegisterModuleViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Configuration
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
            services.AddSingleton(config.SerialPortQR);
            services.AddSingleton(config.Database);

            // ===== INGFRAESTRUCTURE SERVICES =====

            // ===== REPOSITORIOS =====

            // ===== HELPERS =====

            // ===== VIEWMODELS =====


            // ===== VIEWS =====
            services.AddTransient<MainWindow>();

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
