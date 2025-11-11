using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Services.Reports
{
    public interface IExcelService
    {
        bool Test(string exportPath);
    }
}
