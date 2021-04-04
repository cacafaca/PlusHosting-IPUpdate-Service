using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProCode.PlusHosting.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProCode.PlusHosting.ClientTests
{
    [TestClass()]
    public class CPanelDnsTests
    {
        [TestMethod()]
        public void LoadTest()
        {
            CPanelDns cpanel = new CPanelDns(new LoginInfo().UserCredential);
            cpanel.LoadAsync();
        }
    }
}