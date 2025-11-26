using RecipeControl.Models.Config;
using RecipeControl.Services.Interfaces;
using RecipeControl.Services.Scales;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace RecipeControl.Services.Scales
{
    /// <summary>
    /// Ethernet scale implementation
    /// </summary>
    public class EthernetScale : IScale
    {
        private readonly IScaleCommunicationService _scaleCommunicationService;
        private readonly IScaleDataProcessingService _scaleDataProcessingService;
        private readonly EthernetScaleConfig _ethernetScaleConfig;  

        public EthernetScale(IScaleDataProcessingService scaleDataProcessingService,
                     EthernetScaleConfig ethernetScaleConfig)
        {
            _scaleDataProcessingService = scaleDataProcessingService;
            _ethernetScaleConfig = ethernetScaleConfig;

            _scaleCommunicationService = new ScaleCommunicationService(_ethernetScaleConfig);
        }

        /// <summary>
        /// Connect to the scale
        /// </summary>
        /// <returns></returns>
        public async Task StartAsync()
        {
            await _scaleCommunicationService.ConnectAsync();
        }

        /// <summary>
        /// Disconnect from the scale
        /// </summary>
        /// <returns></returns>
        public async Task StopAsync()
        {
            await _scaleCommunicationService.DisconnectAsync();
        }

        /// <summary>
        /// Retrieve if the scale is online
        /// </summary>
        /// <returns></returns>
        public bool IsOnline()
        {
            return _scaleCommunicationService.IsOnline();
        }

        /// <summary>
        /// Retrieve current weight from the scale
        /// </summary>
        /// <returns></returns>
        public async Task<decimal> GetCurrentWeightAsync()
        {
            byte[] request = _scaleDataProcessingService.BuildWeightRequest();
            byte[] response = await _scaleCommunicationService.SendAndReceiveAsync(request);
            decimal weight = _scaleDataProcessingService.ParseWeightResponse(response);
            return weight;
        }

        /// <summary>
        /// Return information about the scale
        /// </summary>
        /// <returns></returns>
        public string GetInfo()
        {
            return _ethernetScaleConfig.IPAddress + ":" + _ethernetScaleConfig.Port;
        }
    }
}
