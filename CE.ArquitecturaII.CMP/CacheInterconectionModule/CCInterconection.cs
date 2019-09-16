using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CE.ArquitecturaII.CMP.ClockModule;
using CE.ArquitecturaII.CMP.CommonModule;

namespace CE.ArquitecturaII.CMP.CacheInterconectionModule
{
    public class CCInterconection
    {
        #region Constants

        /// <summary>
        /// Number of Cache blocks
        /// </summary>
        private static readonly int CACHEBLOCK = 8;

        /// <summary>
        /// Number of Cores
        /// </summary>
        private static readonly int CORENUMBER = 4;

        #endregion Constants

        #region Fields
        /// <summary>
        /// Flag for control the threads exec
        /// </summary>
        private ThreadFlagWrapper threadFlag;
        /// <summary>
        /// Type of request => 01 = Request, 02 = Return
        /// </summary>
        private int type;
        /// <summary>
        /// The Core to the Request/Return is send
        /// </summary>
        private string toCoreId;
        /// <summary>
        /// The Core that make Request/Response
        /// </summary>
        private string fromCoreId;
        /// <summary>
        /// The adrres for the Request Type
        /// </summary>
        private string address;
        /// <summary>
        /// The Value for the Return Type
        /// </summary>
        private int value;
        /// <summary>
        /// Flag for the use of the Bus
        /// </summary>
        private bool bussy;
        /// <summary>
        /// Table for save the Address x Core information
        /// </summary>
        private List<CacheTableBlock> cacheTable;
        private Clock clock;

        private Queue requestQueue;

        #endregion Fileds

        public CCInterconection(ThreadFlagWrapper flag, Clock clk)
        {
            threadFlag = flag;
            clock = clk;
            Initialize();
            InitializeThreads();
        }

        /// <summary>
        /// Method for Enqueue a request to the Cache Interconection
        /// The Method validate if the addres are in the table, if its true then put the request into the Queue
        /// </summary>
        /// <param name="myAddress"></param>
        /// <param name="myCoreId"></param>
        /// <returns></returns>
        public bool MakeRequest(string myAddress, string myCoreId)
        {
            bool result;
            result = false;
            foreach (CacheTableBlock temp in cacheTable)
            {
                if (temp.Address == myAddress && temp.Valid == true)
                {
                    CacheDataRequest request = new CacheDataRequest()
                    {
                        MyCoreId = myCoreId,
                        ToCoreId = temp.CoreId,
                        Address = myAddress
                    };
                    requestQueue.Enqueue(request);
                    result = true;
                    Console.WriteLine(myCoreId + ": " + "Solitud de valor por red de Interconexión");
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// FIFO Ordering for Insert Request into the BUS
        /// </summary>
        public void MakeRequestAux()
        {
            while (threadFlag.Flag)
            {
                if (clock.CLOCK_CCInterconection && !bussy && requestQueue.Count > 0)
                {
                    clock.CLOCK_CCInterconection = false;
                    CacheDataRequest newRequest = (CacheDataRequest)requestQueue.Dequeue();
                    toCoreId = newRequest.ToCoreId;
                    fromCoreId = newRequest.MyCoreId;
                    address = newRequest.Address;
                    type = 1;
                    bussy = true;         
                }
            }           
        }

        /// <summary>
        /// Put the Value into the Bus, keep the bus bussy
        /// </summary>
        /// <param name="myCoreId"></param>
        /// <param name="toCoreId"></param>
        /// <param name="myValue"></param>
        public void ResponseRequest(string myCoreId, int myValue)
        {
            //Security Validation
            if (toCoreId == myCoreId)
            {
                Console.WriteLine(myCoreId + ": " + "Respuesta por red Interconexión");
                //Change Type to Return
                type = 2;
                value = myValue;
                //Change the Id to the Receiver Id
                toCoreId = fromCoreId;
                fromCoreId = myCoreId;
            }
        }

        /// <summary>
        /// Method for obtein the Request, the Core need to valide its Id a get the Addres of the request, this method doesnt relese the Bus until the Core response with the value
        /// </summary>
        /// <param name="myCoreId"></param>
        /// <returns></returns>
        public string GetRequest(string myCoreId)
        {
            string result = "EMPTY";
            if (toCoreId == myCoreId && type == 1)
            {
                type = 1;
                result = address;
            }
            return result;
        }

        /// <summary>
        /// Read the bus in searh of Returns, return NOT_READY when the Value isn't ready
        /// </summary>
        /// <param name="myCoreId"></param>
        /// <returns></returns>
        public string GetReturn(string myCoreId)
        {
            string result;
            if (toCoreId == myCoreId && type == 2 && bussy)
            {
                Console.WriteLine(myCoreId + ": " + "Se obtiene por red de Interconexión");
                result = value.ToString();
                bussy = false;
            }
            else
            {
                result = "NOT_READY";
            }
            return result;
        }

        /// <summary>
        /// Method for save the Cache MetaData in the CacheTable
        /// </summary>
        /// <param name="myAddress"></param>
        /// <param name="myCoreId"></param>
        public void SaveCacheDataTable(string myAddress, string myCoreId, int coreNumber)
        {
            int position = Convert.ToInt32(myAddress, 2) % 2;
            int realPosition = position + (CACHEBLOCK * coreNumber);
            cacheTable[realPosition].Address = myAddress;
            cacheTable[realPosition].Valid = true;
            cacheTable[realPosition].CoreId = myCoreId;
        }

        /// <summary>
        /// Method for Invalidate the cache metadata
        /// </summary>
        /// <param name="position"></param>
        /// <param name="coreNumber"></param>
        public void InvalidCacheData(int position, int coreNumber)
        {
            int realPosition = position + (CACHEBLOCK * coreNumber);
            cacheTable[realPosition].Valid = false;
        }

        /// <summary>
        /// Initialize the CacheTable
        /// </summary>
        private void Initialize()
        {
            cacheTable = new List<CacheTableBlock>();
            requestQueue = new Queue();
            //Thread for Monitorice the Write Bus Request
            for (int i = 0; i < CORENUMBER * CACHEBLOCK; i++)
            {
                CacheTableBlock block = new CacheTableBlock()
                {
                    Address = "",
                    Valid = false,
                    CoreId = ""
                };
                cacheTable.Add(block);
            }
            bussy = false;
        }

        /// <summary>
        /// Methods for initialize Threads
        /// </summary>
        private void InitializeThreads()
        {
            Thread writeBusRequestThr = new Thread(MakeRequestAux);
            writeBusRequestThr.Start();
        }
    }
}
