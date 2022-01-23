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
        const string emailErrorSubject = "Error processing IP update [Plus Hosting]";
        const string emailSuccesseIPUpdateSubject = "Success IP address update [Plus Hosting]";
        const string emailErrorAttachedFileName = "LastPage.html";
        const int maxRetryCount = 3;
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
            CPanelDns cpanel = new CPanelDns(loginInfo.UserCredential);
            try
            {
                MyIpClient myIpClient = new MyIpClient();
                var myIp = await myIpClient.GetMyIp();

                await cpanel.ReadAsync();

                foreach (var configService in loginInfo.PlusHostingRecords)
                {
                    var resourceRecord = cpanel.Services.List.Where(service => service.Name == configService.ServiceName).FirstOrDefault()?
                        .Domains.List.Where(domain => domain.Name == configService.DomainName).FirstOrDefault()?
                        .ResourceRecords.List.Where(rr => rr.Name == configService.ResourceRecord.Name && rr.RecordType == CPanelDnsResourceRecord.TypeA).FirstOrDefault();
                    if (resourceRecord != null)
                    {
                        if (resourceRecord.Data != myIp.ToString())
                        {
                            string oldIp = resourceRecord.Data;
                            resourceRecord.Data = myIp.ToString();

                            // Notify IP address change.
                            var emailClient = new EmailClient(loginInfo.MailSmtpInfo);
                            emailClient.Send(emailSuccesseIPUpdateSubject,
$@"Hi,

New IP ({resourceRecord.Data}) address updated on site www.plus.rs.

Old IP: {oldIp}

Sincerely yours,
Plus Hosting IP Updater Windows Service");
                        }
                        else
                        {
                            Util.Trace.WriteLine($"No need to update IP Address. (PlusHosting) {resourceRecord.Data} = {myIp} (my IP).");
                        }
                    }
                    else
                    {
                        Util.Trace.WriteLine($"Resource record not found.");
                        // Send an email maybe...
                    }
                }
            }
            catch (ClientException ex)
            {
                Util.Trace.WriteLine(ex.ToString());
                var emailSend = new EmailClient(loginInfo.MailSmtpInfo);
                emailSend.Send(emailErrorSubject, ex.ToString(), ex.Html != null ?
                    new System.Collections.Generic.Dictionary<string, System.IO.Stream>
                    {
                            {emailErrorAttachedFileName, new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(ex.Html)) }
                    } : null);
            }
            catch (Exception ex)
            {
                Util.Trace.WriteLine(ex.ToString());
                var emailSend = new EmailClient(loginInfo.MailSmtpInfo);
                emailSend.Send(emailErrorSubject, ex.ToString());
            }
            finally
            {
                cpanel.Logout();
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
