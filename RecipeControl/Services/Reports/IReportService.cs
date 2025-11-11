using RecipeControl.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Services.Reports
{
    public interface IReportService
    {
        Task<byte[]> GenerateReportAsync(int reportId, Dictionary<string, object> parameters);
        Task<byte[]> GenerateReportAsync(string reportName, Dictionary<string, object> parameters);
        Task<byte[]> GenerateReportAsync(IEnumerable<RegistroBatchDTO> registroBatchDTOs);
        bool Test();
    }
}
