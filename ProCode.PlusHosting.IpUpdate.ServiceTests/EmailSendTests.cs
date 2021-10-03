using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProCode.PlusHosting.IpUpdate.Service.Tests
{
    [TestClass()]
    public class EmailSendTests
    {
        [TestMethod()]
        public void Send_Simple_Message()
        {
            EmailClient emailSend = new EmailClient(new LoginInfo().MailSmtpInfo);
            emailSend.Send($"Test message from {System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.", "This is a test message.\nSecond row.\nThird row.");
        }

        [TestMethod()]
        public void Send_Attachemnt()
        {
            EmailClient emailSend = new EmailClient(new LoginInfo().MailSmtpInfo);
            emailSend.Send($"Test message from {System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.", "This is a test message.\nSecond row.\nThird row.", 
                new System.Collections.Generic.Dictionary<string, System.IO.Stream>
                {
                    {"TestFileName1.html", new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes("<html>\n<body>\nSome content.\n</body>\n<html>")) }
                });
        }
    }
}