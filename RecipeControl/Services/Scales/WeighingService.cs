using RecipeControl.Services.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Services.Scales
{
    public class WeighingService : IWeighingService
    {
        private readonly ScaleManagerHostedService _scaleManagerHostedService;

        public WeighingService(ScaleManagerHostedService scaleManagerHostedService)
        {
            _scaleManagerHostedService = scaleManagerHostedService;
        }

        public Task<string> GetScaleInfo(int scaleIndex)
        {
            if (scaleIndex < 0 || scaleIndex >= _scaleManagerHostedService.Scales.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(scaleIndex), "Invalid scale index.");
            }
            var scale = _scaleManagerHostedService.Scales[scaleIndex];
            return scale.GetInfo();
        }

        public decimal GetScaleWeight(int scaleIndex)
        {
            if (scaleIndex < 0 || scaleIndex >= _scaleManagerHostedService.Scales.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(scaleIndex), "Invalid scale index.");
            }
            var scale = _scaleManagerHostedService.Scales[scaleIndex];
            var weightTask = scale.GetCurrentWeightAsync();
            weightTask.Wait(); // Blocking wait for simplicity; consider using async/await in real implementations
            return weightTask.Result;
        }
    }
}
