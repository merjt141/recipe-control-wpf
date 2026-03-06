using Microsoft.Data.SqlClient;
using RecipeControl.Configuration;
using RecipeControl.Services.Serial;
using RecipeControl.Views.RegisterModuleViews;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO.Ports;
using System.Text;
using System.Windows;
using static RecipeControl.Models.DTOs;
using RecipeControl.Services.Loggs;
using RegMicro.Services.Database;

namespace RecipeControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AppSettings _appSettings;
        private readonly IServiceProvider _serviceProvider;

        private readonly Random _rng = new Random();
        private int _nextId = 1;

        // == Constantes de funciones ==
        private const int UTC = -5;

        public MainWindow(
            AppSettings appSettings,
            IServiceProvider serviceProvider)
        {
            InitializeComponent();

            _appSettings = appSettings;
            _serviceProvider = serviceProvider;

            // Importante: para que el Binding encuentre RegistroPesoList
            DataContext = this;

            //para serial QR
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        #region Eventos de inicio y fin de ventana
        /// <summary>
        /// Ejecuta en la carga de ventana para inicializar configuraciones
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Inicializar servicio de loggeo a UI
            LoggerService.OnNotification = (message) =>
            {
                Dispatcher.InvokeAsync(() =>
                {
                    Title = message;
                });
            };

            // Validar acceso a la base de datos
            try
            {
                // Si es null, es porque no hay registros o está apuntando a otra DB
                int? lastId = await DatabaseService.GetLastRegistroBatchIdAsync();
                LabelIDBatch.Content = lastId?.ToString() ?? "SIN REGISTROS";

            }
            catch (Exception ex)
            {
                LabelIDBatch.Content = "ERROR";
                MessageBox.Show($"Fallo DB: {ex.Message}", "DB", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            // Inicializar servicios de lector QR
            await ZebraDS22Client.SerialQRStartupAsync();
            ZebraDS22Client.OnQRScanned += ExecuteQRCommandDispatcher;
        }

        /// <summary>
        /// Ejecuta al inicio del cierre de la ventana para finalizar servicios limpiamente
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            // Cerrar puerto serial de lector QR
            _ = ZebraDS22Client.CloseSerialPort();
        }

        #endregion

        #region Lógica de negocio de lector de QR

        /// <summary>
        /// Evento para invocar la ejecución de ExecuteQRCommand con acceso a
        /// la interfaz de usuario
        /// </summary>
        /// <param name="payload"></param>
        private void ExecuteQRCommandDispatcher(string payload)
        {
            Dispatcher.InvokeAsync(() =>
            {
                ExecuteQRCommand(payload);
            });
        }

        /// <summary>
        /// Controlador de acciones del QR
        /// </summary>
        /// <param name="payload">String extraído directo del QR</param>
        private void ExecuteQRCommand(string payload)
        {
            // Accion del QR C: Extraer compilado, M: Macroingrediente, P: Print
            char action = payload[0];

            // Distribuir acción del tipo de QR
            switch (action)
            {
                case 'C':
                    _ = RetrieveQRData2Async(payload);
                    break;
                default:
                    MessageBox.Show($"Método no soportado, valida lectura de QR", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    LoggerService.NotifySystem("QR inválido");
                    break;
            }
        }

        // fecha: 26 02 2026
        // lectura de pistola nueva version de daos
        private async Task RetrieveQRData2Async(string payload)
        {
            // Validar que inicie con C
            if (!payload.StartsWith("C"))
            {
                LoggerService.NotifySystem("QR inválido: no inicia con 'C'");
                return;
            }

            // Quitar la C inicial
            payload = payload.Substring(1);

            // Separar por ;
            var parts = payload.Split(';');

            int PBatchRegistroId =
            int.TryParse(LabelIDBatch.Content?.ToString(), out int temp)
            ? temp
            : 1000;

            // Convertir tipos correctamente
            int PmacroRegistroId = int.Parse(parts[0]);
            int PusuarioStr = int.Parse(parts[1]);

            // Convertir fecha (formato asumido: yyMMddHHmm)
            if (!DateTime.TryParseExact(
                parts[2],
                "yyyyMMddHHmm",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTime Pfecha))
            {
                throw new Exception("Fecha en QR inválida: " + parts[2]);
            }

            // Formato string requerido
            DateTime PfechaPistoleo = DateTime.Now;

            int PloteStr = int.Parse(parts[3]);
            int PnombreMacroIdStr = int.Parse(parts[4]);

            // Pesos vienen multiplicados x10 → dividir entre 10
            decimal PpesoRealStr = decimal.Parse(parts[5]) / 10m;
            decimal PpesoObjStr = decimal.Parse(parts[6]) / 10m;

            int Pinsumo1Str = int.Parse(parts[7]);
            int Pinsumo2Str = int.Parse(parts[8]);
            int Pinsumo3Str = int.Parse(parts[9]);
            int Pinsumo4Str = int.Parse(parts[10]);

            var reportData = new DataTransferQR2DTO
            {
                BatchRegistroId = PBatchRegistroId,
                MacroRegistroId = PmacroRegistroId,
                UsuarioStr = PusuarioStr,
                Fecha = Pfecha,
                FechaPistoleo = PfechaPistoleo,
                LoteStr = PloteStr,
                NombreMacroIdStr = PnombreMacroIdStr,
                PesoRealStr = PpesoRealStr,
                PesoObjStr = PpesoObjStr,
                Insumo1Str = Pinsumo1Str,
                Insumo2Str = Pinsumo2Str,
                Insumo3Str = Pinsumo3Str,
                Insumo4Str = Pinsumo4Str
            };

            
            try
            {
                await DatabaseService.InsertBatchRegistroBolsa(reportData);

                // Asignar etiquetas
                LabelValPesado.Content = reportData.PesoRealStr.ToString("0.0", CultureInfo.InvariantCulture);
                LabelCodUnico.Content = reportData.MacroRegistroId.ToString();
                LabelFechaPesado.Content = reportData.Fecha.ToString("dd/MM/yyyy HH:mm:ss");
                LabelTipo.Content = "SOLIDO"; // fijo por ahora
                LabelLote.Content = reportData.LoteStr.ToString();

                LoggerService.NotifySystem("Se ha guardado correctamente los ingredientes");

                List<ReportDataGrid2DTO> reportDataGridPreview = await DatabaseService.LoadLast20BatchRegistroDetalle2();
                WeightRegisterDataGrid.ItemsSource = reportDataGridPreview;

                _ = ZebraDS22Client.ConfirmBeepPatternAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error guardando el insumo al batch: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LoggerService.NotifySystem("Error guardando insumo al batch, reintentar");
            }

            return;
        }

        /// <summary>
        /// Decodificar lectura de QR de transferencia y almacenar
        /// en base de datos
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        private async Task RetrieveQRDataAsync(string payload)
        {
            // Char con la cantidad de insumos en bolsa
            char sizeChar = payload[1];
            int size = (int)char.GetNumericValue(sizeChar);

            // El terminador | fue extraído en la lectura del serial
            int requiredSize = 19 + 7 * size;
            if (payload.Length != requiredSize)
            {
                MessageBox.Show("El QR se ha leído incompleto, reintentar lectura", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                LoggerService.NotifySystem("Se ha leído el QR incompleto, reintentar");
                return;
            }

            int macroRegistroId = Convert.ToInt32(payload.Substring(2, 7));
            DateTime fechaCreacion = DateTimeOffset.FromUnixTimeSeconds(Convert.ToUInt32(payload.Substring(9, 10))).ToOffset(TimeSpan.FromHours(UTC)).DateTime;

            List<DataTransferQRDTO> qRDataTransferDTOList = new List<DataTransferQRDTO>();

            for (int i = 0; i < size; i++)
            {
                int insumoIdIdx = 19 + i * 7;
                int pesoRealIdx = 21 + i * 7;

                int insumoId = Convert.ToInt32(payload.Substring(insumoIdIdx, 2)) + 990;
                decimal pesoReal = Convert.ToDecimal(payload.Substring(pesoRealIdx, 5)) / 10.0m;

                qRDataTransferDTOList.Add(new DataTransferQRDTO()
                {
                    MacroRegistroId = macroRegistroId,
                    FechaCreacion = fechaCreacion,
                    InsumoId = insumoId,
                    PesoReal = pesoReal
                });
            }

            try
            {
                await DatabaseService.InsertOrUpdateBatchRegistroDetalle(qRDataTransferDTOList);

                // Asignar etiquetas
                LabelValPesado.Content = qRDataTransferDTOList.Sum(x => x.PesoReal).ToString("0.0", CultureInfo.InvariantCulture);
                LabelCodUnico.Content = qRDataTransferDTOList[0].MacroRegistroId.ToString();
                LabelFechaPesado.Content = qRDataTransferDTOList[0].FechaCreacion.ToString("dd/MM/yyyy HH:mm:ss");
                LabelTipo.Content = "SOLIDO"; // fijo por ahora
                LabelLote.Content = LabelIDBatch.Content;

                LoggerService.NotifySystem("Se ha guardado correctamente los ingredientes");

                List<ReportDataGridDTO> reportDataGridPreview = await DatabaseService.LoadLast20BatchRegistroDetalle();
                WeightRegisterDataGrid.ItemsSource = reportDataGridPreview;

                _ = ZebraDS22Client.ConfirmBeepPatternAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error guardando el insumo al batch: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LoggerService.NotifySystem("Error guardando insumo al batch, reintentar");
            }

            return;
        }

        #endregion

        private void BotonConfig_Click(object sender, RoutedEventArgs e)
        {
            var win = new Configuracion();
            win.Owner = Window.GetWindow(this);   // para que quede encima de la ventana padre
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.ShowDialog(); // o Show() si no quieres modal
        }
    }
}
