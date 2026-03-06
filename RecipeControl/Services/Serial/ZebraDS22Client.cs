using RecipeControl.Configuration;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using RecipeControl.Views;
using RecipeControl.Services.Loggs;

namespace RecipeControl.Services.Serial
{
    public static class ZebraDS22Client
    {
        // Variables de puerto
        private static SerialPort? _sp;
        private static readonly StringBuilder _rx = new StringBuilder();

        // Control de timeout
        private static readonly object _lock = new();
        private const int ScanTimeoutMs = 1500;
        private static System.Timers.Timer _scanTimeout = new System.Timers.Timer(ScanTimeoutMs);

        // Declaración de eventos
        public static event Action<string>? OnQRScanned;

        /// <summary>
        /// Inicializa los componentes del servicio controlador de
        /// lectura de QR
        /// </summary>
        public static async Task SerialQRStartupAsync()
        {
            // Control de timeout de lectura de QR
            _scanTimeout = new System.Timers.Timer(ScanTimeoutMs);
            _scanTimeout.AutoReset = false;
            _scanTimeout.Elapsed += (_, _) =>
            {
                lock (_lock)
                {
                    _rx.Clear();
                }

                // Lanza evento de falla por timeout
                OnScanFailedTimeout();
            };

            // Configuración de puerto de lectura
            string portName = ConfigurationManager.Instance.Settings.SerialPortQR.PortName;
            try
            {
                _sp = new SerialPort(portName, 19200, Parity.None, 8, StopBits.One)
                {
                    Handshake = Handshake.None,
                    DtrEnable = true,
                    RtsEnable = true,
                    Encoding = Encoding.ASCII,
                    NewLine = "\r\n",
                    ReadTimeout = 500,
                    WriteTimeout = 500
                };

                // Data recividad
                _sp.DataReceived += DataReceived;

                // Apertura de puerto
                _sp.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No pude abrir {portName} de lector QR: {ex.Message}", "QR Serial",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                LoggerService.NotifySystem($"No pude abrir {portName} de lector QR: {ex.Message}");
            }
        }

        /// <summary>
        /// Cierra el puerto serial que se había aperturado
        /// </summary>
        /// <returns></returns>
        public static async Task CloseSerialPort()
        {
            try
            {
                if (_sp != null)
                {
                    _sp.DataReceived -= DataReceived;
                    if (_sp.IsOpen) _sp.Close();
                    _sp.Dispose();
                    _sp = null;
                }
            }
            catch
            {
                // no-op
            }
        }

        /// <summary>
        /// Evento cuando no se llegó a detectar el termiandor de QR '|1
        /// en el tiempo fijado
        /// </summary>
        private static void OnScanFailedTimeout()
        {
            MessageBox.Show("Error de lectura QR, reintentar", "Error", MessageBoxButton.OK, MessageBoxImage.None);
        }

        /// <summary>
        /// Procesamiento de valores de lectura de QR
        /// </summary>
        private static void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (_sp == null) return;

            try
            {
                // Extrae los datos actuales del buffer
                string chunk = _sp.ReadExisting();

                if (string.IsNullOrEmpty(chunk)) return;

                string? payloadToProcess = null;

                lock (_rx)
                {
                    // Inicializa el timer de timeout
                    if (_rx.Length == 0)
                        _scanTimeout.Start();

                    // Concatena respuesta de puerto serial
                    _rx.Append(chunk);

                    string buffer = _rx.ToString();

                    // Busca elemento terminador en la cadena de caracteres
                    int nl = buffer.IndexOf('|');
                    if (nl >= 0)
                    {
                        // Almacena el payload en memoria
                        payloadToProcess = buffer.Substring(0, nl);
                        _rx.Remove(0, nl + 1);

                        // Detiene el temporizador de timeout
                        _scanTimeout.Stop();
                    }
                }

                // Confirma que está entrando y llegando chunk al controlador de mensajes
                LoggerService.NotifySystem($"RX chunk: {chunk.Length} bytes");

                // Retorna si no se ha acabado de recibir el buffer
                if (payloadToProcess == null) return;

                // Elimina caracteres especiales
                string payload = payloadToProcess.Trim('\r', '\n', ' ');
                if (string.IsNullOrWhiteSpace(payload)) return;
                
                // Lanza evento de recepción de buffer en payload
                OnQRScanned?.Invoke(payload);

            }
            catch (Exception ex)
            {
                // Avisa en caso de falla a la controlador de mensajes
                LoggerService.NotifySystem("ERROR RX: " + ex.Message);
            }
        }

        /// <summary>
        /// Notificación de lectura exitosa
        /// </summary>
        /// <returns></returns>
        public static async Task ConfirmBeepPatternAsync()
        {
            await Task.Delay(500);
            BeepScannerOnce();
            await Task.Delay(10);
            BeepScannerOnce();
        }

        /// <summary>
        /// Comando para lanzar un beep corto en el lector de QR
        /// por serial
        /// </summary>
        private static void BeepScannerOnce()
        {
            if (_sp is { IsOpen: true })
                _sp.Write(new byte[] { 0x07 }, 0, 1);
        }
    }
}
