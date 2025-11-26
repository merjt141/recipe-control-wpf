using RecipeControl.Services.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Services.Scales
{
    /// <summary>
    /// Weighing service to manage scales
    /// </summary>
    public class WeighingService : IWeighingService
    {
        private readonly ScaleManagerHostedService _scaleManagerHostedService;

        public WeighingService(ScaleManagerHostedService scaleManagerHostedService)
        {
            _scaleManagerHostedService = scaleManagerHostedService;
        }

        /// <summary>
        /// Start weighing service and all scales
        /// </summary>
        /// <returns></returns>
        public Task StartService()
        {
            return _scaleManagerHostedService.StartAsync();
        }

        /// <summary>
        /// Stop weighing service and all scales
        /// </summary>
        /// <returns></returns>
        public Task StopService()
        {
            return _scaleManagerHostedService.StopAsync();
        }

        /// <summary>
        /// Return information about a specific scale
        /// </summary>
        /// <param name="scaleIndex">Scale index from 0,1,2,...</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public string GetScaleInfo(int scaleIndex)
        {
            if (scaleIndex < 0 || scaleIndex >= _scaleManagerHostedService.Scales.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(scaleIndex), "Invalid scale index.");
            }
            IScale scale = _scaleManagerHostedService.Scales[scaleIndex];

            return scale.GetInfo();
        }

        /// <summary>
        /// Retrieve current weight from a specific scale
        /// </summary>
        /// <param name="scaleIndex"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<decimal> GetScaleWeightAsync(int scaleIndex)
        {
            // Validate scale index
            if (scaleIndex < 0 || scaleIndex >= _scaleManagerHostedService.Scales.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(scaleIndex), "Invalid scale index.");
            }

            // Get the scale
            IScale scale = _scaleManagerHostedService.Scales[scaleIndex];

            return await scale.GetCurrentWeightAsync();
        }
    }
}
