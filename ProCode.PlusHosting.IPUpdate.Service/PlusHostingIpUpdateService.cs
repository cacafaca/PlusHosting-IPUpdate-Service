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
        private System.Timers.Timer timer;
        private const int timerIntervalInseconds = 30;

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
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            Util.Debug.WriteLine($"Service Timer started: {DateTime.Now}. Interval: {timer.Interval / 1000}.");
        }

        protected override void OnStop()
        {
            timer.Stop();
            timer = null;   // In hope that GC will take it. :)
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Util.Debug.WriteLine($"Service Timer elapsed: {DateTime.Now}");
            Task.Run(() =>
            {
                MyIpClient myIpClient = new MyIpClient();
                var myIp = myIpClient.GetMyIp().Result;

                CPanelDns cpanel = new CPanelDns(new LoginInfo().UserCredential);
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
                                            resourceRecordA.Data = myIp.ToString();
                                        }
                                        else
                                        {
                                            Util.Trace.WriteLine($"No need to update IP Address. (PlusHosting) {resourceRecordA.Data} = {myIp} (my IP).");
                                        }
                                        //using (EventLog eventLog = new EventLog("Application"))
                                        //{
                                        //    eventLog.Source = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                                        //    eventLog.WriteEntry($"Resource record updated: {resourceRecordA}");
                                        //};                                                                                
                                    }
                                }
                            }
                        }
                    }
                }

                cpanel.Logout();
            });
        }

#if DEBUG
        public void OnStartDebug()
        {
            OnStart(null);
        }
#endif
    }
}
