using Microsoft.Data.SqlClient;
using RecipeControl.Configuration;
using RecipeControl.Helpers.Interfaces;
using RecipeControl.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RecipeControl.Views
{
    /// <summary>
    /// Interaction logic for TestConnectionView.xaml
    /// </summary>
    public partial class TestConnectionView : Window
    {
        private readonly IConnectionHelper _connectionHelper;
        private readonly IDatabaseService _databaseService;
        private readonly AppSettings _settings;
        private bool _dbSuccessfullConnection = false;

        public TestConnectionView()
        {
            InitializeComponent();

            // Retrieve services
            _connectionHelper = App.GetService<IConnectionHelper>();
            _databaseService = App.GetService<IDatabaseService>();
            _settings = ConfigurationManager.Instance.Settings;

            ShowCurrentConfiguration();
        }

        private void ShowCurrentConfiguration()
        {
            var connectionString = _settings.ConnectionStrings.DefaultConnection;
            var builder = new SqlConnectionStringBuilder(connectionString);

            var config = $"📋 Configuración del Sistema\n\n" +
                        $"Base de Datos:\n" +
                        $"  • Servidor: {builder.DataSource}\n" +
                        $"  • Base de Datos: {builder.InitialCatalog}\n" +
                        $"  • Timeout: {_settings.Database.CommandTimeout}s\n" +
                        $"  • Max Reintentos: {_settings.Database.MaxRetryCount}\n\n" +
                        $"Puerto Serial:\n" +
                        $"  • Puerto: {_settings.SerialPort.PortName}\n" +
                        $"  • Baud Rate: {_settings.SerialPort.BaudRate}\n" +
                        $"  • Data Bits: {_settings.SerialPort.DataBits}\n" +
                        $"  • Parity: {_settings.SerialPort.Parity}\n" +
                        $"  • Stop Bits: {_settings.SerialPort.StopBits}";

            txtConfiguracion.Text = config;
        }

        private async void BtnProbarDB_Click(object sender, RoutedEventArgs e)
        {
            await TestConnectionDBAsync();
        }

        private async Task TestConnectionDBAsync()
        {
            try
            {
                progressDB.Visibility = Visibility.Visible;
                progressDB.IsIndeterminate = true;
                txtResultadoDB.Text = "🔄 Probando conexión a la base de datos...";

                var resultado = await _connectionHelper.TestDBConnectionAsync();

                progressDB.IsIndeterminate = false;
                progressDB.Visibility = Visibility.Collapsed;

                // Mostrar resultado
                txtResultadoDB.Text = resultado.Mensaje;
                txtResultadoDB.Foreground = resultado.Exitoso ?
                    new SolidColorBrush(Color.FromRgb(39, 174, 96)) :
                    new SolidColorBrush(Color.FromRgb(231, 76, 60));

                _dbSuccessfullConnection = resultado.Exitoso;
                btnCrearTablas.IsEnabled = resultado.Exitoso;

                // Verificar si las tablas existen
                if (resultado.Exitoso)
                {
                    var tablasExisten = await _connectionHelper.CheckTablesExistanceAsync();
                    if (tablasExisten)
                    {
                        txtResultadoDB.Text += "\n\n✓ Las tablas del sistema ya existen";
                        btnCrearTablas.Content = "Tablas OK";
                        btnCrearTablas.IsEnabled = false;
                    }
                    else
                    {
                        txtResultadoDB.Text += "\n\n⚠️ Las tablas del sistema NO existen. Presione 'Crear Tablas'";
                        btnCrearTablas.Content = "Crear Tablas";
                        btnCrearTablas.IsEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                progressDB.IsIndeterminate = false;
                progressDB.Visibility = Visibility.Collapsed;
                txtResultadoDB.Text = $"x Error inesperado: {ex.Message}";
                txtResultadoDB.Foreground = new SolidColorBrush(Color.FromRgb(231, 76, 60));
                _dbSuccessfullConnection = false;
            }
        }
    }
}
