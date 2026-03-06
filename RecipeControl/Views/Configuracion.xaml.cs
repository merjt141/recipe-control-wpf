using RecipeControl.Configuration;
using RecipeControl.Services.Database;
using System;
using System;
using System.Collections.Generic;
using System.IO; 
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static RecipeControl.Models.DTOs;

namespace RecipeControl.Views
{
    /// <summary>
    /// Lógica de interacción para Configuracion.xaml
    /// </summary>
    public partial class Configuracion : Window
    {
        private List<InsumoPesoDTO> _insumos = new();
        public Configuracion()
        {
            InitializeComponent();
            RefreshPorts();
            UpdateLabelComConf();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshPorts();
            UpdateLabelComConf();
        }

        private void UpdateLabelComConf()
        {
            // Lee lo que está usando el sistema (appsettings del BIN)
            string com = ConfigurationManager.Instance.Settings.SerialPortScale.PortName;

            if (string.IsNullOrWhiteSpace(com))
                LabelComConf.Content = "COM actual: (no configurado)";
            else
                LabelComConf.Content = $"COM actual: {com}";
        }

        private void RefreshPorts()
        {
            var ports = SerialPort.GetPortNames()
                                  .OrderBy(p => p)
                                  .ToList();

            ComboSelecCOM.ItemsSource = ports;

            if (ports.Count > 0 && ComboSelecCOM.SelectedIndex < 0)
                ComboSelecCOM.SelectedIndex = 0;
        }

        private void BotonGuardaCOMBLZ_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ComboSelecCOM.SelectedItem == null)
                {
                    MessageBox.Show("Selecciona un puerto COM primero.", "Config COM",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string selectedPort = ComboSelecCOM.SelectedItem.ToString()!.Trim();

                // Decide qué archivo editar (Production/Development)
                string env = ConfigurationManager.GetEnvironment(); // "Production" o "Development"
                string fileName = env.Equals("Development", StringComparison.OrdinalIgnoreCase)
                    ? "appsettings.Development.json"
                    : "appsettings.json";

                // Mejor que CurrentDirectory: usar BaseDirectory (bin\Debug\netX)
                string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

                if (!File.Exists(filePath))
                {
                    filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                    if (!File.Exists(filePath))
                    {
                        MessageBox.Show("No se encontró appsettings.json en el directorio de ejecución.", "Config COM",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                // Leer y editar JSON
                string jsonText = File.ReadAllText(filePath);
                JsonNode? root = JsonNode.Parse(jsonText);

                if (root == null)
                {
                    MessageBox.Show("No se pudo leer el archivo de configuración (JSON inválido).", "Config COM",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Actualizar la sección correcta
                root["SerialPortScale"] ??= new JsonObject();
                root["SerialPortScale"]!["PortName"] = selectedPort;

                // Guardar con indentado
                var options = new JsonSerializerOptions { WriteIndented = true };
                File.WriteAllText(filePath, root.ToJsonString(options));

                // Recargar config en runtime
                ConfigurationManager.Instance.ReloadConfiguration();

                // Refresca nuevo COM configurado
                UpdateLabelComConf();

                MessageBox.Show(
                    $"Guardado correctamente:\nSerialPortScale.PortName = {selectedPort}\nArchivo: {System.IO.Path.GetFileName(filePath)}",
                    "Config COM", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error guardando el puerto COM:\n{ex.Message}", "Config COM",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task CargarInsumosAsync()
        {
            _insumos = await DatabaseService.LoadInsumosPesoAsync();
            InsumosDataGrid.ItemsSource = _insumos;

            BtnGuardarPesos.IsEnabled = _insumos.Count > 0;
        }
        private async void BtnRecargarInsumos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await CargarInsumosAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar:\n{ex.Message}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            }
        }

        private async void BtnGuardarPesos_Click(object sender, RoutedEventArgs e)
        {
            // Confirmación antes de guardar
            var confirm = MessageBox.Show(
                "¿Deseas guardar los pesos Frima?",
                "Confirmar guardado",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes)
                return;

            try
            {
                // 1. Forzar commit de la celda que esté en edición
                InsumosDataGrid.CommitEdit();
                InsumosDataGrid.CommitEdit();

                // 2. Validación básica (decimal(7,4) → máximo 999.9999)
                if (_insumos.Any(x => x.PesoFrima1 < 0 || x.PesoFrima1 > 999.9999m))
                {
                    MessageBox.Show("PesoFrima1 fuera de rango permitido (0 - 999.9999).",
                                    "Validación",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);
                    return;
                }

                // 3. Guardar en base de datos
                await DatabaseService.UpdatePesoFrima1Async(_insumos);

                MessageBox.Show("Pesos actualizados correctamente.",
                                "Éxito",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar:\n{ex.Message}",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }
    }
}
