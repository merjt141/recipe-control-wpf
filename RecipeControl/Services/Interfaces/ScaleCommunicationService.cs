using Microsoft.Identity.Client;
using Microsoft.VisualBasic;
using RecipeControl.Models.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Services.Interfaces
{
    public class ScaleCommunicationService : IScaleCommunicationService
    {
        private readonly EthernetScaleConfig _ethernetScaleConfig;

        private bool _isOnline = false;
        private TcpClient? _client;
        private NetworkStream? _networkStream;

        public ScaleCommunicationService(EthernetScaleConfig ethernetScaleConfig)
        {
            _ethernetScaleConfig = ethernetScaleConfig;
        }

        /// <summary>
        /// Connect to the scale
        /// </summary>
        /// <returns></returns>
        public async Task ConnectAsync()
        {
            _client = new TcpClient();

            await _client.ConnectAsync(_ethernetScaleConfig.IPAddress, _ethernetScaleConfig.Port);
            _client.ReceiveTimeout = 1000;
            _networkStream = _client.GetStream();

            _isOnline = true;
        }

        /// <summary>
        /// Disconnect from the scale
        /// </summary>
        /// <returns></returns>
        public async Task DisconnectAsync()
        {
            // Dispose network stream and close client
            if (_networkStream != null)
            {
                await _networkStream.DisposeAsync();
                _networkStream = null;
            }

            // Close the TCP client
            if (_client != null)
            {
                _client.Close();
                _client = null;
            }

            _isOnline = false;
        }

        /// <summary>
        /// Retrieve if the scale is online
        /// </summary>
        /// <returns></returns>
        public bool IsOnline()
        {
            return _isOnline;
        }

        /// <summary>
        /// Logic to send and receive buffer of bytes from scale
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<byte[]> SendAndReceiveAsync(byte[] request)
        {
            try // Using only during simulation, to test parsing logic when there is no scale
            {
                // Check if scale is online
                if (_networkStream == null)
                {
                    _isOnline = false;
                    throw new InvalidOperationException("Scale is not connected");
                }

                // Write request network stream of the scale
                await _networkStream.WriteAsync(request, 0, request.Length);

                // Read response of network stream of the scale
                var response = new byte[256];
                int bytesRead = await _networkStream.ReadAsync(response, 0, response.Length);

                return response;
            }
            catch (InvalidOperationException)
            {
                decimal simulatedWeight = (decimal)Math.Round(new Random().NextDouble() + 10, 2);
                Debug.WriteLine($"Error on connection, returning simulated value: {simulatedWeight}");
                return Encoding.ASCII.GetBytes($"WR:{simulatedWeight}");
            }
        }
    }
}
