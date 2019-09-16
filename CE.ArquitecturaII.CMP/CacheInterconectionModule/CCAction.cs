using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CE.ArquitecturaII.CMP.ClockModule;
using CE.ArquitecturaII.CMP.ConstantsModule;
using CE.ArquitecturaII.CMP.CommonModule;

namespace CE.ArquitecturaII.CMP.CacheInterconectionModule
{
    public class CCAction
    {
        #region Fields

        private bool coreZeroRead = false;
        private bool coreOneRead = false;
        private bool coreTwoRead = false;
        private bool coreThreeRead = false;
        private string acction;
        private int position;
        private string tag;
        private bool bussy;
        private Queue actionQueue;
        private Clock clock;
        private ThreadFlagWrapper threadFlag;
        private readonly object x = new object();

        #endregion Fields

        #region Properties

        #endregion Properties

        #region Constructor
        public CCAction(ThreadFlagWrapper flag, Clock clk)
        {
            bussy = false;
            actionQueue = new Queue();
            threadFlag = flag;
            clock = clk;
            Thread writeBusRequestThr = new Thread(WriteAcctionBusAux);
            writeBusRequestThr.Start();
        }
        #endregion COnstructor

        #region Methods
        public void WriteActionBus(string acction, string tag, string coreName)
        {
            int position = Convert.ToInt32(tag, 2) % 2;
            CacheActionRequest temp = new CacheActionRequest()
            {
                Acction = acction,
                Position = position,
                Tag = tag,
                CoreName = coreName
            };
            actionQueue.Enqueue(temp);
        }

        private void WriteAcctionBusAux()
        {
            while (threadFlag.Flag)
            {
                //Operate only when Clock is True Simulate Clock Signals
                if (!bussy && actionQueue.Count > 0)
                {
                    clock.CLOCK_CCAction = false;
                    CacheActionRequest temp = (CacheActionRequest)actionQueue.Dequeue();
                    acction = temp.Acction;
                    tag = temp.Tag;
                    position = temp.Position;
                    bussy = true;
                    switch (temp.CoreName)
                    {
                        case Constants.CORE_ZERO_ID:
                            coreZeroRead = true;
                            break;
                        case Constants.CORE_ONE_ID:
                            coreOneRead = true;
                            break;
                        case Constants.CORE_TWO_ID:
                            coreTwoRead = true;
                            break;
                        case Constants.CORE_THREE_ID:
                            coreThreeRead = true;
                            break;
                        default:
                            break;
                    };
                }
            }
        }

        public string ReadActionBus(string myCoreId)
        {
            lock (x)
            {
                string result;
                if (!bussy)
                {
                    result = Constants.CC_ACTION_BUS_FREE;
                }
                else
                {
                    result = GetAcctionData(myCoreId);
                    ClearBus();
                }
                return result;
            }
        }

        private string GetAcctionData(string myCoreId)
        {
            string result;
            switch (myCoreId)
            {
                case Constants.CORE_ZERO_ID:
                    if (coreZeroRead == true)
                    {
                        result = Constants.CC_ACTION_ALREADY;
                    }
                    else
                    {
                        coreZeroRead = true;
                        result = acction + "@" + position + "@" + tag;
                    }
                    break;
                case Constants.CORE_ONE_ID:
                    if (coreOneRead == true)
                    {
                        result = Constants.CC_ACTION_ALREADY;
                    }
                    else
                    {
                        coreOneRead = true;
                        result = acction + "@" + position + "@" + tag;
                    }
                    break;
                case Constants.CORE_TWO_ID:
                    if (coreTwoRead == true)
                    {
                        result = Constants.CC_ACTION_ALREADY;
                    }
                    else
                    {
                        coreTwoRead = true;
                        result = acction + "@" + position + "@" + tag;
                    }
                    break;
                case Constants.CORE_THREE_ID:
                    if (coreThreeRead == true)
                    {
                        result = Constants.CC_ACTION_ALREADY;
                    }
                    else
                    {
                        coreThreeRead = true;
                        result = acction + "@" + position + "@" + tag;
                    }
                    break;
                default:
                    result = Constants.CC_ACTION_BUS_FREE;
                    break;
            }

            return result;
        }

        private void ClearBus()
        {
            if (coreZeroRead && coreOneRead && coreTwoRead && coreThreeRead)
            {
                acction = "";
                position = -1;
                tag = "";
                coreZeroRead = false;
                coreOneRead = false;
                coreTwoRead = false;
                coreThreeRead = false;
                bussy = false;
            }
        }
        #endregion Methods
    }
}
