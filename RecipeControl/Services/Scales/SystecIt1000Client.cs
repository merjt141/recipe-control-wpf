using System;
using System.Globalization;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

public sealed class SystecIt1000Client
{
    private readonly string _ip;
    private readonly int _port;

    public SystecIt1000Client(string ip, int port)
    {
        _ip = ip;
        _port = port;
    }

    public async Task<decimal> ReadNetWeightAsync(bool settled, int scaleNo, CancellationToken ct)
    {
        string cmd = (settled ? "RN" : "RM") + scaleNo.ToString(CultureInfo.InvariantCulture) + "\r";

        using var client = new TcpClient { NoDelay = true };
        await client.ConnectAsync(_ip, _port, ct);

        using NetworkStream ns = client.GetStream();

        byte[] tx = Encoding.ASCII.GetBytes(cmd);
        await ns.WriteAsync(tx, 0, tx.Length, ct);
        await ns.FlushAsync(ct);

        string resp = await ReadAsciiAsync(ns, expectedMinChars: 40, ct: ct);

        resp = resp.Trim('\n', '\r');

        if (resp.Length < 2)
            throw new InvalidOperationException("Respuesta vacía o incompleta desde IT1000.");

        // Si tu equipo devuelve "00" como OK, esto te sirve.
        // Si ves otro formato real, me pasas el string y lo ajusto.
        string err = resp.Substring(0, 2);
        if (err != "00")
            throw new InvalidOperationException($"IT1000 devolvió error: {err}. Resp=[{resp}]");

        return ParseNet(resp);
    }

    private static async Task<string> ReadAsciiAsync(NetworkStream ns, int expectedMinChars, CancellationToken ct)
    {
        var sb = new StringBuilder();
        var buffer = new byte[256];

        while (sb.Length < expectedMinChars)
        {
            int n = await ns.ReadAsync(buffer, 0, buffer.Length, ct);
            if (n <= 0) break;

            sb.Append(Encoding.ASCII.GetString(buffer, 0, n));

            if (!ns.DataAvailable && sb.Length >= expectedMinChars)
                break;
        }

        return sb.ToString();
    }

    private static decimal ParseNet(string resp)
    {
        // Método “tolerante”: toma el último número decimal de la respuesta.
        // Si me pasas la respuesta real, lo dejo fijo por campos.
        var matches = Regex.Matches(resp, @"(-?\d+[.,]\d+)", RegexOptions.IgnoreCase);
        if (matches.Count == 0)
            throw new InvalidOperationException($"No se encontró número en la respuesta: [{resp}]");

        string num = matches[^1].Value.Replace(',', '.');

        if (!decimal.TryParse(num, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
            throw new InvalidOperationException($"No pude convertir el peso: '{num}'. Resp=[{resp}]");

        return value;
    }
}