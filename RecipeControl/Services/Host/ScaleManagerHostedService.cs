using RecipeControl.Services.Interfaces;
using RecipeControl.Services.Scales;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var tasks = new List<Task>();
            foreach (var scale in Scales)
            {
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        await scale.StartAsync();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error connecting to scale {scale.GetInfo()}: {ex.Message}");
                    }
                }));
            }

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Stop the hosted service by disconnecting from all scales.
        /// </summary>
        /// <returns></returns>
        public async Task StopAsync()
        {
            foreach (var scale in Scales)
            {
                try
                {
                    await scale.StopAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error disconnecting from scale {scale.GetInfo()}: {ex.Message}");
                }
            }
        }
    }
}
