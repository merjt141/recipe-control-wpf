using Microsoft.Data.SqlClient;
using ReportMicro.Models;
using ReportMicro.Services.Database;
using ReportMicro.Services.Report;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static ReportMicro.Models.DTOs;

namespace ReportMicro.Views
{
    /// <summary>
    /// Lógica de interacción para ReportView.xaml
    /// </summary>
    public partial class ReportView : Window
    {
        public List<ReportDataGridDTO> MicroReportData = new List<ReportDataGridDTO>();
        public List<ReportDataGrid2DTO> MicroReportData2 = new List<ReportDataGrid2DTO>();

        public ReportView()
        {
            InitializeComponent();

            // Valores de inicio de los filtros
            FechaFinal.SelectedDate = DateTime.Now;
            FechaInicial.SelectedDate = FechaFinal.SelectedDate?.AddDays(-1);
        }

        private void BotonActualizar_Click_1(object sender, RoutedEventArgs e)
        {
            _ = BuildReportDataTablePreview();
        }

        private void BotonReporte_Click(object sender, RoutedEventArgs e)
        {
            // Genera reporte de la información almacenada en memoria
            _ = ExcelService.GenerateReportAsyc2(this.MicroReportData2);
        }

        #region Database Application

        private async Task BuildReportDataTablePreview()
        {
            if (FechaInicial.SelectedDate is null || FechaFinal is null) return;

            DateTime fechaInicial = FechaInicial.SelectedDate ?? DateTime.Now;
            DateTime fechaFinal = FechaFinal.SelectedDate ?? DateTime.Now.AddDays(-1);

            var lista = await DatabaseService.BuildReportDataTable2Preview(fechaInicial, fechaFinal);

            // Guardar en memoria de code-behind
            MicroReportData2 = lista;
            ReportDataGrid2.ItemsSource = lista.Take(10);
        }

        #endregion

        private void ReportDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
