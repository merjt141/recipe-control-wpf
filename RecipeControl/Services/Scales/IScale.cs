using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Services.Scales
{
    public interface IScale
    {
        Task<bool> ConnectAsync();
        Task<decimal> GetCurrentWeightAsync();
    }
}
