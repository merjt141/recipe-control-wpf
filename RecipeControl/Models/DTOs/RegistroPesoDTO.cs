using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Models.DTOs
{
    public class RegistroPesoDTO
    {
        public int RegistroPesoId { get; set; }
        public string InsumoCodigo { get; set; } = string.Empty;
        public string TipoInsumoCodigo { get; set; } = string.Empty;
        public DateTime FechaPesado { get; set; }
        public decimal CantidadPesada { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;
        public string RegistroPesoCodigo { get; set; } = string.Empty;
        public int Estado { get; set; }
    }
}
