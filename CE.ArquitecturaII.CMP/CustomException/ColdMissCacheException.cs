using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CE.ArquitecturaII.CMP.CustomException
{
    public class ColdMissCacheException : Exception
    {
        public ColdMissCacheException() : base("Cold Miss Cache Execption")
        {
        }
    }
}
