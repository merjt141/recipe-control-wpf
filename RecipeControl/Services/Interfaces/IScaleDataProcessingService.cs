using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Services.Interfaces
{
    public interface IScaleDataProcessingService
    {
        VariantType BuildWeightRequest();
        decimal ParseWeightResponse(VariantType response);
    }
}
