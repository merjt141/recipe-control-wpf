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

            public decimal PesoFrima1 { get; set; }
        }

        // fecha: 25 02 2025, nuevo modelo de datos 
        public class DataTransfer2QRDTO
        {
            public int MacroRegistroId { get; set; }
            public string Usuario { get; set; } = string.Empty;
            public DateTime FechaCreacion { get; set; }
            public int Lote { get; set; }
            public int NombreMacroId { get; set; }
            public decimal PesoTotRealGr { get; set; }
            public decimal PesoTotObjGr { get; set; }
            public int Insumo1Id { get; set; }
            public int Insumo2Id { get; set; }
            public int Insumo3Id { get; set; }
            public int Insumo4Id { get; set; }

        }

        public class InsumoPesoDTO
        {
            public int InsumoId { get; set; }
            public string Codigo { get; set; } = string.Empty;
            public string Descripcion { get; set; } = string.Empty;
            public string Unidad { get; set; } = string.Empty;

            // decimal(7,4) en SQL
            public decimal PesoFrima1 { get; set; }
        }
    }
}
