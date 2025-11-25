using RecipeControl.Models.Entities.Base;
using RecipeControl.Models.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Models.Entities
{
    public class RegistroPeso : BaseEntity, IBaseEntity
    {
        public int RegistroPesoId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string? Descripcion { get; set; } = string.Empty;
        public int RecetaVersionId { get; set; }
        public int InsumoId { get; set; }
        public int BalanzaId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime FechaPesado { get; set; }
        public decimal CantidadPesada { get; set; }
        public bool Estado { get; set; }
        public bool EstadoRegistro { get; set; }
    }
}
