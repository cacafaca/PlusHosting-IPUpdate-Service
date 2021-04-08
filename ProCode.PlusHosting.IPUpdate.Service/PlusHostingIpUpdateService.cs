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
        private const int timerIntervalInseconds = 5 * 60;  // 5 minutes
        private System.Timers.Timer timer;

        public PlusHostingIpUpdateService()
        {
            InitializeComponent();
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

        private void UpdateCheck()
        {
            try
            {
                LoginInfo loginInfo = new LoginInfo();
                CPanelDns cpanel = new CPanelDns(loginInfo.UserCredential);
                try
                {
                    MyIpClient myIpClient = new MyIpClient();
                    //var myIp = myIpClient.GetMyIp_whatismyipaddress_com().Result;
                    var myIp = myIpClient.GetMyIp_ipv4_icanhazip_com().Result;

                    var services = cpanel.Services.GetServiceListAsync().Result;
                    if (services != null)
                    {
                        var fhrService = services.Where(s => s.Name == "FileHosterRepo").FirstOrDefault();
                        if (fhrService != null)
                        {
                            var domains = fhrService.Domains.GetDomainListAsync().Result;
                            if (domains != null)
                            {
                                var fhrDomain = domains.Where(d => d.Name == "filehosterrepo.in.rs").FirstOrDefault();
                                if (fhrDomain != null)
                                {
                                    var resourceRecords = fhrDomain.ResourceRecords.GetResourceRecirdListAsync().Result;
                                    if (resourceRecords != null)
                                    {
                                        var resourceRecordA = resourceRecords.Where(rec => rec.RecordType == "A" && rec.Name == "filehosterrepo.in.rs.").FirstOrDefault();
                                        if (resourceRecordA != null)
                                        {
                                            if (resourceRecordA.Data != myIp.ToString())
                                            {
                                                string oldIp = resourceRecordA.Data;
                                                resourceRecordA.Data = myIp.ToString();
                                                using (var emailSend = new EmailSend(loginInfo.MailSmtpInfo))
                                                {
                                                    emailSend.Send("IP address updated [Plus Hosting]",
                                                         $@"Hi,

New IP ({resourceRecordA.Data}) address updated on site www.plus.rs.

Old IP: {oldIp}

Sincerely yours,
Plus Hosting IP Updater Windows Service"
                                                         );
                                                }
                                            }
                                            else
                                            {
                                                Util.Trace.WriteLine($"No need to update IP Address. (PlusHosting) {resourceRecordA.Data} = {myIp} (my IP).");
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
