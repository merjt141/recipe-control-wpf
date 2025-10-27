using Microsoft.Extensions.DependencyInjection;
using RecipeControl.Models.Config;
using RecipeControl.Services.Interfaces;
using RecipeControl.Services.Scales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace RecipeControl.Services.Scales
{
    public class ScaleFactory : IScaleFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEnumerable<EthernetScaleConfig> _ethernetScaleConfigs;
        public ScaleFactory(IServiceProvider serviceProvider, IEnumerable<EthernetScaleConfig> ethernetScaleConfigs)
        {
            _serviceProvider = serviceProvider;
            _ethernetScaleConfigs = ethernetScaleConfigs;
        }
        public IEnumerable<IScale> CreateAll()
        {
            foreach (var config in _ethernetScaleConfigs)
            {
                yield return ActivatorUtilities.CreateInstance<EthernetScale>(_serviceProvider, config);
            }
        }
    }
}
