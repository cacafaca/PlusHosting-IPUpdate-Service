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

        [TestMethod()]
        public void Generate_Json()
        {
            LoginInfoPoco.Rootobject loginInfo = new LoginInfoPoco.Rootobject
            {
                PlusHostingLoginInfo = new LoginInfoPoco.PlusHostingLoginInfo
                {
                    User = "User1",
                    Pass = "Pass1"
                },
                PlusHostingRecords = new LoginInfoPoco.PlusHostingRecord[]
                {
                    new LoginInfoPoco.PlusHostingRecord
                    {
                        ServiceName="Service1",
                        DomainName ="domain.rs",
                        ResourceRecord=new LoginInfoPoco.ResourceRecord
                        {
                            Name ="domain.rs",
                            Type="A"
                        }
                    }
                },
                MailSmtpInfo = new LoginInfoPoco.MailSmtpInfo
                {
                    Server = "smtp.gmail.cpm",
                    Port = 587,
                    EnableSsl = true,
                    Pass = "Pass2",
                    User = "mail@gmail.com",
                    ReportTo = "some@mail.com"
                }
            };
            var jsonStr = JsonSerializer.Serialize(loginInfo);
            File.WriteAllText("LoginInfo_Generated.json", jsonStr);
        }
    }
}