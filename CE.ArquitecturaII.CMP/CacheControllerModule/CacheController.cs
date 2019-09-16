using System;
using System.Threading;
using CE.ArquitecturaII.CMP.CacheInterconectionModule;
using CE.ArquitecturaII.CMP.CacheModule;
using CE.ArquitecturaII.CMP.ClockModule;
using CE.ArquitecturaII.CMP.CommonModule;
using CE.ArquitecturaII.CMP.ConstantsModule;
using CE.ArquitecturaII.CMP.CustomException;
using CE.ArquitecturaII.CMP.MemoryBusModule;

namespace CE.ArquitecturaII.CMP.CacheControllerModule
{
    public class CacheController
    {
        #region Constants
        private readonly string CORENAME;
        private readonly int CORENUMBER;
        #endregion Constants

        #region Fields
        public Cache cache = new Cache(8);
        private CCAction ccAction;
        private CCInterconection ccInter;
        /// <summary>
        /// Object for register activity with Log4Net
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ThreadFlagWrapper threadFlag;
        private Clock clock;
        private MemoryBus memBus;
        #endregion Fields

        #region Properties

        #endregion Properties

        #region Constructor 
        public CacheController(string coreId, int coreNum, CCAction ccAct, CCInterconection ccInt, ThreadFlagWrapper flag, Clock clk, MemoryBus bus)
        {
            CORENAME = coreId;
            CORENUMBER = coreNum;
            ccAction = ccAct;
            ccInter = ccInt;
            threadFlag = flag;
            clock = clk;
            memBus = bus;
            InitializeThreads();
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Method for continuos read the ActionBus
        /// </summary>
        private void ReadCCActionBus()
        {
            while (threadFlag.Flag)
            {                   
                string action = ccAction.ReadActionBus(CORENAME);
                //If bus is free or default or itself miss do nothing
                if (action == Constants.CC_ACTION_BUS_FREE || action == Constants.CC_ACTION_ALREADY)
                {
                    continue;
                }
                else
                {
                    string[] temp = action.Split('@');
                    MakeAction(temp[0], Int32.Parse(temp[1]), temp[2]);
                }
            }         
        }

        /// <summary>
        /// 
        /// </summary>
        private void ReadCCInterBus()
        {
            while (threadFlag.Flag)
            {
                string response = ccInter.GetRequest(CORENAME);
                if (response != "EMPTY")
                {
                    int value = cache.ReadCacheValue(response);
                    ccInter.ResponseRequest(CORENAME, value);
                    cache.ActionBusMiss(response);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="position"></param>
        /// <param name="tag"></param>
        private void MakeAction(string action, int position, string tag)
        {
            try
            {
                bool exist = cache.ExistCacheValue(tag);
                if (exist && action == Constants.MISS)
                {
                    int value = cache.ReadCacheValue(tag);
                    memBus.RequestBus(CORENAME, tag, Constants.CC_WRITE, value);
                    cache.ActionBusMiss(tag);
                }
                else if (exist && action == Constants.WRITE)
                {
                    ccInter.InvalidCacheData(position, CORENUMBER);
                    cache.ActionBusWrite(tag);
                }
            }
            catch (Exception e)
            {
                log.Error("CacheController, MakeAction, " + e.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int GetCacheValue(string tag)
        {
            int result = 0;
            bool reading = true;
            while (reading)
            {
                if (clock.CLOCK_CacheController)
                {
                    try
                    {
                        int value = cache.ReadCacheValue(tag); //HIT
                    }
                    catch (ColdMissCacheException ex1) //ColdMiss
                    {
                        ccAction.WriteActionBus(Constants.MISS, tag, CORENAME);
                        log.Error(CORENAME + ": " + ex1.Message);
                        result = ManageMiss(tag);
                    }
                    catch (InvalidMissCacheException ex2)
                    {
                        ccAction.WriteActionBus(Constants.MISS, tag, CORENAME);
                        log.Error(CORENAME + ": " + ex2.Message);
                        result = ManageMiss(tag);
                    }
                    catch (CorrespondenceMissCacheException ex3)
                    {
                        ccAction.WriteActionBus(Constants.MISS, tag, CORENAME);
                        log.Error(CORENAME + ": " + ex3.Message);
                        result = ManageMiss(tag);
                    }
                    reading = false;
                }
            }
            return result;
        }

        private int ManageMiss(string tag)
        {
            //Solicito el dato por red de interconexion
            var exec = GetDataFromInterconection(tag);
            bool success = exec.Item1;
            int value = exec.Item2;
            if (success)
            {
                cache.WriteCacheValue(tag, value, "S");
            }
            else
            {
                value = GetDataFromMem(tag);
                cache.WriteCacheValue(tag, value, "S");
            }
            ccInter.SaveCacheDataTable(tag, CORENAME, CORENUMBER);
            return value;
        }

        private Tuple<bool, int> GetDataFromInterconection(string tag)
        {
            bool message = false;
            int value = -1;
            bool request = ccInter.MakeRequest(tag, CORENAME);

            if (request)
            {
                bool reading = true;
                while (reading)
                {
                    string response = ccInter.GetReturn(CORENAME);
                    if (response != "NOT_READY")
                    {
                        value = Int32.Parse(response);
                        message = true;
                        reading = false;
                    }
                }
            }
            else
            {
                message = false;
                value = -1;
            }
            return Tuple.Create(message, value);
        }

        private int GetDataFromMem(string tag)
        {
            int value = 0;
            memBus.RequestBus(CORENAME, tag, Constants.CC_READ, 0);
            bool reading = true;
            while (reading)
            {
                string temp = memBus.ReadBus(CORENAME);
                if (temp != Constants.NO_DATA)
                {
                    reading = false;
                    value = Int32.Parse(temp);
                }
            }
            return value;
        }

        public void SaveCacheValue(string tag, int value)
        {
            bool saving = true;
            while (saving)
            {
                if (clock.CLOCK_CacheController)
                {
                    clock.CLOCK_CacheController = false;
                    try
                    {
                        cache.WriteCacheValue(tag, value, "M");
                    }
                    catch (CorrespondenceMissCacheException ex1)
                    {
                        log.Error(ex1.Message);
                        int position = Convert.ToInt32(tag, 2) % 2;
                        var oldData = cache.ReadCacheValue(position);
                        memBus.RequestBus(CORENAME, oldData.Item2, Constants.CC_WRITE, oldData.Item1);
                        cache.WriteCacheValue(tag, value, "M");
                    }
                    finally
                    {
                        ccInter.SaveCacheDataTable(tag, CORENAME, CORENUMBER);
                        ccAction.WriteActionBus(Constants.WRITE, tag, CORENAME);
                    }
                    saving = false;
                }
            }
        }

        private void InitializeThreads()
        {
            Thread monitorActionBusThr = new Thread(ReadCCActionBus);
            Thread monitorCCInterBusThr = new Thread(ReadCCInterBus);
            monitorActionBusThr.Start();
            monitorCCInterBusThr.Start();
        }


        #endregion Methods
    }
}
