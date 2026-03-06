using RecipeControl.Configuration;
using RecipeControl.Services.Database;
using RecipeControl.Services.Ethernet.Printers;
using RecipeControl.Services.Loggs;
using RecipeControl.Services.Serial;
using System.Diagnostics;
using System.Globalization;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using static RecipeControl.Models.DTOs;

namespace RecipeControl.Views
{
    public partial class RegisterModuleView : Window
    {

        // TODO: Ajusta estos valores según tu tarjeta Ethernet (IP/puerto reales)
        private const string It1000Ip = "192.168.1.50";
        private const int It1000Port = 1234;

        // == Constantes de funciones ==
        private const int UTC = -5;

        // === Variables de interfaz ===
        private int _tipoInsumoId;
        private int _insumoId;

        public RegisterModuleView()
        {
            InitializeComponent();

            // Inicializar elementos
            StartUpWindowsComponents();
        }

        #region Eventos de UI

        /// <summary>
        /// Evento de click de botón Generar QR
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MacroPreImpresionBtn_Click(object sender, RoutedEventArgs e)
        {
            //// Validaciones de campos
            //if (string.IsNullOrWhiteSpace(MacroCodeAmountInput.Text))
            //{
            //    MessageBox.Show("Ingrese un valor.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            //    return;
            //}

            //if (!int.TryParse(MacroCodeAmountInput.Text, out int nTimes) || nTimes < 1)
            //{
            //    MessageBox.Show("Ingrese un valor válido de impresiones.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            //    return;
            //}

            // Imprime la cantidad solicitada de veces
            //PrintEmptyNTimes(nTimes);
        }

        /// <summary>
        /// Apertura de ventana de configuración de puerto serial balanza
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BotonConfig_Click(object sender, RoutedEventArgs e)
        {
            var win = new Configuracion();
            win.Owner = Window.GetWindow(this);   // para que quede encima de la ventana padre
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.ShowDialog(); // o Show() si no quieres modal
        }

        /// <summary>
        /// Actualización de valor de indice de combobox en backend
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SAPCodeCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _insumoId = Convert.ToInt32(SAPCodeCmb.SelectedValue);

        }

        #endregion

        #region Control de elementos de UI / Inicialización

        private async void StartUpWindowsComponents()
        {
            // Inicialización de servicio de notificaciones
            LoggerService.OnNotification = (message) =>
            {
                Dispatcher.InvokeAsync(() =>
                {
                    LabelMsgPesaje.Text = message;
                });
            };

            // Inicialización de combobox con base de datos
            try
            {
                _tipoInsumoId = 1001; // Selección fija en tipo SOLIDOS

                // Listado de insumos
                List<ComboBoxDTO> InsumoList = await DatabaseService.GetInsumosByTipo(_tipoInsumoId);
                SAPCodeCmb.ItemsSource = InsumoList;
                // Forzar selección por defecto
                SAPCodeCmb.SelectedIndex = 0;

                // Listado de insumos
                List<ComboBoxDTO> InsumoList2 = await DatabaseService.GetInsumosByTipo(_tipoInsumoId);
                SAPCodeCmb2.ItemsSource = InsumoList2;
                // Forzar selección por defecto
                SAPCodeCmb2.SelectedIndex = 0;
                // Listado de insumos

                List<ComboBoxDTO> InsumoList3 = await DatabaseService.GetInsumosByTipo(_tipoInsumoId);
                SAPCodeCmb3.ItemsSource = InsumoList3;
                // Forzar selección por defecto
                SAPCodeCmb3.SelectedIndex = 0;

                // Listado de insumos
                List<ComboBoxDTO> InsumoList4 = await DatabaseService.GetInsumosByTipo(_tipoInsumoId);
                SAPCodeCmb4.ItemsSource = InsumoList4;
                // Forzar selección por defecto
                SAPCodeCmb4.SelectedIndex = 0;

                // Llenado de combo de insumos - por ahora se inician vacios.
                //_insumoId = InsumoList.FirstOrDefault()?.Id ?? 1001;
                //SAPCodeCmb.SelectedValue = _insumoId;

            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error actualizando la interfaz de la BD: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

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
                _ = ZebraDS22Client.ConfirmBeepPatternAsync();
                LoggerService.NotifySystem("Se ha guardado correctamente los ingredientes");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error guardando el insumo al batch: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LoggerService.NotifySystem("Error guardando insumo al batch, reintentar");
            }

            return;
        }

