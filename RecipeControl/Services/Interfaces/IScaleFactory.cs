using RecipeControl.Services.Scales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Services.Interfaces
{
    public interface IScaleFactory
    {
        IEnumerable<IScale> CreateAll();
    }
}
