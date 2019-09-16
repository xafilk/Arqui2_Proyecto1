using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CE.ArquitecturaII.CMP.CustomException
{
    public class InvalidMissCacheException : Exception
    {
        public InvalidMissCacheException() : base("Invalid Miss Cache Execption")
        {
        }
    }
}
