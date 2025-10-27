using RecipeControl.Models.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Models.Entities
{
    public class RegistroPeso : IBaseEntity
    {
        public int RegistroPesoId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int RecetaVersionId { get; set; }
        public int InsumoId { get; set; }
        public int BalanzaId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime FechaPesado { get; set; }
        public decimal Valor { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
    }
}
