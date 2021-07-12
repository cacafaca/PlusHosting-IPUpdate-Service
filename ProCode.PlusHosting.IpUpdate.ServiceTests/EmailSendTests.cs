using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProCode.PlusHosting.IpUpdate.Service.Tests
{
    [TestClass()]
    public class EmailSendTests
    {
        [TestMethod()]
        public void SendTest()
        {
            EmailClient emailSend = new EmailClient(new LoginInfo().MailSmtpInfo);
            emailSend.Send($"Test message from {System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.", "This is a test message.\nSecond row.\nThird row.");
        }
    }
}