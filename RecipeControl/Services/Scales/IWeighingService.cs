using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Services.Scales
{
    public interface IWeighingService
    {
        Task StartService();
        Task StopService();
        string GetScaleInfo(int scaleIndex);
        Task<decimal> GetScaleWeightAsync(int scaleIndex);
    }
}
