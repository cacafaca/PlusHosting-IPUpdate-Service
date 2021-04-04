using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProCode.PlusHosting.Client;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace ProCode.PlusHosting.ClientTests
{
    [TestClass()]
    public class PlusHostingClientTests
    {
        private PlusHostingClient GetClientWithLoginInfo()
        {
            PlusHostingClient client = new PlusHostingClient(new LoginInfo().UserCredential);
            return client;
        }

        [TestMethod()]
        public void Login_And_Logout()
        {
            var client = GetClientWithLoginInfo();
            client.LoginAsync().Wait();
            client.LogoutAsync().Wait();
            // If there is no exception, then test succeeded. 
        }

        [TestMethod()]
        public void Get_List_Of_CPanels()
        {
            var client = GetClientWithLoginInfo();
            client.LoginAsync().Wait();
            try
            {
                var cpanelDnsList = client.GetCPanelDnsServiceUriListAsync().Result;
                Assert.IsNotNull(cpanelDnsList);
                Assert.AreNotEqual(0, cpanelDnsList.Count);
            }
            finally
            {
                client.LogoutAsync().Wait();    // Logout before assertion.
            }
        }

        [TestMethod()]
        public void Get_List_Of_All_CPanels_And_Its_All_Domains()
        {
            var client = GetClientWithLoginInfo();
            client.LoginAsync().Wait();
            try
            {
                var cpanelDnsServiceUriList = client.GetCPanelDnsServiceUriListAsync().Result;
                Assert.IsNotNull(cpanelDnsServiceUriList);
                Assert.AreNotEqual(0, cpanelDnsServiceUriList.Count);

                foreach (var service in cpanelDnsServiceUriList)
                {
                    /* If I run test in Debug mode (not Run mode) than I can preview values Directly in Output window in VS. Just need to select "Show output from Debug".                     
                     * Then I do not have to run DebugView tool. That's because I\m lazy and I always need to search whole disk (60 sec) to debugview.exe. :) */
                    System.Diagnostics.Debug.WriteLine($"Service name: '{service.Name}'({service.Uri})");

                    var domainUriList = client.GetCPanelDnsDomainUriListAsync(service.Uri).Result;
                    Assert.IsNotNull(domainUriList);
                    Assert.AreNotEqual(0, domainUriList.Count);

                    foreach (var domainUri in domainUriList)
                    {
                        System.Diagnostics.Debug.WriteLine($"Service name / Domain name: '{service.Name}' / '{domainUri.Name}'({domainUri.Uri})");
                    }
                }

            }
            finally
            {
                client.LogoutAsync().Wait();    // Logout before assertion.
            }
        }

        [TestMethod()]
        public void Get_First_Domain_And_Its_ResourceRecords()
        {
            var client = GetClientWithLoginInfo();
            client.LoginAsync().Wait();
            try
            {
                var cpanelDnsServiceUriList = client.GetCPanelDnsServiceUriListAsync().Result;
                Assert.IsNotNull(cpanelDnsServiceUriList);
                Assert.AreNotEqual(0, cpanelDnsServiceUriList.Count);

                foreach (var service in cpanelDnsServiceUriList)
                {
                    /* If I run test in Debug mode (not Run mode) than I can preview values Directly in Output window in VS. Just need to select "Show output from Debug".                     
                     * Then I do not have to run DebugView tool. That's because I\m lazy and I always need to search whole disk (60 sec) to debugview.exe. :) */
                    System.Diagnostics.Debug.WriteLine($"Service name: '{service.Name}'({service.Uri})");

                    var domainUriList = client.GetCPanelDnsDomainUriListAsync(service.Uri).Result;
                    Assert.IsNotNull(domainUriList);
                    Assert.AreNotEqual(0, domainUriList.Count);

                    foreach (var domainUri in domainUriList)
                    {
                        System.Diagnostics.Debug.WriteLine($"Service name / Domain name: '{service.Name}' / '{domainUri.Name}'({domainUri.Uri})");

                        var resourceRecordList = client.GetCPanelDnsDomainResourceRecordListAsync(domainUri.Uri).Result;
                        Assert.IsNotNull(resourceRecordList);
                        Assert.AreNotEqual(0, resourceRecordList.Count);

                        foreach (var resourceRecord in resourceRecordList)
                        {
                            System.Diagnostics.Debug.WriteLine($"Service name / Domain name / Resource record: '{service.Name}' / '{domainUri.Name}'({domainUri.Uri}) / {resourceRecord}");
                        }
                    }
                }

            }
            finally
            {
                client.LogoutAsync().Wait();    // Logout before assertion.
            }
        }

        [TestMethod()]
        public void Update_A_Record()
        {
            var client = GetClientWithLoginInfo();
            client.LoginAsync().Wait();
            try
            {
                var cpanelDnsServiceUriList = client.GetCPanelDnsServiceUriListAsync().Result;
                Assert.IsNotNull(cpanelDnsServiceUriList);
                Assert.AreNotEqual(0, cpanelDnsServiceUriList.Count);

                foreach (var service in cpanelDnsServiceUriList)
                {
                    /* If I run test in Debug mode (not Run mode) than I can preview values Directly in Output window in VS. Just need to select "Show output from Debug".                     
                     * Then I do not have to run DebugView tool. That's because I\m lazy and I always need to search whole disk (60 sec) to debugview.exe. :) */
                    System.Diagnostics.Debug.WriteLine($"Service name: '{service.Name}'({service.Uri})");

                    var domainUriList = client.GetCPanelDnsDomainUriListAsync(service.Uri).Result;
                    Assert.IsNotNull(domainUriList);
                    Assert.AreNotEqual(0, domainUriList.Count);

                    System.Diagnostics.Debug.WriteLine($"Service name / Domain name: '{service.Name}' / '{domainUriList[0].Name}'({domainUriList[0].Uri})");

                    var resourceRecordList = client.GetCPanelDnsDomainResourceRecordListAsync(domainUriList[0].Uri).Result;
                    Assert.IsNotNull(resourceRecordList);
                    Assert.AreNotEqual(0, resourceRecordList.Count);

                    var resourceRecord = resourceRecordList.Where(rec => rec.RecordType == "A" && rec.Data != "127.0.0.1").FirstOrDefault();
                    if (resourceRecord != null)
                    {
                        string newIpValue = resourceRecord.Data = "46.240.181.102";
                        System.Diagnostics.Debug.WriteLine($"Service name / Domain name / Resource record: '{service.Name}' / '{domainUriList[0].Name}'({domainUriList[0].Uri}) / {resourceRecord}");
                        var resourceRecordListNew = client.UpdateCPanelDnsDomainResourceRecordAsync(resourceRecord, domainUriList[0].Uri).Result;

                        Assert.AreEqual(newIpValue, resourceRecordList.Where(rec => rec.RecordType == "A" && rec.Data != "127.0.0.1").FirstOrDefault().Data);
                    }
                }

            }
            finally
            {
                client.LogoutAsync().Wait();    // Logout before assertion.
            }
        }
    }
}