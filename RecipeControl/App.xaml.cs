using Microsoft.Extensions.DependencyInjection;
using RecipeControl.Configuration;
using RecipeControl.Services.Database;
using RecipeControl.Services.Interfaces;
using RecipeControl.Services.Serial;
using RecipeControl.ViewModels;
using RecipeControl.Views;
using System.Data;
using System.Net.NetworkInformation;
using System.Windows;

namespace RecipeControl
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IServiceProvider? _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                var config = ConfigurationManager.Instance;

                // Load and validate configuration
                if (!config.ValidateConfiguration(out string errorMessage))
                {
                    MessageBox.Show(
                        $"Error en la configuración:\n{errorMessage}\n\nVerifique el archivo appsettings.json",
                        "Error de Configuración",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    Shutdown();
                    return;
                }

                // Configures dependency injection
                _serviceProvider = ConfigureServices();

                // Check that the services are registered
                if (!DependencyInjection.ValidateServices(_serviceProvider, out string validationError))
                {
                    MessageBox.Show(
                        $"Error al validar servicios:\n\n{validationError}",
                        "Error de Configuración",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    Shutdown();
                    return;
                }

                // Show test window on startup
                //var testWindow = _serviceProvider.GetRequiredService<TestConnectionView>();
                //var resultado = testWindow.ShowDialog();

                // Show main windows on successfull test
                //if (resultado == true || MessageBox.Show(
                //    "¿Desea continuar sin probar las conexiones?",
                //    "Advertencia",
                //    MessageBoxButton.YesNo,
                //    MessageBoxImage.Warning) == MessageBoxResult.Yes)
                //{
                try
                {
                    var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                    mainWindow.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Startup error");
                }
                //}
                //else
                //{
                //    Shutdown();
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al iniciar la aplicación:\n{ex.Message}",
                    "Error Fatal",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Shutdown();
            }
        }

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddApplicationServices();

            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Retrieves a service from the DI container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static T GetService<T>() where T : class
        {
            var app = (App)Current;
            return app._serviceProvider?.GetService<T>()
                ?? throw new InvalidOperationException($"Servicio {typeof(T).Name} no registrado.");
        }

        /// <summary>
        /// Retrieves service provider
        /// </summary>
        public static IServiceProvider ServiceProvider
        {
            get
            {
                var app = (App)Current;
                return app._serviceProvider ?? throw new InvalidOperationException("ServiceProvider no inicializado");
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }

            base.OnExit(e);
        }
    }

}
