using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace RecipeControl.Services.Reports
{
    public class ExcelService : IExcelService
    {
        private Excel.Workbook CreateNewWorkBook()
        {
            var excelApp = new Excel.Application();
            Excel.Workbook workbook = excelApp.Workbooks.Add();
            return workbook;
        }

        private Excel.Worksheet GetFirstWorksheet(Excel.Workbook workbook)
        {
            return (Excel.Worksheet)workbook.Sheets[1];
        }

        public bool Test(string exportPath)
        {
            // Implementation for generating report by name without parameters
            try
            {
                var excelApp = new Excel.Application();

                Excel.Workbook workbook = excelApp.Workbooks.Add();
                Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets[1];

                worksheet.Cells[1, 1] = "Hello, World!";

                // Save the workbook to a temporary file
                string tempFilePath = exportPath + $"\\TestReport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                workbook.SaveAs(tempFilePath);
                workbook.Close();
                excelApp.Quit();
                return true;
            }
            catch (Exception ex)
            {
                // Handle exceptions
                throw new Exception("Error generating report", ex);
            }
        }
    }
}
