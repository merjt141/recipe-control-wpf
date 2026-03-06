using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Models
{
    public class DTOs
    {
        public class DataTransferQRDTO
        {
            public int MacroRegistroId { get; set; }
            public DateTime FechaCreacion { get; set; }
            public string Usuario { get; set; } = string.Empty;
            public int InsumoId { get; set; }
            public string Codigo { get; set; } = string.Empty;
            public decimal PesoReal { get; set; }
        }

        public class ComboBoxDTO
        {
            public int Id { get; set; }
            public string Codigo { get; set; } = string.Empty;
        }

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

        public class DataTransferQR2DTO
        {
            public int BatchRegistroId { get; set; }
            public int MacroRegistroId { get; set; }
            public int UsuarioStr { get; set; }
            public DateTime Fecha { get; set; }
            public DateTime FechaPistoleo { get; set; }
            public int LoteStr { get; set; }
            public int NombreMacroIdStr { get; set; }
            public decimal PesoRealStr { get; set; }
            public decimal PesoObjStr { get; set; }
            public int Insumo1Str { get; set; }
            public int Insumo2Str { get; set; }
            public int Insumo3Str { get; set; }
            public int Insumo4Str { get; set; }
        }

        public class ReportDataGrid2DTO
        {
            public int BatchRegistroId { get; set; }
            public int MacroRegistroId { get; set; }
            public int UsuarioStr { get; set; }
            public DateTime Fecha { get; set; }
            public DateTime FechaPistoleo { get; set; }
            public int LoteStr { get; set; }
            public int NombreMacroIdStr { get; set; }
            public decimal PesoRealStr { get; set; }
            public decimal PesoObjStr { get; set; }
            public int Insumo1Str { get; set; }
            public int Insumo2Str { get; set; }
            public int Insumo3Str { get; set; }
            public int Insumo4Str { get; set; }
        }
    }
}
