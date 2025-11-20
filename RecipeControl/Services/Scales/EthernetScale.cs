using RecipeControl.Models.Config;
using RecipeControl.Services.Interfaces;
using RecipeControl.Services.Scales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace RecipeControl.Services.Scales
{
    public class EthernetScale : IScale
    {
        private readonly IScaleCommunicationService _scaleCommunicationService;
        private readonly IScaleDataProcessingService _scaleDataProcessingService;
        private readonly EthernetScaleConfig _ethernetScaleConfig;

        public EthernetScale(IScaleCommunicationService scaleCommunicationService,
                     IScaleDataProcessingService scaleDataProcessingService,
                     EthernetScaleConfig ethernetScaleConfig)
        {
            _scaleCommunicationService = scaleCommunicationService;
            _scaleDataProcessingService = scaleDataProcessingService;
            _ethernetScaleConfig = ethernetScaleConfig;
        }

        public async Task<bool> ConnectAsync()
        {
            await Task.Delay(500);
            return true;
        }

        public async Task<bool> DisconnectAsync()
        {
            await Task.Delay(500);
            return true;
        }

        public async Task<decimal> GetCurrentWeightAsync()
        {
            var request = _scaleDataProcessingService.BuildWeightRequest();
            var response = await _scaleCommunicationService.SendAndReceiveAsync(request);
            decimal weight = _scaleDataProcessingService.ParseWeightResponse(response);
            return weight;
        }

        public async Task<string> GetInfo()
        {
            await Task.Delay(500);
            return _ethernetScaleConfig.IPAddress + ":" + _ethernetScaleConfig.Port;
        }
    }
}
