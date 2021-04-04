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
    }
}