using Microsoft.VisualBasic;
using RecipeControl.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Services.Protocols
{
    public class ASCIIProtocol : IScaleDataProcessingService
    {
        public VariantType BuildWeightRequest()
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (NotImplementedException ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }
        }

        public decimal ParseWeightResponse(VariantType response)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (NotImplementedException ex)
            {
                Debug.WriteLine(ex.Message);
                return (decimal)(new Random().NextDouble()) * 20m;
            }
        }
    }
}
