using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CE.ArquitecturaII.CMP.CustomException;

namespace CE.ArquitecturaII.CMP.CacheModule
{
    public class Cache
    {
        #region Constants

        /// <summary>
        /// Amount fo Cache Blocks
        /// </summary>
        private readonly int NUMBLOCKS;

        #endregion Constants

        #region Fields

        /// <summary>
        /// List of CacheBlocks
        /// </summary>
        public List<CacheBlock> memoryCache;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="numBlocks"></param>
        public Cache(int numBlocks)
        {
            NUMBLOCKS = numBlocks;
            InitializeCache();
        }

        #endregion COnstructor

        #region Methods

        /// <summary>
        /// Initialize de Cache (Cold Cache)
        /// </summary>
        public void InitializeCache()
        {
            memoryCache = new List<CacheBlock>();
            for (int i = 0; i < NUMBLOCKS; i++)
            {
                CacheBlock block = new CacheBlock()
                {
                    State = "I",
                    Tag = "",
                    Value = 0
                };
                memoryCache.Add(block);
            }
        }

        /// <summary>
        /// Method for get a cache value, Throw exception if has a Miss
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public int ReadCacheValue(string tag)
        {
            int value;
            int position = Convert.ToInt32(tag, 2) % 2;
            CacheBlock temp = memoryCache[position];

            if (temp.Tag == "")
            {
                throw new ColdMissCacheException();
            }
            else if (temp.Tag == tag)
            {
                if (temp.State == "I")
                {
                    throw new InvalidMissCacheException();
                }
                else
                {
                    value = temp.Value;
                }
            }
            else
            {
                throw new CorrespondenceMissCacheException();
            }
            return value;
        }

        /// <summary>
        /// Method for get a cache value, Throw exception if has a Miss
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public Tuple<int, string> ReadCacheValue(int position)
        {
            int value = memoryCache[position].Value;
            string tag = memoryCache[position].Tag;
            return Tuple.Create(value,tag);
        }

        /// <summary>
        /// Validate if the position a tag given is in the Cache
        /// </summary>
        /// <param name="position"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool ExistCacheValue(string tag)
        {
            bool result;
            int position = Convert.ToInt32(tag, 2) % 2;
            CacheBlock temp = memoryCache[position];
            result = (temp.Tag == tag) ? true : false;
            return result;
        }

        /// <summary>
        /// Method for Write cache Value
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="value"></param>
        public void WriteCacheValue(string tag, int value, string state)
        {
            int position = Convert.ToInt32(tag, 2) % 2;
            bool exist = ExistCacheValue(tag);
            if (exist || memoryCache[position].State == "I" || memoryCache[position].Tag == "")
            {
                memoryCache[position].Value = value;
                memoryCache[position].Tag = tag;
                memoryCache[position].State = state;
            }
            else
            {
                throw new CorrespondenceMissCacheException();
            }

        }

        /// <summary>
        /// Change the State when a Miss ocurs in the Action Bus
        /// </summary>
        /// <param name="position"></param>
        public void ActionBusMiss(string tag)
        {
            int position = Convert.ToInt32(tag, 2) % 2;
            switch (memoryCache[position].State)
            {
                case "M":
                    memoryCache[position].State = "S";
                    break;
                default:
                    memoryCache[position].State = memoryCache[position].State;
                    break;
            }
        }

        /// <summary>
        /// Change the state when a Write ocurs in the action Bus
        /// </summary>
        /// <param name="position"></param>
        public void ActionBusWrite(string tag)
        {
            int position = Convert.ToInt32(tag, 2) % 2;
            switch (memoryCache[position].State)
            {
                case "M":
                    memoryCache[position].State = "I";
                    break;
                case "S":
                    memoryCache[position].State = "I";
                    break;
                default:
                    memoryCache[position].State = memoryCache[position].State;
                    break;
            }
        }
        #endregion Methods
    }
}
