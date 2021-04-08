using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProCode.PlusHosting.IpUpdate.Service.Tests
{
    [TestClass()]
    public class EmailSendTests
    {
        [TestMethod()]
        public void SendTest()
        {
            EmailSend emailSend = new EmailSend(new LoginInfo().MailSmtpInfo);
            emailSend.Send($"Test message from {System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.", "This is a test message.\nSecond row.\nThird row.");
        }
    }
}