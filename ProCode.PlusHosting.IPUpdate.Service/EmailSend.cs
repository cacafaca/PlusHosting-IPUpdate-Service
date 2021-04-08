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
        }
        #endregion

        #region Methods
        public void Send(string subject, string body)
        {
            smtpClient.Send(new MailMessage($"{senderName} <{mailInfo.User}>", mailInfo.ReportTo, subject, body));
        }

        public void Dispose()
        {
            smtpClient.Dispose();
        }
        #endregion
    }
}
