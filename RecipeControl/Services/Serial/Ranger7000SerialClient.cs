using RecipeControl.Configuration;
using RecipeControl.Services.Loggs;
using System;
using System.Globalization;
using System.IO.Ports;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace RecipeControl.Services.Serial
{
    public static class Ranger7000Service
    {
        /// <summary>
        /// Captura el valor del peso de la balanaza por comunicación serial
        /// </summary>
        public static async Task<decimal> RetrieveWeightAsync()
        {
            string portName = ConfigurationManager.Instance.Settings.SerialPortScale.PortName;
            decimal net = 0;

            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(12));

                // Cambia COMx y velocidad según tu configuración RS232 de la Ranger7000
                using var client = new Ranger7000SerialClient(
                    portName: portName, // "COM3",
                    baudRate: 9600,
                    parity: Parity.None,
                    dataBits: 8,
                    stopBits: StopBits.One,
                    handshake: Handshake.None
                );

                net = await client.ReadWeightAsync(settled: false, ct: cts.Token);
            }
            catch (OperationCanceledException)
            {
                LoggerService.NotifySystem("Timeout leyendo la balanza (no respondió a tiempo).");
            }
            catch (Exception ex)
            {
                LoggerService.NotifySystem($"Error leyendo la balanza: {ex.Message}");
            }

            // === SIMULACIÓN DE PESO ===
            ///////////////////////////////////////////////////////////////////////////////////////
            //var rnd = new Random();
            //net = Math.Round((decimal)rnd.NextDouble() * 800m, 1);
            ///////////////////////////////////////////////////////////////////////////////////////

            return net;
        }
    }

    public sealed class Ranger7000SerialClient : IDisposable
    {
        private readonly SerialPort _sp;
        private readonly StringBuilder _rx = new();
        private TaskCompletionSource<string>? _tcs;

        public Ranger7000SerialClient(
            string portName,
            int baudRate = 9600,
            Parity parity = Parity.None,
            int dataBits = 8,
            StopBits stopBits = StopBits.One,
            Handshake handshake = Handshake.None)
        {
            _sp = new SerialPort(portName, baudRate, parity, dataBits, stopBits)
            {
                Handshake = handshake,
                NewLine = "\r\n", // si tu balanza usa solo \r, cambia a "\r"
                ReadTimeout = 2000,
                WriteTimeout = 2000
            };

            _sp.DataReceived += OnDataReceived;
        }

        public void Open()
        {
            if (_sp.IsOpen) return;

            _sp.Open();
            _sp.DiscardInBuffer();
            _sp.DiscardOutBuffer();
            _rx.Clear();
        }

        // settled=true => SP (estable), settled=false => IP (instantáneo)
        public async Task<decimal> ReadWeightAsync(bool settled, CancellationToken ct)
        {
            Open();

            if (_tcs != null)
                throw new InvalidOperationException("Ya hay una lectura en curso.");

            _tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);

            using var reg = ct.Register(() => _tcs.TrySetCanceled(ct));

            try
            {
                string cmd = settled ? "SP" : "IP";
                _sp.Write(cmd + "\r\n");
                LoggerService.NotifySystem("Esperando lectura de la balanza.");
                string line = await _tcs.Task.ConfigureAwait(false);

                return ParseOnlyNumber(line);
            }
            finally
            {
                _tcs = null;
            }
        }

        private void OnDataReceived(object? sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string incoming = _sp.ReadExisting();
                if (string.IsNullOrEmpty(incoming)) return;

                _rx.Append(incoming);

                string nl = _sp.NewLine;
                int idx;
                while ((idx = _rx.ToString().IndexOf(nl, StringComparison.Ordinal)) >= 0)
                {
                    string line = _rx.ToString(0, idx).Trim();
                    _rx.Remove(0, idx + nl.Length);

                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        _tcs?.TrySetResult(line);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                _tcs?.TrySetException(ex);
            }
        }

        private static decimal ParseOnlyNumber(string raw)
        {
            // Ej: "+ 12.34 kg" / "12,34 g" / "NET 0.123"
            var m = Regex.Match(raw, @"([+-]?\s*\d+(?:[.,]\d+)?)");
            if (!m.Success)
                throw new FormatException($"No se pudo interpretar el peso devuelto: '{raw}'");

            string num = m.Groups[1].Value.Replace(" ", "").Replace(",", ".");
            return decimal.Parse(num, CultureInfo.InvariantCulture);
        }

        public void Dispose()
        {
            _sp.DataReceived -= OnDataReceived;
            try { if (_sp.IsOpen) _sp.Close(); } catch { }
            _sp.Dispose();
        }
    }
}