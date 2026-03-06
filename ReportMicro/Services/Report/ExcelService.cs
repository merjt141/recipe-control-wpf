using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Globalization;

using static ReportMicro.Models.DTOs;
using Excel = Microsoft.Office.Interop.Excel;

namespace ReportMicro.Services.Report
{
    public static class ExcelService
    {
        private static readonly string defaultPath = ReportMicro.Configuration.ConfigurationManager.Instance.Settings.Reports.ExportPath;

        /// <summary>
        /// Genera reporte de la variable interna que contiene
        /// </summary>
        /// <param name="reportDataGridDTO"></param>
        /// <returns></returns>
        public static async Task<byte[]> GenerateReportAsyc(IEnumerable<ReportDataGridDTO> reportDataGridDTO)
        {
            var excelApp = new Excel.Application();
            Excel.Workbook workbook = excelApp.Workbooks.Add();
            Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets[1];

            worksheet.Cells[1, 1] = "Id";
            worksheet.Cells[1, 2] = "Código Receta (SAP)";
            worksheet.Cells[1, 3] = "Lote";
            worksheet.Cells[1, 4] = "Usuario";
            worksheet.Cells[1, 5] = "Fecha Registro";
            worksheet.Cells[1, 6] = "Id Bolsa";
            worksheet.Cells[1, 7] = "Fecha Pesado";
            worksheet.Cells[1, 8] = "Código Insumo (SAP)";
            worksheet.Cells[1, 9] = "Peso SP";
            worksheet.Cells[1, 10] = "Peso Real";

            int row = 2;
            foreach (ReportDataGridDTO dto in reportDataGridDTO)
            {
                worksheet.Cells[row, 1] = dto.BatchRegistroId;
                worksheet.Cells[row, 2] = dto.RecetaCodigo;
                worksheet.Cells[row, 3] = dto.Lote;
                worksheet.Cells[row, 4] = dto.Usuario;
                worksheet.Cells[row, 5] = dto.FechaPreparacion;
                worksheet.Cells[row, 6] = dto.MacroRegistroId;
                worksheet.Cells[row, 7] = dto.MacroRegistroFechaCreacion;
                worksheet.Cells[row, 8] = dto.InsumoCodigo;
                worksheet.Cells[row, 9] = dto.PesoObjetivo;
                worksheet.Cells[row, 10] = dto.PesoReal;
                row++;
            }

            string tempFilePath = defaultPath + $"\\RegistroBatchReport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            // Crear directorio si no existe
            Directory.CreateDirectory(defaultPath);

            // Guardar el archivo Excel en el directorio con el nombre asignado
            try
            {
                await Task.Run(() => workbook.SaveAs2(tempFilePath));
                MessageBox.Show($"Archivo guardado en {tempFilePath}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ah ocurrido un error en el guardado del archivo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            workbook.Close();
            excelApp.Quit();

            byte[] fileBytes = System.IO.File.ReadAllBytes(tempFilePath);
            return fileBytes;
        }

        public static async Task<byte[]> GenerateReportAsyc2(IEnumerable<ReportDataGrid2DTO> reportDataGridDTO)
        {

            if (reportDataGridDTO == null)
                return Array.Empty<byte>();

            // Asegura carpeta destino
            Directory.CreateDirectory(defaultPath);

            string tempFilePath = Path.Combine(
                defaultPath,
                $"RegistroBatchReport2_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            );

            try
            {
                // ClosedXML trabaja en memoria; lo envolvemos en Task.Run para no bloquear UI
                byte[] bytes = await Task.Run(() =>
                {
                    using var workbook = new XLWorkbook();
                    var ws = workbook.Worksheets.Add("Reporte");

                    // -------------------------
                    // ENCABEZADOS
                    // -------------------------
                    ws.Cell(1, 1).Value = "Batch";
                    ws.Cell(1, 2).Value = "Cod. Único";
                    ws.Cell(1, 3).Value = "Fecha QR";
                    ws.Cell(1, 4).Value = "Fecha Pistoleo";
                    ws.Cell(1, 5).Value = "Usuario";
                    ws.Cell(1, 6).Value = "Lote";
                    ws.Cell(1, 7).Value = "Peso SP (g)";
                    ws.Cell(1, 8).Value = "Peso Real (g)";

                    // INSUMO 1
                    ws.Cell(1, 9).Value = "Ins1 Id";
                    ws.Cell(1, 10).Value = "Ins1 SAP";
                    ws.Cell(1, 11).Value = "Ins1 Descripción";

                    // INSUMO 2
                    ws.Cell(1, 12).Value = "Ins2 Id";
                    ws.Cell(1, 13).Value = "Ins2 SAP";
                    ws.Cell(1, 14).Value = "Ins2 Descripción";

                    // INSUMO 3
                    ws.Cell(1, 15).Value = "Ins3 Id";
                    ws.Cell(1, 16).Value = "Ins3 SAP";
                    ws.Cell(1, 17).Value = "Ins3 Descripción";

                    // INSUMO 4
                    ws.Cell(1, 18).Value = "Ins4 Id";
                    ws.Cell(1, 19).Value = "Ins4 SAP";
                    ws.Cell(1, 20).Value = "Ins4 Descripción";

                    // Estilo header
                    var header = ws.Range(1, 1, 1, 20);
                    header.Style.Font.Bold = true;
                    header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    header.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    // Congelar fila 1
                    ws.SheetView.FreezeRows(1);

                    // -------------------------
                    // DATA
                    // -------------------------
                    int row = 2;
                    foreach (var dto in reportDataGridDTO)
                    {
                        ws.Cell(row, 1).Value = dto.BatchRegistroId;
                        ws.Cell(row, 2).Value = dto.MacroRegistroId;

                        // Fechas con formato
                        ws.Cell(row, 3).Value = dto.Fecha;
                        ws.Cell(row, 3).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";

                        if (dto.FechaPistoleo.HasValue)
                        {
                            ws.Cell(row, 4).Value = dto.FechaPistoleo.Value;
                            ws.Cell(row, 4).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";
                        }
                        else
                        {
                            ws.Cell(row, 4).Value = ""; // vacío si null
                        }

                        ws.Cell(row, 5).Value = dto.UsuarioStr ?? "";
                        ws.Cell(row, 6).Value = dto.LoteStr ?? "";

                        ws.Cell(row, 7).Value = dto.PesoObj;
                        ws.Cell(row, 8).Value = dto.PesoReal;

                        // 2 decimales a pesos
                        ws.Cell(row, 7).Style.NumberFormat.Format = "0.00";
                        ws.Cell(row, 8).Style.NumberFormat.Format = "0.00";

                        // INSUMO 1
                        ws.Cell(row, 9).Value = dto.Insumo1Strid ?? "";
                        ws.Cell(row, 10).Value = dto.Insumo1StrSAP ?? "";
                        ws.Cell(row, 11).Value = dto.Insumo1StrDescr ?? "";

                        // INSUMO 2
                        ws.Cell(row, 12).Value = dto.Insumo2Strid ?? "";
                        ws.Cell(row, 13).Value = dto.Insumo2StrSAP ?? "";
                        ws.Cell(row, 14).Value = dto.Insumo2StrDescr ?? "";

                        // INSUMO 3
                        ws.Cell(row, 15).Value = dto.Insumo3Strid ?? "";
                        ws.Cell(row, 16).Value = dto.Insumo3StrSAP ?? "";
                        ws.Cell(row, 17).Value = dto.Insumo3StrDescr ?? "";

                        // INSUMO 4
                        ws.Cell(row, 18).Value = dto.Insumo4Strid ?? "";
                        ws.Cell(row, 19).Value = dto.Insumo4StrSAP ?? "";
                        ws.Cell(row, 20).Value = dto.Insumo4StrDescr ?? "";

                        row++;
                    }

                    // Autofiltro y ajuste de columnas
                    ws.RangeUsed().SetAutoFilter();
                    ws.Columns(1, 20).AdjustToContents();

                    // Guardar a disco
                    workbook.SaveAs(tempFilePath);

                    // Retornar bytes
                    return File.ReadAllBytes(tempFilePath);
                });

                MessageBox.Show($"Archivo guardado en:\n{tempFilePath}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                return bytes;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generando Excel (ClosedXML):\n{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return Array.Empty<byte>();
            }
        }
    }
}
