using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Services.Scales
{
    public interface IScale
    {
        /// <summary>
        /// Connect to the scale
        /// </summary>
        /// <returns></returns>
        Task StartAsync();

        /// <summary>
        /// Disconnect from the scale
        /// </summary>
        /// <returns></returns>
        Task StopAsync();

        /// <summary>
        /// Retrieve if the scale is online
        /// </summary>
        /// <returns></returns>
        bool IsOnline();

        /// <summary>
        /// Retrieve current weight from the scale
        /// </summary>
        /// <returns></returns>
        Task<decimal> GetCurrentWeightAsync();

        /// <summary>
        /// Return information about the scale
        /// </summary>
        /// <returns></returns>
        string GetInfo();
    }
}
