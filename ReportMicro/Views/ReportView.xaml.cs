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

        #region Eventos de UI
        private void BotonActualizar_Click_1(object sender, RoutedEventArgs e)
        {
            _ = BuildReportDataTablePreview();
        }

        private void BotonReporte_Click(object sender, RoutedEventArgs e)
        {
            // Genera reporte de la información almacenada en memoria
            _ = ExcelService.GenerateReportAsyc2(this.MicroReportData2);
        }

        private void ReportDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        #endregion

        #region Database Application

        private async Task BuildReportDataTablePreview()
        {
            try
            {
                //if (FechaInicial.SelectedDate is null || FechaFinal is null) return;

                DateTime fechaInicial = FechaInicial.SelectedDate ?? DateTime.Now;
                DateTime fechaFinal = FechaFinal.SelectedDate ?? DateTime.Now.AddDays(-1);

                var lista = await DatabaseService.BuildReportDataTable2Preview(fechaInicial, fechaFinal);

                // Guardar en memoria de code-behind
                MicroReportData2 = lista;
                ReportDataGrid2.ItemsSource = lista.Take(10);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error actualizando la interfaz de la BD: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

    }
}
