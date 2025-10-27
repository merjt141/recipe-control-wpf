using Microsoft.Extensions.DependencyInjection;
using RecipeControl.Helpers;
using RecipeControl.Helpers.Interfaces;
using RecipeControl.Models.Config;
using RecipeControl.Repositories;
using RecipeControl.Repositories.Interfaces;
using RecipeControl.Services.Database;
using RecipeControl.Services.Serial;
using RecipeControl.ViewModels;
using RecipeControl.ViewModels.RegisterModule;
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
            services.AddSingleton(config.ScaleEthernetPorts);
            services.AddSingleton(config.ScaleEthernetPorts.Ports);
            services.AddSingleton(config.SerialPort);
            services.AddSingleton(config.Database);

            // ===== INGFRAESTRUCTURE SERVICES =====
            services.AddSingleton<ISerialService, SerialService>();
            services.AddSingleton<IDatabaseService, DatabaseService>();

            // ===== REPOSITORIOS =====
            services.AddScoped<ITipoInsumoRepository, TipoInsumoRepository>();
            services.AddScoped<IRegistroPesoRepository, RegistroPesoRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IInsumoRepository, InsumoRepository>();
            services.AddScoped<IRecetaVersionRepository, RecetaVersionRepository>();

            // ===== HELPERS =====
            services.AddTransient<IConnectionHelper, ConnectionHelper>();

            // ===== VIEWMODELS =====
            services.AddTransient<MainViewModel>();
            services.AddTransient<RegisterModuleViewModel>();


            // ===== VIEWS =====
            services.AddTransient<MainWindow>();
            services.AddTransient<RegisterModuleView>();
            services.AddTransient<TestConnectionView>();

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
                if (serviceProvider.GetService<IDatabaseService>() is null)
                {
                    errorMessage = $"{nameof(IDatabaseService)} no está registrado.";
                    return false;
                }

                if (serviceProvider.GetService<ISerialService>() is null)
                {
                    errorMessage = $"{nameof(ISerialService)} no está registrado";
                    return false;
                }

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
