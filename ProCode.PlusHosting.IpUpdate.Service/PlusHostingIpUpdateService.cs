using ProCode.PlusHosting.Client;
using System;
using System.Data;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using Util = ProCode.PlusHosting.Client.Util;

namespace ProCode.PlusHosting.IpUpdate.Service
{
    public partial class PlusHostingIpUpdateService : ServiceBase
    {
        #region Constants
        private const int timerIntervalInseconds = 5 * 60;  // 5 minutes
        #endregion

        #region Fields
        private System.Timers.Timer timer;
        readonly LoginInfo loginInfo;
        #endregion

        public PlusHostingIpUpdateService()
        {
            InitializeComponent();
            loginInfo = new LoginInfo();
        }

        protected override void OnStart(string[] args)
        {
            timer = new System.Timers.Timer(1000 * timerIntervalInseconds)
            {
                AutoReset = true
            };

            Util.Trace.WriteLine($"Service started: {DateTime.Now}. Interval: {timer.Interval / 1000} sec.");

            timer.Elapsed += Timer_Elapsed;

            // Do initial check immediately.
            Task.Run(() =>
            {
                UpdateCheck();
            });

            timer.Start();
        }

        protected override void OnStop()
        {
            timer.Stop();
            timer = null;   // In hope that GC will take it. :)
            Util.Trace.WriteLine($"Service stopped: {DateTime.Now}.");
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Util.Debug.WriteLine($"Service Timer elapsed: {DateTime.Now}");
            Task.Run(() =>
            {
                UpdateCheck();
            });
        }

        private async void UpdateCheck()
        {
            try
            {
                CPanelDns cpanel = new CPanelDns(loginInfo.UserCredential);
                try
                {
                    MyIpClient myIpClient = new MyIpClient();
                    //var myIp = myIpClient.GetMyIp_whatismyipaddress_com().Result;
                    var myIp = await myIpClient.GetMyIp_ipv4_icanhazip_com();

                    var services = await cpanel.Services.GetServiceListAsync();
                    if (services != null)
                    {
                        foreach (var configService in loginInfo.PlusHostingRecords)
                        {
                            var plusHostingService = services.Where(s => s.Name == configService.ServiceName).FirstOrDefault();
                            if (plusHostingService != null)
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
                                                    using (var emailSend = new EmailSend(loginInfo.MailSmtpInfo))
                                                    {
                                                        emailSend.Send("IP address updated [Plus Hosting]",
                                                             $@"Hi,

New IP ({plusHostingResourceRecord.Data}) address updated on site www.plus.rs.

Old IP: {oldIp}

Sincerely yours,
Plus Hosting IP Updater Windows Service"
                                                             );
                                                    }
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

                    }
                }
                catch (Exception ex)
                {
                    Util.Trace.WriteLine(ex.ToString());
                }
                finally
                {
                    cpanel.Logout();
                }
            }
            catch (Exception ex)
            {
                Util.Trace.WriteLine(ex.ToString());
            }

        }

#if DEBUG
        public void OnStartDebug()
        {
            OnStart(null);
        }
#endif
    }
}
