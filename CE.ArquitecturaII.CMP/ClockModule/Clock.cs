using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CE.ArquitecturaII.CMP.ConstantsModule;
using CE.ArquitecturaII.CMP.CommonModule;

namespace CE.ArquitecturaII.CMP.ClockModule
{
    public unsafe class Clock
    {
        #region Properties

        /// <summary>
        /// Clock for CCInterconection Bus
        /// </summary>
        public bool CLOCK_CCInterconection { get; set; }
        /// <summary>
        /// Clock for CCAction Bus
        /// </summary>
        public bool CLOCK_CCAction { get; set; }
        /// <summary>
        /// Clock for Cache Controller Module
        /// </summary>
        public bool CLOCK_CacheController { get; set; }
        /// <summary>
        /// Clock for memory bus
        /// </summary>
        public bool CLOCK_MemoryBus { get; set; }
        /// <summary>
        /// Clock for memory RAM
        /// </summary>
        public bool CLOCK_RAM { get; set; }

        #endregion Properties

        #region Fields

        /// <summary>
        /// Threads Execution Control
        /// </summary>
        private readonly ThreadFlagWrapper threadFlag;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="flag"></param>
        public Clock(ThreadFlagWrapper flag)
        {
            threadFlag = flag;
            CLOCK_CCInterconection = false;
            CLOCK_CCAction = false;
            CLOCK_CacheController = false;
            CLOCK_MemoryBus = false;
            CLOCK_RAM = false;
            InitializeThreads();
        }

        #endregion COnstructor

        #region Methods

        /// <summary>
        /// Cache Controller Interconection Clock Generator
        /// </summary>
        private void CCInterClock()
        {
            while (threadFlag.Flag)
            {
                CLOCK_CCInterconection = !CLOCK_CCInterconection;
                Thread.Sleep(Constants.CLOCK_CACHE_INTERCONECTION_BUS);
            }
        }

        /// <summary>
        /// Cache Controller Action Bus Clock Generator
        /// </summary>
        private void CCActionClock()
        {
            while (threadFlag.Flag)
            {
                CLOCK_CCAction = !CLOCK_CCAction;
                Thread.Sleep(Constants.CLOCK_CACHE_ACTION_BUS);
            }
        }

        /// <summary>
        /// Cache Controller Clock Generator
        /// </summary>
        private void CacheControllerClock()
        {
            while (threadFlag.Flag)
            {
                CLOCK_CacheController = !CLOCK_CacheController;
                Thread.Sleep(Constants.CLOCK_CACHE_CONTROLLER);
            }
        }

        /// <summary>
        /// Memory Bus Clock Generator
        /// </summary>
        private void MemoryBusClock()
        {
            while (threadFlag.Flag)
            {
                CLOCK_MemoryBus = !CLOCK_MemoryBus;
                Thread.Sleep(Constants.CLOCK_MEMORY_BUS);
            }
        }

        /// <summary>
        /// Memory RAM clock
        /// </summary>
        private void MemoryRamClock()
        {
            while (threadFlag.Flag)
            {
                CLOCK_RAM = !CLOCK_RAM;
                Thread.Sleep(Constants.CLOCK_RAM);
            }
        }

        /// <summary>
        /// Method for Initialize all Clock Threads
        /// </summary>
        public void InitializeThreads()
        {
            Thread ccInterThr = new Thread(CCInterClock);
            Thread ccActionThr = new Thread(CCActionClock);
            Thread ccThr = new Thread(CacheControllerClock);
            Thread memBus = new Thread(MemoryBusClock);
            Thread memRam = new Thread(MemoryRamClock);
            ccInterThr.Start();
            ccActionThr.Start();
            ccThr.Start();
            memBus.Start();
            memRam.Start();
        }

        #endregion Methods
    }
}
