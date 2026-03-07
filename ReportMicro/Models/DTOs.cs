using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportMicro.Models
{
    public static class DTOs
    {
        public class ReportDataGridDTO
        {
            public int BatchRegistroId { get; set; }
            public string RecetaCodigo { get; set; } = string.Empty;
            public int Lote { get; set; }
            public string Usuario { get; set; } = string.Empty;
            public DateTime FechaPreparacion { get; set; }
            public int MacroRegistroId { get; set; }
            public DateTime MacroRegistroFechaCreacion { get; set; }
            public string? InsumoCodigo { get; set; }
            public decimal PesoObjetivo { get; set; }
            public decimal PesoReal { get; set; }
        }

        public class ReportDataGrid2DTO
        {
            // --- Identificadores ---
            public int BatchRegistroId { get; set; }  //batch generado por 
            public int MacroRegistroId { get; set; }  //coodigo unido

            // --- Fechas ---
            public DateTime Fecha { get; set; }                 // Fecha QR
            public DateTime? FechaPistoleo { get; set; }        // Puede ser null

            // --- Información operativa ---
            public string UsuarioStr { get; set; } = string.Empty;
            public string LoteStr { get; set; } = string.Empty; // numeord  elote escrito en la etapa de pesado inicial.

            // --- Pesos en gramos ---
            public decimal PesoObj { get; set; }
            public decimal PesoReal { get; set; }

            // --- Insumos SAP ---

            public string Insumo1Strid { get; set; } = string.Empty;
            public string Insumo1StrSAP { get; set; } = string.Empty;
            public string Insumo1StrDescr { get; set; } = string.Empty;
            public string Insumo2Strid { get; set; } = string.Empty;
            public string Insumo2StrSAP { get; set; } = string.Empty;
            public string Insumo2StrDescr { get; set; } = string.Empty;
            public string Insumo3Strid { get; set; } = string.Empty;
            public string Insumo3StrSAP { get; set; } = string.Empty;
            public string Insumo3StrDescr { get; set; } = string.Empty;
            public string Insumo4Strid { get; set; } = string.Empty;
            public string Insumo4StrSAP { get; set; } = string.Empty;
            public string Insumo4StrDescr { get; set; } = string.Empty;

        }
    }
}
