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
        Task<bool> DisconnectAsync();
        bool IsOnline();
        Task<decimal> GetCurrentWeightAsync();
        Task<string> GetInfo();
    }
}
