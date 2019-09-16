using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CE.ArquitecturaII.CMP.CacheModule
{
    public class CacheBlock
    {
        public string State { get; set; }
        public string Tag { get; set; }
        public int Value { get; set; }
    }
}
