using Microsoft.Extensions.Configuration;
using RecipeControl.Models.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace RecipeControl.Configuration
{
    /// <summary>
    /// Main configuration class that maps appsettings.json
    /// </summary>
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; } = new();
        public SerialPortSettings SerialPort { get; set; } = new();
        public ScaleEthernetPorts ScaleEthernetPorts { get; set; } = new();
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

    public class ScaleEthernetPorts
    {
        public int Amount { get; set; } = 0;
        public IEnumerable<EthernetScaleConfig> Ports { get; set; } = Array.Empty<EthernetScaleConfig>();
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

        /// <summary>
        /// Get Parity value as enum
        /// </summary>
        /// <returns></returns>
        public System.IO.Ports.Parity GetParity()
        {
            return Enum.Parse<System.IO.Ports.Parity>(Parity, true);
        }

        /// <summary>
        /// Get StopBits value as enum
        /// </summary>
        /// <returns></returns>
        public System.IO.Ports.StopBits GetStopBits()
        {
            return Enum.Parse<System.IO.Ports.StopBits>(StopBits, true);
        }
    }

    public class  ReportSettings
    {
        public string ExportPath { get; set; } = string.Empty;
        public string FileDateFormat { get; set; } = string.Empty;
        public string DefaultExportFormat { get; set; } = "Excel";
        public bool IncludeGraphs { get; set; } = true;
    }
}
