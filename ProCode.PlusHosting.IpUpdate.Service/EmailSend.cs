using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace ProCode.PlusHosting.IpUpdate.Service
{
    class EmailSend : IDisposable
    {
        #region Constants
        private const string senderName = "Plus Hosting IP Update Service";
        #endregion

        #region Fields
        private readonly MailSmtpInfo mailInfo;
        private readonly SmtpClient smtpClient;
        private bool readyToSendMail;
        #endregion

        #region Constructor
        public EmailSend(MailSmtpInfo mailInfo)
        {
            this.mailInfo = mailInfo;
            smtpClient = new SmtpClient(mailInfo.Server, mailInfo.Port)
            {
                Credentials = new NetworkCredential(mailInfo.User, mailInfo.Pass),
                EnableSsl = mailInfo.EnableSsl
            };
            readyToSendMail = true;    // Assume that previous mail is sent at the beginning.
        }
        #endregion

        #region Methods
        public void Send(string subject, string body)
        {
            if (readyToSendMail)        // Accept only if previous mail is sent.
            {
                Task.Run(() =>
                {
                    do
                    {
                        try
                        {
                            smtpClient.Send(new MailMessage($"{senderName} <{mailInfo.User}>", mailInfo.ReportTo, subject, body));
                            readyToSendMail = true;
                        }
                        catch
                        {
                            readyToSendMail = false;
                        }
                    }
                    while (!readyToSendMail);
                });
            }
        }

        public void Dispose()
        {
            smtpClient.Dispose();
        }
        #endregion
    }
}
