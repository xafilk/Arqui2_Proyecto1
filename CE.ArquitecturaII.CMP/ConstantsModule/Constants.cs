using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CE.ArquitecturaII.CMP.ConstantsModule
{
    public class Constants
    {
        public const string CC_ACTION_BUS_FREE = "NO ACTION";
        public const string CC_ACTION_ALREADY = "ALREADY READ";
        public const string CORE_ZERO_ID = "CORE_ZERO";
        public const string CORE_ONE_ID = "CORE_ONE";
        public const string CORE_TWO_ID = "CORE_TWO";
        public const string CORE_THREE_ID = "CORE_THREE";
        public const string RAM_ID = "RAM_ONE";
        public const string MISS = "MISS";
        public const string WRITE = "WRITE";
        public const int CLOCK_CACHE_CONTROLLER = 1000; //ms
        public const int CLOCK_CACHE_INTERCONECTION_BUS = 1; //ms
        public const int CLOCK_CACHE_ACTION_BUS = 1; //ms
        public const int CLOCK_MEMORY_BUS = 1;
        public const int CLOCK_RAM = 10; //ms
        public const int CLOCK_PROC = 1000; //ms
        public const string MEMORY_BUS_NOT_READY = "NOT READY";
        public const string READ = "READ";
        public const string CC_READ = "CCREAD";
        public const string RAM_WRITE = "RAMWRITE";
        public const string CC_WRITE = "CCWRITE";
        public const string NO_DATA = "NODATA";
    }
}
