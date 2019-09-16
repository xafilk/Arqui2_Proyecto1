using System;
using CE.ArquitecturaII.CMP.CacheInterconectionModule;
using CE.ArquitecturaII.CMP.CacheModule;
using CE.ArquitecturaII.CMP.CustomException;
using CE.ArquitecturaII.CMP.RAMModule;
using CE.ArquitecturaII.CMP.CacheControllerModule;
using CE.ArquitecturaII.CMP.ConstantsModule;
using CE.ArquitecturaII.CMP.ClockModule;
using CE.ArquitecturaII.CMP.CommonModule;
using System.Threading;
using CE.ArquitecturaII.CMP.MemoryBusModule;

namespace CE.ArquitecturaII.CMP.OutConsole1
{
    class ControlPanel1
    {
        static void Main(string[] args)
        {
            ///Flag for application threads Control
            ThreadFlagWrapper flag = new ThreadFlagWrapper();
            flag.Flag = true;

            Clock clock = new Clock(flag);

            CCAction cc = new CCAction(flag, clock);

            CCInterconection interconection = new CCInterconection(flag, clock);

            MemoryBus mainBus = new MemoryBus(flag, clock);

            RAM ram = new RAM(flag, clock, mainBus);

            mainBus.RequestBus(Constants.CORE_ZERO_ID, "0000", Constants.CC_WRITE, 5);

            //TODO Agregar el main bus a los controladores de  cache y a la RAM
            CacheController cacheControler0 = new CacheController(Constants.CORE_ZERO_ID, 0, cc, interconection, flag, clock, mainBus);
            CacheController cacheControler1 = new CacheController(Constants.CORE_ONE_ID, 1, cc, interconection, flag, clock, mainBus);
            CacheController cacheControler2 = new CacheController(Constants.CORE_TWO_ID, 2, cc, interconection, flag, clock, mainBus);
            CacheController cacheControler3 = new CacheController(Constants.CORE_THREE_ID, 3, cc, interconection, flag, clock, mainBus);

            Console.WriteLine("Prueba 1: Coherencia de Cache - Escritura");
            string Test1Addres = "0000";
            cacheControler0.SaveCacheValue(Test1Addres, 1);
            cacheControler1.SaveCacheValue(Test1Addres, 2);
            cacheControler2.SaveCacheValue(Test1Addres, 3);
            cacheControler3.SaveCacheValue(Test1Addres, 4);
            int position = Convert.ToInt32(Test1Addres, 2) % 2;
            Console.WriteLine("Cache Core 0: " + "Value = " + cacheControler0.cache.memoryCache[position].Value + " State = " + cacheControler0.cache.memoryCache[position].State);
            Console.WriteLine("Cache Core 1: " + "Value = " + cacheControler1.cache.memoryCache[position].Value + " State = " + cacheControler1.cache.memoryCache[position].State);
            Console.WriteLine("Cache Core 2: " + "Value = " + cacheControler2.cache.memoryCache[position].Value + " State = " + cacheControler2.cache.memoryCache[position].State);
            Console.WriteLine("Cache Core 3: " + "Value = " + cacheControler3.cache.memoryCache[position].Value + " State = " + cacheControler3.cache.memoryCache[position].State);
            Thread.Sleep(1000);
            cacheControler0.GetCacheValue("0000");

            for (int i = 0; i < 5; i++)

            {
                Thread.Sleep(1000);
                int a = 5;
            }
            int x = cacheControler0.GetCacheValue("0000");
       


        }
    }
}
