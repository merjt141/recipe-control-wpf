using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Services.Scales
{
    public interface IWeighingService
    {
        Task<string> GetScaleInfo(int scaleIndex);
        decimal GetScaleWeight(int scaleIndex);
    }
}
