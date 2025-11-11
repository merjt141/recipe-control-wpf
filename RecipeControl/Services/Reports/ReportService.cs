using RecipeControl.Configuration;
using RecipeControl.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace RecipeControl.Services.Reports
{
    public class ReportService : IReportService
    {
        private readonly ReportSettings _reportSettings;
        private readonly IExcelService _excelService;
        private string _exportPath;

        public ReportService(
            ReportSettings reportSettings,
            IExcelService excelService)
        {
            _reportSettings = reportSettings;
            _excelService = excelService;

            _exportPath = _reportSettings.ExportPath;
        }

        public Task<byte[]> GenerateReportAsync(int reportId, Dictionary<string, object> parameters)
        {
            // Implementation for generating report by ID
            throw new NotImplementedException();
        }
        public Task<byte[]> GenerateReportAsync(string reportName, Dictionary<string, object> parameters)
        {
            // Implementation for generating report by name with parameters
            throw new NotImplementedException();
        }
        public async Task<byte[]> GenerateReportAsync(IEnumerable<RegistroBatchDTO> registroBatchDTOs)
        {
            var excelApp = new Excel.Application();
            Excel.Workbook workbook = excelApp.Workbooks.Add();
            Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets[1];

            worksheet.Cells[1, 1] = "RegistroBatchWarehouseId";
            worksheet.Cells[1, 2] = "Lote";
            worksheet.Cells[1, 3] = "FormulaId";
            worksheet.Cells[1, 4] = "FormulaCodigo";
            worksheet.Cells[1, 5] = "RecetaId";
            worksheet.Cells[1, 6] = "RecetaCodigo";
            worksheet.Cells[1, 7] = "InsumoId";
            worksheet.Cells[1, 8] = "Usuario";
            worksheet.Cells[1, 9] = "InsumoCodigo";
            worksheet.Cells[1, 10] = "DetalleRecetaValor";
            worksheet.Cells[1, 11] = "InsumoUnidad";
            worksheet.Cells[1, 12] = "DetalleRecetaVariacion";
            worksheet.Cells[1, 13] = "RegistroPesoId";
            worksheet.Cells[1, 14] = "RegistroPesoCodigo";
            worksheet.Cells[1, 15] = "RegistroPesoValor";
            worksheet.Cells[1, 16] = "RegistroBatchWarehouseValorReal";
            worksheet.Cells[1, 17] = "RegistroBatchFechaPreparacion";
            int row = 2;

            // Implementation for generating a default report
            foreach (RegistroBatchDTO dto in registroBatchDTOs)
            {
                // Process each RegistroBatchDTO as needed
                worksheet.Cells[row, 1] = dto.RegistroBatchWarehouseId;
                worksheet.Cells[row, 2] = dto.Lote;

                worksheet.Cells[row, 3] = dto.FormulaId;
                worksheet.Cells[row, 4] = dto.FormulaCodigo;
                worksheet.Cells[row, 5] = dto.RecetaId;
                worksheet.Cells[row, 6] = dto.RecetaCodigo;
                worksheet.Cells[row, 7] = dto.InsumoId;
                worksheet.Cells[row, 8] = dto.Usuario;
                worksheet.Cells[row, 9] = dto.InsumoCodigo;
                worksheet.Cells[row, 10] = dto.DetalleRecetaValor;
                worksheet.Cells[row, 11] = dto.InsumoUnidad;
                worksheet.Cells[row, 12] = dto.DetalleRecetaVariacion;
                worksheet.Cells[row, 13] = dto.RegistroPesoId;
                worksheet.Cells[row, 14] = dto.RegistroPesoCodigo;
                worksheet.Cells[row, 15] = dto.RegistroPesoValor;
                worksheet.Cells[row, 16] = dto.RegistroBatchWarehouseValorReal;
                worksheet.Cells[row, 17] = dto.RegistroBatchFechaPreparacion;
                row++;

            }
            // Save the workbook to a temporary file
            string tempFilePath = _exportPath + $"\\RegistroBatchReport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            workbook.SaveAs(tempFilePath);
            workbook.Close();
            excelApp.Quit();

            // Read the file into a byte array
            byte[] fileBytes = System.IO.File.ReadAllBytes(tempFilePath);
            return fileBytes;
        }
        public bool Test()
        {
            return _excelService.Test(_exportPath);
        }
    }
}
