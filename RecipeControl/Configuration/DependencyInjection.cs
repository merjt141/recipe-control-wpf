using Microsoft.Extensions.DependencyInjection;
using RecipeControl.Helpers;
using RecipeControl.Helpers.Interfaces;
using RecipeControl.Models.Config;
using RecipeControl.Repositories;
using RecipeControl.Repositories.Interfaces;
using RecipeControl.Services.Credential;
using RecipeControl.Services.Database;
using RecipeControl.Services.Host;
using RecipeControl.Services.Interfaces;
using RecipeControl.Services.Reports;
using RecipeControl.Services.Scales;
using RecipeControl.Services.Serial;
using RecipeControl.ViewModels;
using RecipeControl.ViewModels.RegisterModule;
using RecipeControl.ViewModels.ReportModule;
using RecipeControl.Views;
using RecipeControl.Views.RegisterModuleViews;
using RecipeControl.Views.ReportModuleViews;
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
            services.AddSingleton(config.Reports);

            // ===== INFRAESTRUCTURE SERVICES =====
            services.AddSingleton<ISerialService, SerialService>();
            services.AddSingleton<IDatabaseService, DatabaseService>();
            services.AddSingleton<IReportService, ReportService>();
            services.AddSingleton<IExcelService, ExcelService>();
            services.AddSingleton<ICredentialService, CredentialService>();

            services.AddSingleton<IWeighingService, WeighingService>();
            services.AddSingleton<ScaleManagerHostedService>();
            services.AddSingleton<IScaleFactory,ScaleFactory>();
            services.AddSingleton<IScaleCommunicationService, ScaleCommunicationService>();
            services.AddSingleton<IScaleDataProcessingService, ScaleDataProcessingService>();

            // ===== REPOSITORIES =====
            services.AddScoped<ITipoInsumoRepository, TipoInsumoRepository>();
            services.AddScoped<IRegistroPesoRepository, RegistroPesoRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IInsumoRepository, InsumoRepository>();
            services.AddScoped<IRecetaVersionRepository, RecetaVersionRepository>();
            services.AddScoped<IRecetaVersionDetalleRepository, RecetaVersionDetalleRepository>();

            // ===== HELPERS =====
            services.AddTransient<IConnectionHelper, ConnectionHelper>();

            // ===== VIEWMODELS =====
            services.AddTransient<MainViewModel>();
            services.AddTransient<RegisterModuleViewModel>();
            services.AddTransient<ReportModuleViewModel>();

            // ===== VIEWS =====
            services.AddTransient<MainWindow>();
            services.AddTransient<RegisterModuleView>();
            services.AddTransient<ReportModuleView>();
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
