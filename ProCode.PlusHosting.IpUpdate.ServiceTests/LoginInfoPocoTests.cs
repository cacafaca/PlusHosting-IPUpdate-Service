using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text.Json;

namespace ProCode.PlusHosting.IpUpdate.Service.Tests
{
    [TestClass()]
    public class LoginInfoPocoTests
    {
        [TestMethod()]
        public void Load_LoginInfo_Json_File()
        {
            var jsonStr = File.ReadAllText("LoginInfo.json");
            LoginInfoPoco.Rootobject loginInfo = JsonSerializer.Deserialize<LoginInfoPoco.Rootobject>(jsonStr);
            Assert.IsNotNull(loginInfo);
            Assert.IsNotNull(loginInfo.PlusHostingLoginInfo);
            Assert.IsNotNull(loginInfo.PlusHostingRecords);
            Assert.IsNotNull(loginInfo.MailSmtpInfo);
        }
    }
}