using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace ReportMicro.Configuration
{
    /// <summary>
    /// Main configuration class that maps appsettings.json
    /// </summary>
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; } = new();
        public SerialPortSettings SerialPort { get; set; } = new();
        public DatabaseSettings Database {  get; set; } = new();
        public ReportSettings Reports { get; set; } = new();
    }

    public class ConnectionStrings
    {
        public string DefaultConnection { get; set; } = string.Empty;
        public string BackupConnection {  get; set; } = string.Empty;
    }

    public class DatabaseSettings
    {
        public int CommandTimeout { get; set; } = 30;
        public int MaxRetryCount { get; set; } = 3;
        public int RetryDelay { get; set; } = 2000;
        public bool EnablePooling { get; set; } = true;
        public int MinPoolSize { get; set; } = 5;
        public int MaxPoolSize { get; set; } = 100;

    }

    public class SerialPortSettings
    {
        public string PortName {  get; set; } = string.Empty;
        public int BaudRate { get; set; } = 9600;
        public int DataBits { get; set; } = 8;
        public string Parity { get; set; } = string.Empty;
        public string StopBits {  get; set; } = string.Empty;
        public int ReadTimeout { get; set; } = 5000;
        public int WriteTimeout { get; set; } = 5000;
    }

    public class ReportSettings
    {
        public string ExportPath { get; set; } = string.Empty;
        public string FileDateFormat { get; set; } = string.Empty;
        public string DefaultExportFormat { get; set; } = "Excel";
        public bool IncludeGraphs { get; set; } = true;
    }
}
