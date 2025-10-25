using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Models.DTOs
{
    public class RegisterWeightDataGridDTO
    {
        public int RegistroPesoId { get; set; }
        public string InsumoCodigo { get; set; } = string.Empty;
        public string TipoInsumoCodigo { get; set; } = string.Empty;
        public DateTime FechaPesado { get; set; }
        public decimal Valor { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public int Estado { get; set; }
    }
}
