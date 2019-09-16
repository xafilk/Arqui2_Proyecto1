using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using CE.ArquitecturaII.CMP.MemoryBusModule;
using CE.ArquitecturaII.CMP.ClockModule;
using CE.ArquitecturaII.CMP.CommonModule;
using CE.ArquitecturaII.CMP.ConstantsModule;
using System.Threading;

namespace CE.ArquitecturaII.CMP.RAMModule
{
    public class RAM
    {
        #region Constants
        /// <summary>
        /// Number of RAM Blocks
        /// </summary>
        private readonly int NUMBLOCKS = 16;

        #endregion Constants
        #region Fileds

        /// <summary>
        /// Object for register activity with Log4Net
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// List for save the ram blocks that compose the Memory RAM
        /// </summary>
        private List<RAMBlock> memoryRAM;

        private MemoryBus memBus;
        private Clock clock;
        private ThreadFlagWrapper threadFlagWrapper;



        #endregion Fields

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public RAM(ThreadFlagWrapper flag, Clock clk, MemoryBus memoryBus)
        {
            threadFlagWrapper = flag;
            clock = clk;
            memBus = memoryBus;
            InitializeRam();
            InitialaizeThreads();
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Method for create the 16 ram block and initialize with cero
        /// </summary>
        private void InitializeRam()
        {
            memoryRAM = new List<RAMBlock>();
            for (int i = 0; i< NUMBLOCKS; i++)
            {
                RAMBlock block = new RAMBlock() {
                    Address = Convert.ToString(i, 2).PadLeft(4, '0'),
                    Value = 0
                };
                //log.Info(Convert.ToInt32(block.Address, 2));
                //log.Info(block.Address);
                memoryRAM.Add(block);
            }
        }
        
        /// <summary>
        /// Method for get the value for an specific memory addres
        /// </summary>
        /// <param name="addres"></param>
        /// <returns></returns>
        public string GetValue(string address)
        {
            string result;
            try
            {
                RAMBlock temp = memoryRAM.Where(x => x.Address == address).SingleOrDefault();
                result = temp.Value.ToString();
            }
            catch (Exception e)
            {
                log.Error("RAM, GetValue, Error No existe la dirección de Memoria " + e.Message.ToString());
                result = Constants.NO_DATA;
            }
            return result;
        }

        /// <summary>
        /// Method for set the value of an specific address
        /// </summary>
        /// <param name="addres"></param>
        /// <param name="value"></param>
        private void SetValue(string address, int value)
        {
            for (int i = 0; i < NUMBLOCKS; i++)
            {
                if (memoryRAM[i].Address == address)
                {
                    memoryRAM[i].Value = value;
                    break;
                }
            }
        }

        private void ReadMainBus()
        {
            while (threadFlagWrapper.Flag)
            {
                if (clock.CLOCK_RAM)
                {
                    string data = memBus.ReadBus("RAM");
                    if (data != Constants.NO_DATA)
                    {
                        string[] temp = data.Split('@');
                        if (temp[0] == Constants.CC_WRITE)
                        {
                            SetValue(temp[1], Int32.Parse(temp[2]));
                        }
                        else if (temp[0] == Constants.CC_READ)
                        {
                            string value = GetValue(temp[1]);
                            if (value != Constants.NO_DATA)
                            {
                                WriteMainBus(Int32.Parse(value));
                            }
                        }
                    }
                }
            }
        }

        private void WriteMainBus(int value)
        {
            memBus.RequestBus("", "", Constants.RAM_WRITE, value);
        }

        private void InitialaizeThreads()
        {
            Thread readMainBusThr = new Thread(ReadMainBus);
            readMainBusThr.Start();
        }


        #endregion Methods
    }
}
