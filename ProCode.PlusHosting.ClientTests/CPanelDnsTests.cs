using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProCode.PlusHosting.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProCode.PlusHosting.Client.Tests
{
    [TestClass()]
    public class CPanelDnsTests
    {
        [TestMethod()]
        public void Get_All_Records()   // Most comprehensive test.
        {
            CPanelDns cpanel = new CPanelDns(new IpUpdate.Service.LoginInfo().UserCredential);
            cpanel.Services.ReadAsync().Wait();
            Assert.AreNotEqual(0, cpanel.Services.List.Count);
            foreach (var service in cpanel.Services.List)
            {
                Assert.AreNotEqual(0, service.Domains.List.Count);
                foreach (var domain in service.Domains.List)
                {
                    Assert.AreNotEqual(0, domain.ResourceRecords.List.Count);
                    Assert.IsNotNull(domain.ResourceRecords.List.Where(rr => rr.RecordType == CPanelDnsResourceRecord.TypeA).FirstOrDefault());
                }
            }
            cpanel.Logout();
            Assert.IsFalse(cpanel.IsLoggedIn);
        }
    }
}