using RecipeControl.Services.Interfaces;
using RecipeControl.Services.Scales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Services.Host
{
    /// <summary>
    /// Hosted service that manages the lifecycle of scales.
    /// </summary>
    public class ScaleManagerHostedService : IHostedService
    {
        private readonly IScaleFactory _scaleFactory;
        public IReadOnlyList<IScale> Scales { get; private set; }

        public ScaleManagerHostedService(IScaleFactory scaleFactory)
        {
            _scaleFactory = scaleFactory;

            // Create all scales using the factory
            Scales = _scaleFactory.CreateAll().ToList();
        }

        /// <summary>
        /// Start the hosted service by connecting to all scales.
        /// </summary>
        /// <returns></returns>
        public async Task StartAsync()
        {
            foreach (var scale in Scales)
            {
                await scale.ConnectAsync();
            }
        }

        /// <summary>
        /// Stop the hosted service by disconnecting from all scales.
        /// </summary>
        /// <returns></returns>
        public async Task StopAsync()
        {
            foreach (var scale in Scales)
            {
                await scale.DisconnectAsync();
            }
        }
    }
}
