using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProCode.PlusHosting.Client
{
    /// <summary>
    /// Represents cPanel functionality from PlusHosting.
    /// </summary>
    public class CPanelDns
    {
        #region Fields
        private readonly PlusHostingClient client;
        private readonly CPanelDnsServices services;
        #endregion

        #region Constructors
        public CPanelDns(LoginInfo userCredential)
        {
            client = new PlusHostingClient(userCredential);

            services = new CPanelDnsServices(client);
        }
        #endregion

        #region Finalizes
        ~CPanelDns()
        {
            client.LogoutAsync().Wait();
        }
        #endregion

        #region Properties
        public CPanelDnsServices Services { get { return services; } }
        public bool IsLoggedIn { get { return client.IsLoggedIn; } }
        #endregion

        #region Methods
        public void Logout()
        {
            client.LogoutAsync().Wait();
        }

        public async Task UpdateAsync(System.Net.IPAddress ip)
        {
            var services = await Services.GetServiceListAsync();
            if (services != null)
            {
                foreach (var configService in loginInfo.PlusHostingRecords)
                {
                    var plusHostingService = services.Where(s => s.Name == configService.ServiceName).FirstOrDefault();
                    if (plusHostingService != null)
                    {
                        try
                        {
                            int retryCount = 1;
                            while (retryCount < maxRetryCount)
                            {
                                var plusHostingDomains = await plusHostingService.Domains.GetDomainListAsync();
                                if (plusHostingDomains != null)
                                {
                                    var plusHostingDomain = plusHostingDomains.Where(d => d.Name == configService.DomainName).FirstOrDefault();
                                    if (plusHostingDomain != null)
                                    {
                                        var plusHostingResourceRecords = await plusHostingDomain.ResourceRecords.GetResourceRecirdListAsync();
                                        if (plusHostingResourceRecords != null)
                                        {
                                            var plusHostingResourceRecord = plusHostingResourceRecords.Where(rec => rec.RecordType == configService.ResourceRecord.Type && rec.Name == configService.ResourceRecord.Name).FirstOrDefault();
                                            if (plusHostingResourceRecord != null)
                                            {
                                                if (plusHostingResourceRecord.Data != myIp.ToString())
                                                {
                                                    string oldIp = plusHostingResourceRecord.Data;
                                                    plusHostingResourceRecord.Data = myIp.ToString();

                                                    // Notify IP address change.
                                                    var emailClient = new EmailClient(loginInfo.MailSmtpInfo);
                                                    emailClient.Send(emailSuccesseIPUpdateSubject,
$@"Hi,

New IP ({plusHostingResourceRecord.Data}) address updated on site www.plus.rs.

Old IP: {oldIp}

Sincerely yours,
Plus Hosting IP Updater Windows Service");
                                                }
                                                else
                                                {
                                                    Util.Trace.WriteLine($"No need to update IP Address. (PlusHosting) {plusHostingResourceRecord.Data} = {myIp} (my IP).");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }

        }
        #endregion
    }
}