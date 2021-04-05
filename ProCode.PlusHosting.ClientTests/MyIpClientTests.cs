using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProCode.PlusHosting.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProCode.PlusHosting.Client.Tests
{
    [TestClass()]
    public class MyIpClientTests
    {
        [TestMethod()]
        public void GetMyIpTest()
        {
            MyIpClient myIpClinet = new MyIpClient();
            var myIp = myIpClinet.GetMyIp().Result;
            Assert.IsNotNull(myIp);
            System.Diagnostics.Debug.WriteLine($"My IP address is {myIp}");            
        }

        [TestMethod()]
        public void TaskRun()
        {
            System.Diagnostics.Debug.WriteLine($"Start of main procedure.");
            Task.Run(() => 
            {
                System.Diagnostics.Debug.WriteLine($"Start of task procedure.");
                System.Diagnostics.Debug.WriteLine($"Sleep for 1sec in task procedure.");
                System.Threading.Thread.Sleep(1000);
                System.Diagnostics.Debug.WriteLine($"Awake in task procedure.");
                System.Diagnostics.Debug.WriteLine($"Exit from task procedure.");
            });
            System.Diagnostics.Debug.WriteLine($"End of main procedure.");
        }
    }
}