        /// <summary>
        /// Impresión del compilado de insumos de la bolsa seleccionada desde a base da datos
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        private async Task ImprimirCompiladoInsumos(string payload)
        {
            // Validar valor de código único
            if (!int.TryParse(UnicodeInput.Text, out int macroRegistroId))
            {
                MessageBox.Show("No se ha ingresado ningún valor de código único, leer de nuevo el QR de bolsa a imprimir", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LoggerService.NotifySystem("Reintentar lectura de QR");
                return;
            }

            List<DataTransferQRDTO> qRDataTransferList = await DatabaseService.GetCompiladoPesosAsync(macroRegistroId);

            if (qRDataTransferList is null) return;

            DateTime fechaCreacion = qRDataTransferList[0].FechaCreacion;

            List<string> qrArrayData = new List<string>
            {
                qRDataTransferList.Count().ToString(),                                                              // 01 CHAR = 01
                macroRegistroId.ToString(),                                                                         // 07 CHAR = 07
                ((uint)new DateTimeOffset(fechaCreacion.ToUniversalTime()).ToUnixTimeSeconds()).ToString(),         // 10 CHAR = 10
            };

            foreach (DataTransferQRDTO qRDataTransfer in qRDataTransferList)
            {
                qrArrayData.Add((qRDataTransfer.InsumoId - 990).ToString());                                        // 02 CHAR = 02
                qrArrayData.Add((qRDataTransfer.PesoReal * 10).ToString("00000", CultureInfo.InvariantCulture));    // 05 CHAR = 05
            }

            string qrData = string.Join("", qrArrayData);

            try
            {
                int qrMagnification = 10;

                // ZPL con formato tipo foto
                string zpl = ZebraZplBuilder.BuildEtiquetaCompilada(
                    qrData: qrData,
                    qRDataTransferList: qRDataTransferList,
                    qrMagnification: qrMagnification
                );

                string printerName = "ZDesigner ZD220-203dpi ZPL";
                Debug.WriteLine(zpl);
                await ZebraUsbSender.SendAsync(printerName, zpl);

                _ = ZebraDS22Client.ConfirmBeepPatternAsync();
                LoggerService.NotifySystem("Etiqueta enviada a la impresora.");
            }
            catch (Exception ex)
            {
                LoggerService.NotifySystem($"Error imprimiendo QR: {ex.Message}");
            }
        }

        // fecha 25 02 2026
        //imprimir QR final
        private async Task ImprimirCompiladoInsumos2()
        {
            // Validar valor de código único
            if (!int.TryParse(UnicodeInput.Text, out int macroRegistroId))
            {
                MessageBox.Show("No se ha ingresado ningún valor de código único, leer de nuevo el QR de bolsa a imprimir",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LoggerService.NotifySystem("Reintentar lectura de QR");
                return;
            }

            List<DataTransfer2QRDTO> qRDataTransfer2List =
                await DatabaseService.GetCompiladoMacroIngrAsync(macroRegistroId);

            if (qRDataTransfer2List.Count == 0)
            {
                MessageBox.Show("No se encontró información para el MacroRegistroId.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var data = qRDataTransfer2List[0];

            // ====== CONVERSIONES A NUMÉRICO -> STRING ======

            // Usuario representado por 10 provisional
            string usuarioStr = "10";

            // Fecha solo hasta minutos: yyyyMMddHHmm (ej: 202602251435)
            string fechaStr = data.FechaCreacion.ToString("yyyyMMddHHmm");

            string loteStr = data.Lote.ToString();
            string nombreMacroIdStr = data.NombreMacroId.ToString();

            // Pesos: x10 -> entero -> string
            int pesoRealInt = (int)Math.Round(data.PesoTotRealGr * 10m, 0, MidpointRounding.AwayFromZero);
            int pesoObjInt = (int)Math.Round(data.PesoTotObjGr * 10m, 0, MidpointRounding.AwayFromZero);

            string pesoRealStr = pesoRealInt.ToString();
            string pesoObjStr = pesoObjInt.ToString();

            string insumo1Str = data.Insumo1Id.ToString();
            string insumo2Str = data.Insumo2Id.ToString();
            string insumo3Str = data.Insumo3Id.ToString();
            string insumo4Str = data.Insumo4Id.ToString();

            // ====== CONCATENADO FINAL PARA EL QR ======
            // Orden sugerido: Usuario;Fecha;Lote;NombreMacroId;PesoReal10;PesoObj10;I1;I2;I3;I4
            string qrData = string.Join(";",
                macroRegistroId,
                usuarioStr,
                fechaStr,
                loteStr,
                nombreMacroIdStr,
                pesoRealStr,
                pesoObjStr,
                insumo1Str,
                insumo2Str,
                insumo3Str,
                insumo4Str
            );

            // Ejemplo: mostrar para validar (luego tú lo mandas a la impresora/QR)
            //MessageBox.Show(qrData, "QR Data");

            try
            {
                int qrMagnification = 10;

                // ZPL con formato tipo foto
                string zpl = ZebraZplBuilder.BuildEtiquetaCompilada2(
                    qrData: qrData,
                    qRDataTransfer2List: qRDataTransfer2List,
                    qrMagnification: qrMagnification
                );

                string printerName = "ZDesigner ZD220-203dpi ZPL";
                Debug.WriteLine(zpl);
                await ZebraUsbSender.SendAsync(printerName, zpl);

                _ = ZebraDS22Client.ConfirmBeepPatternAsync();
                LoggerService.NotifySystem("Etiqueta final enviada a la impresora.");
            }
            catch (Exception ex)
            {
                LoggerService.NotifySystem($"Error imprimiendo QR: {ex.Message}");
            }

        }


        /// <summary>
        /// Registrar el valor del insumo pesado actual en
        /// la base de datos con el índice de bolsa
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        private async Task RegistrarInsumoBolsaMacro(string payload)
        {
            // action;fecha;codigoUnico
            var parts = payload.Split(';');

            if (parts.Length < 3)
            {
                LoggerService.NotifySystem($"QR incompleto ({parts.Length} campos)");
                return;
            }

            string fechaStr = parts[1].Trim();
            string codUnicoStr = parts[2].Trim();

            // Fecha: viene como "dd/MM/yyy HH:mm:ss"
            bool okFecha = DateTime.TryParseExact(fechaStr, "dd/MM/yyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaPesado);

            // Codigo único es un valor entero
            if (!int.TryParse(codUnicoStr, out int codUnico))
            {
                MessageBox.Show($"Lectura inválida dle QR, reintentar", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }            

            // Ejecutar funciones de pesado de insumos
            decimal net = await Ranger7000Service.RetrieveWeightAsync();

            // Actualizar valores de interfaz
            UnicodeInput.Text = codUnico.ToString();
            DatetimeInput.Text = okFecha ? fechaPesado.ToString("dd/MM/yyyy HH:mm:ss") : fechaStr;
            WeighInput.Text = net.ToString("0.0", CultureInfo.InvariantCulture);

            // === EJECUTAR GUARDADO DE DATOS EN BASE DE DATOS ===
            int insumoId = Convert.ToInt32(SAPCodeCmb.SelectedValue);
            int insumoLote = Convert.ToInt32(LoteInsumoInput.Text);
            decimal pesoObjetivo = await DatabaseService.GetPesoObjetivoAsync(insumoId) * 1000;
            decimal pesoReal = net;

            try
            {
                await DatabaseService.InsertOrUpdateMicroAsync(codUnico, insumoId, insumoLote, pesoObjetivo, pesoReal);
                _ = ZebraDS22Client.ConfirmBeepPatternAsync();
                LoggerService.NotifySystem("QR Cargado correctamente");

            }
            catch (Exception ex)
            {
                LoggerService.NotifySystem("QR OK + INSERT ERROR");
                MessageBox.Show($"Error insertando en DB: {ex.Message}", "DB",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #endregion

        // Fecha: 25 02 2026
        // Registro de todos los datos de la bolsa incluido los insumos "solo KPI".
        // registra en tabla dbo.MCR_MacroRegistro
        private async Task RegistrarDatosBolsaMacro(string payload)
        {

        }


        #region Lógica de pre impresión de etiquetas bolsa vacía

            /// <summary>
            /// Función de impresión de múltiples etiquetas
            /// </summary>
            /// <param name="nTimes"></param>
            /// <exception cref="InvalidOperationException"></exception>
        private async void PrintEmptyNTimes(int nTimes)
        {
            // Valores de guardado de macro ingredientes
            DateTime MacroDateTime = DateTime.Now;
            string MacroDatetimeString = MacroDateTime.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

            string cs = ConfigurationManager.Instance.Settings.ConnectionStrings.DefaultConnection;

            if (string.IsNullOrWhiteSpace(cs))
                throw new InvalidOperationException("No hay cadena de conexión configurada (DefaultConnection).");

            // Iterar múltiples impresiones
            for (int i = 0; i < nTimes; i++)
            {
                int MCR_MacroRegistroId = 0;

                try
                {
                    MCR_MacroRegistroId = await DatabaseService.InsertMacroAndReturnIdAsync(MacroDateTime);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error insertando en DB: {ex.Message}", "DB",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Construcción de string
                string fecha = EscapeForZpl((MacroDatetimeString ?? "").Trim());
                string unico = EscapeForZpl((MCR_MacroRegistroId.ToString() ?? "").Trim());

                string zpl = ZebraZplBuilder.BuildEtiqueaVacia(
                    fecha: fecha,
                    codigoUnico: unico,
                    qrMagnification: 10
                );

                string printerName = "ZDesigner ZD220-203dpi ZPL";

                // Impresión de etiquetas de pesos
                try
                {
                    await ZebraUsbSender.SendAsync(printerName, zpl);
                    LoggerService.NotifySystem($"Etiqueta N° {i + 1} enviada a la impresora.");
                    Debug.WriteLine(zpl);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error al imprimir {ex.Message}");
                    LoggerService.NotifySystem($"Impresora no encontrada, zpl solo en consola");
                }
            }
        }

        /// <summary>
        /// Reemplaza valores que podrían dañar el archvio ZPL
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string EscapeForZpl(string s)
        {
            return (s ?? "")
                .Replace("^", "^^")
                .Replace("\r", " ")
                .Replace("\n", " ");
        }

        #endregion

        //boton para capturar pesado
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var item1 = (ComboBoxDTO)SAPCodeCmb.SelectedItem;
            decimal peso1 = item1.PesoFrima1;
            var item2 = (ComboBoxDTO)SAPCodeCmb2.SelectedItem;
            decimal peso2 = item2.PesoFrima1;
            var item3 = (ComboBoxDTO)SAPCodeCmb3.SelectedItem;
            decimal peso3 = item3.PesoFrima1;
            var item4 = (ComboBoxDTO)SAPCodeCmb4.SelectedItem;
            decimal peso4 = item4.PesoFrima1;

            decimal pesoObj = Math.Round((peso1 + peso2 + peso3 + peso4) * 1000m, 1); // suma de pesos objetivo de insumos pasado a g
            PesoObjLbl.Content = pesoObj.ToString("F1");
            // Ejecutar funciones de pesado de insumos
            decimal net = await Ranger7000Service.RetrieveWeightAsync();
            WeighInput.Text = net.ToString();
            //LoggerService.NotifySystem($"PesoFrima1: {pesoObj}");
        }

        // une todos los datos para generar el QR todo en uno
        private async void Button2_Click(object sender, RoutedEventArgs e)
        {



            int insd1 = (SAPCodeCmb.SelectedValue != null) ? Convert.ToInt32(SAPCodeCmb.SelectedValue) : 1000;
            int insd2 = (SAPCodeCmb2.SelectedValue != null) ? Convert.ToInt32(SAPCodeCmb2.SelectedValue) : 1000;
            int insd3 = (SAPCodeCmb3.SelectedValue != null) ? Convert.ToInt32(SAPCodeCmb3.SelectedValue) : 1000;
            int insd4 = (SAPCodeCmb4.SelectedValue != null) ? Convert.ToInt32(SAPCodeCmb4.SelectedValue) : 1000;

            int lote = Convert.ToInt32(LoteInsumoInput.Text);

            try
            {
                // 1) Fecha pesado
                DateTime fechaCreacion = DateTime.Now;

                // 2) Insert en DB y devuelve MacroRegistroId
                int macroRegistroId = await DatabaseService.InsertMacroAndReturnIdAsync(fechaCreacion);

                // 3) Mostrar en UI
                DatetimeInput.Text = fechaCreacion.ToString("dd/MM/yyyy HH:mm:ss");
                decimal PesoReal = Convert.ToDecimal(WeighInput.Text);

                // 4) Código único basado en el ID (recomendado)
                UnicodeInput.Text = $"{macroRegistroId}";

                decimal PesoObj = Convert.ToDecimal(PesoObjLbl.Content);

                await DatabaseService.InsertOrUpdateMacroTotalAsync(macroRegistroId, "op", fechaCreacion,lote, 10, PesoReal, PesoObj, insd1, insd2, insd3, insd4);
                //_ = ZebraDS22Client.ConfirmBeepPatternAsync();

                await ImprimirCompiladoInsumos2();  //llamo al extractor de datos de la bolsa para imprimir version final 25 02 2026

                LoggerService.NotifySystem("QR Cargado correctamente");

               

            }
            catch (Exception ex)
            {

            }
        }

        private void ProdNomCodeCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SAPCodeCmb2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SAPCodeCmb3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SAPCodeCmb4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

    }
}