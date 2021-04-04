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
        public void Get_All_Services()
        {
            CPanelDns cpanel = new CPanelDns(new LoginInfo().UserCredential);
            var services = cpanel.Services.GetServiceListAsync().Result;
            System.Diagnostics.Debug.WriteLine($"Number of services: {services.Count}");
            Assert.AreNotEqual(0, services.Count);
            foreach(var service in services)
            {
                System.Diagnostics.Debug.WriteLine($"Service name: {service.Name}");
            }

            cpanel.Logout();
            Assert.IsFalse(cpanel.IsLoggedIn);
        }

        [TestMethod()]
        public void Get_All_Domains()
        {
            CPanelDns cpanel = new CPanelDns(new LoginInfo().UserCredential);
            
            // HTTPS get service list
            var services = cpanel.Services.GetServiceListAsync().Result;
            
            Assert.IsNotNull(services);
            System.Diagnostics.Debug.WriteLine($"Number of services: {services.Count}");
            Assert.AreNotEqual(0, services.Count);
            foreach (var service in services)
            {
                System.Diagnostics.Debug.WriteLine(service);

                // HTTPS get domain list for service                
                var domains = service.Domains.GetDomainListAsync().Result;
                
                Assert.IsNotNull(domains);
                System.Diagnostics.Debug.WriteLine($"Number of domains: {domains.Count}");
                foreach(var domain in domains)
                {
                    System.Diagnostics.Debug.WriteLine(domain);
                }
            }
            
            cpanel.Logout();
            Assert.IsFalse(cpanel.IsLoggedIn);
        }

        [TestMethod()]
        public void Get_All_Records()   // Most comprehensive test.
        {
            CPanelDns cpanel = new CPanelDns(new LoginInfo().UserCredential);

            // HTTPS get service list
            var services = cpanel.Services.GetServiceListAsync().Result;

            Assert.IsNotNull(services);
            System.Diagnostics.Debug.WriteLine($"Number of services: {services.Count}");
            Assert.AreNotEqual(0, services.Count);
            foreach (var service in services)
            {
                System.Diagnostics.Debug.WriteLine(service);

                // HTTPS get domain list for service                
                var domains = service.Domains.GetDomainListAsync().Result;

                Assert.IsNotNull(domains);
                System.Diagnostics.Debug.WriteLine($"Number of domains: {domains.Count}");
                foreach (var domain in domains)
                {
                    System.Diagnostics.Debug.WriteLine(domain);

                    // HTTPS get resource records
                    var resourceRecords = domain.ResourceRecords.GetResourceRecirdListAsync().Result;

                    Assert.IsNotNull(resourceRecords);
                    System.Diagnostics.Debug.WriteLine($"Number of resource records: {resourceRecords.Count}");
                    foreach (var resourceRecord in resourceRecords)
                    {
                        System.Diagnostics.Debug.WriteLine(resourceRecord);
                    }
                }
            }

            cpanel.Logout();
            Assert.IsFalse(cpanel.IsLoggedIn);
        }
    }
}