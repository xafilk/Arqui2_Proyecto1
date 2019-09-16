using System;
using System.Collections.Generic;
using System.Text;

namespace CE.ArquitecturaII.CMP.CacheInterconectionModule
{
    public class CacheTableBlock
    {
        /// <summary>
        /// Memory Addres
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Valid Bit
        /// </summary>
        public bool Valid { get; set; }
        /// <summary>
        /// Identifier ot the core that have the Adrres
        /// </summary>
        public string CoreId { get; set; }
    }
}
