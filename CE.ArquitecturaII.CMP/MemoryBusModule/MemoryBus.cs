using CE.ArquitecturaII.CMP.ClockModule;
using CE.ArquitecturaII.CMP.CommonModule;
using CE.ArquitecturaII.CMP.ConstantsModule;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CE.ArquitecturaII.CMP.MemoryBusModule
{
    public class MemoryBus
    {
        #region Fields
        private string coreName;
        private bool bussy;
        private string address;
        private int value;
        private string type;
        Queue busQueue = new Queue();
        ThreadFlagWrapper threadFlag;
        Clock clock;
        #endregion Fields

        #region Properties
        #endregion Properties

        #region Constructor
        public MemoryBus(ThreadFlagWrapper flag, Clock clk)
        {
            threadFlag = flag;
            clock = clk;
            InitializeThreads();
        }
        #endregion Constructor

        #region Methods

        /// <summary>
        /// Method for Enqueue Request of Data
        /// </summary>
        /// <param name="coreId"></param>
        /// <param name="address"></param>
        public void RequestBus(string coreId, string newAddress, string type, int value)
        {
            MemoryDataSave memData = new MemoryDataSave()
            {
                CoreId = (coreId == "") ? coreName : coreId,
                Address = (newAddress == "") ? address : newAddress,
                Type = type,
                Value = value
            };
            busQueue.Enqueue(memData);
        }

        /// <summary>
        /// Method for read Memomy Bus
        /// </summary>
        /// <param name="coreId"></param>
        /// <param name="ramId"></param>
        /// <returns></returns>
        public string ReadBus(string readMember)
        {
            string data;
            //CC Write Data, RAM Read
            if (bussy && type == Constants.CC_WRITE && readMember == "RAM")
            {
                data = type + "@" + address + "@" + value;
                type = Constants.NO_DATA;
                bussy = false;
            }
            //CC Request, RAM return data
            else if (bussy && type == Constants.CC_READ && readMember == "RAM")
            {
                data = type + "@" + address;
                type = Constants.NO_DATA;
                bussy = false;
            }
            //Ran Write, CC Read
            else if (bussy && type == Constants.RAM_WRITE && readMember != "RAM")
            {
                data = (coreName == readMember) ? value.ToString() : Constants.NO_DATA;
                bussy = (coreName == readMember) ? bussy : false;
            }
            //default
            else
            {
                data = Constants.NO_DATA;
            }
            return data;
        }

        /// <summary>
        /// Method for make thread to charge data to de bus
        /// </summary>
        private void RequestBusAux()
        {
            while (threadFlag.Flag)
            {
                if (clock.CLOCK_MemoryBus && !bussy && busQueue.Count > 0)
                {
                    MemoryDataSave temp = (MemoryDataSave)busQueue.Dequeue();
                    coreName = temp.CoreId;
                    address = temp.Address;
                    bussy = true;
                    value = temp.Value;
                    type = temp.Type;
                }
            }
        }

        /// <summary>
        /// Method fot Initialize Threads
        /// </summary>
        public void InitializeThreads()
        {
            Thread memBusThr = new Thread(RequestBusAux);
            memBusThr.Start();
        }
        #endregion Methods
    }
}
