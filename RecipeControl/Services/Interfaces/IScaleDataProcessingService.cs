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
        /// <summary>
        /// Build weight request message
        /// </summary>
        /// <returns></returns>
        byte[] BuildWeightRequest();

        /// <summary>
        /// Parse weight response message
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        decimal ParseWeightResponse(byte[] response);
    }
}
