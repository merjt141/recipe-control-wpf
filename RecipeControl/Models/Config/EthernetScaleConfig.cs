using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Models.Config
{
    public class EthernetScaleConfig
    {
        public int Id { get; set; }
        public string IPAddress { get; set; } = string.Empty;
        public int Port { get; set; } = 0;
    }
}
