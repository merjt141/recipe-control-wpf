using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Services.Ethernet.Printers
{
    public static class ZebraUsbSender
    {
        public static Task SendAsync(string printerName, string zpl)
            => Task.Run(() => Send(printerName, zpl));

        private static void Send(string printerName, string zpl)
        {
            if (!OpenPrinter(printerName, out var hPrinter, IntPtr.Zero))
                throw new InvalidOperationException($"No se pudo abrir la impresora: {printerName}");

            try
            {
                var di = new DOCINFOA { pDocName = "ZPL Job", pDataType = "RAW" };

                if (!StartDocPrinter(hPrinter, 1, ref di))
                    throw new InvalidOperationException("StartDocPrinter falló.");

                if (!StartPagePrinter(hPrinter))
                    throw new InvalidOperationException("StartPagePrinter falló.");

                // Para QR y datos simples: ASCII es lo más estable.
                // Si usarás tildes/ñ, dímelo y lo ajustamos a UTF-8/CI28.
                var bytes = Encoding.ASCII.GetBytes(zpl);

                if (!WritePrinter(hPrinter, bytes, bytes.Length, out _))
                    throw new InvalidOperationException("WritePrinter falló.");

                EndPagePrinter(hPrinter);
                EndDocPrinter(hPrinter);
            }
            finally
            {
                ClosePrinter(hPrinter);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)] public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)] public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)] public string pDataType;
        }

        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true)]
        private static extern bool OpenPrinter(string pPrinterName, out IntPtr phPrinter, IntPtr pDefault);

        [DllImport("winspool.Drv", SetLastError = true)]
        private static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true)]
        private static extern bool StartDocPrinter(IntPtr hPrinter, int level, ref DOCINFOA di);

        [DllImport("winspool.Drv", SetLastError = true)]
        private static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", SetLastError = true)]
        private static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", SetLastError = true)]
        private static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", SetLastError = true)]
        private static extern bool WritePrinter(IntPtr hPrinter, byte[] pBytes, int dwCount, out int dwWritten);
    }
}