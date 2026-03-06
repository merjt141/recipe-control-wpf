using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Services.Ethernet.Printers
{
    internal class ZebraTcpSender
    {
        /// <summary>
        /// Envía ZPL por TCP/IP a la impresora Zebra (puerto típico: 9100).
        /// </summary>
        public static async Task SendAsync(string ip, int port, string zpl)
        {
            if (string.IsNullOrWhiteSpace(ip))
                throw new ArgumentException("IP de impresora inválida.", nameof(ip));

            if (port <= 0)
                throw new ArgumentException("Puerto inválido.", nameof(port));

            if (zpl == null) zpl = string.Empty;

            using var client = new TcpClient();
            await client.ConnectAsync(ip, port);

            using NetworkStream ns = client.GetStream();
            byte[] bytes = Encoding.ASCII.GetBytes(zpl);

            await ns.WriteAsync(bytes, 0, bytes.Length);
            await ns.FlushAsync();
        }
    }
}