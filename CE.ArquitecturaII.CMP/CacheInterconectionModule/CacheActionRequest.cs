using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CE.ArquitecturaII.CMP.CacheInterconectionModule
{
    public class CacheActionRequest
    {
        public int Position { get; set; }
        public string Tag { get; set; }
        public string Acction {get; set;}
        public string CoreName { get; set; }
    }
}
