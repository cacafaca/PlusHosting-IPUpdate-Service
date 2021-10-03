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
    class EmailClient
    {
        #region Constants
        private const string senderName = "Plus Hosting IP Update Service";
        #endregion

        #region Fields
        private static bool readyToSendMail;
        private readonly MailSmtpInfo mailInfo;
        #endregion

        #region Constructor
        public EmailClient(MailSmtpInfo mailInfo)
        {
            this.mailInfo = mailInfo;
            readyToSendMail = true;    // Assume that previous mail is sent at the beginning.
        }
        #endregion

        #region Methods
        public void Send(string subject, string body, Dictionary<string, System.IO.Stream> attachmentDictionary = null)
        {
            if (readyToSendMail)        // Accept only if previous mail is sent.
            {                
#if !DEBUG
                // Fire and forget only in RElease mode, because it is easier to perform unit tests.      
                Task.Run(() =>
                {
#endif
                    do
                    {
                        try
                        {
                            SmtpClient smtpClient = new SmtpClient(mailInfo.Server, mailInfo.Port)
                            {
                                Credentials = new NetworkCredential(mailInfo.User, mailInfo.Pass),
                                EnableSsl = mailInfo.EnableSsl
                            };
                            var message = new MailMessage($"{senderName} <{mailInfo.User}>", mailInfo.ReportTo, subject, body);

                            // Attachments.
                            if (attachmentDictionary != null)
                            {
                                foreach (var attachment in attachmentDictionary)
                                {
                                    message.Attachments.Add(new Attachment(attachment.Value, attachment.Key));
                                }
                            }

                            smtpClient.Send(message);
                            smtpClient.Dispose();
                            readyToSendMail = true;
                            Client.Util.Trace.WriteLine($"Mail sent to {mailInfo.ReportTo}.");
                        }
                        catch (Exception ex)
                        {
                            readyToSendMail = false;
                            Client.Util.Trace.WriteLine($"Can't sent email to {mailInfo.ReportTo}. Subject: {subject}.");
                            Client.Util.Trace.WriteLine($"Error message: {ex.Message}");
                            Client.Util.Trace.WriteLine(ex.StackTrace);
                            System.Threading.Thread.Sleep(new TimeSpan(0, 1, 0));
                        }
                    }
                    while (!readyToSendMail);
#if !DEBUG

                });
#endif
            }
        }
#endregion
    }
}
