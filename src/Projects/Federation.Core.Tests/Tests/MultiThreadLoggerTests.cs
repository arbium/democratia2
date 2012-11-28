using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Federation.Core.Tests
{
    public class MultiThreadLoggerTests : IDisposable
    {
        private delegate void NewThread();
        private static ServiceLogger logger;

        public MultiThreadLoggerTests()
        {
            ConstHelper.AppPath = "C://";
            logger = new ServiceLogger("test-service");
        }

        public void Test()
        {            
            for (int i = 0; i < 100; i++)
            {
                //var b = new NewThread(EnterSomeData);
                Thread t = new Thread(new ThreadStart(EnterSomeData));
                t.Start();
                //b.BeginInvoke(null, null);
            }
        }

        public void EnterSomeData()
        {
            while (true)
            {
                logger.Write("!testing!");
                SaveData();
            }
        }

        public void SaveData()
        {
            logger.SaveLog();
        }

        public void Dispose()
        {
            logger.Close();
            logger = null;
        }
    }
}
