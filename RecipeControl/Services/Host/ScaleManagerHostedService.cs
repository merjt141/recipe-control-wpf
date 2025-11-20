using RecipeControl.Services.Interfaces;
using RecipeControl.Services.Scales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Services.Host
{
    public class ScaleManagerHostedService : IHostedService
    {
        private readonly IScaleFactory _scaleFactory;
        public IReadOnlyList<IScale> Scales { get; private set; }

        public ScaleManagerHostedService(IScaleFactory scaleFactory)
        {
            _scaleFactory = scaleFactory;
        }

        public async Task StartAsync()
        {
            Scales = _scaleFactory.CreateAll().ToList();

            foreach (var scale in Scales)
            {
                await scale.ConnectAsync();
            }
        }

        public async Task StopAsync()
        {
            foreach (var scale in Scales)
            {
                await scale.DisconnectAsync();
            }
        }
    }
}
