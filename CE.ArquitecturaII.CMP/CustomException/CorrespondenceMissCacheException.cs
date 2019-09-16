using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CE.ArquitecturaII.CMP.CustomException
{
    public class CorrespondenceMissCacheException : Exception
    {
        public CorrespondenceMissCacheException() : base("Correspondence Miss Cache Exception")
        {
        }
    }
}
