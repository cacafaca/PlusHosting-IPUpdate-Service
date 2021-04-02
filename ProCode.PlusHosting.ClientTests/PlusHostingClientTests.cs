using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProCode.PlusHosting.Client;
using System.Linq;
using System.Security;

namespace ProCode.PlusHosting.ClientTests
{
    [TestClass()]
    public class PlusHostingClientTests
    {
        [TestMethod()]
        public void LoginAsyncTest()
        {
            var loginInfo = new LoginInfo();
            PlusHostingClient client = new PlusHostingClient(new UserCredential(
                username: loginInfo.User,
                password: loginInfo.Pass
            ));
            client.LoginAsync().Wait();
            client.LogoutAsync().Wait();
            // If there is no exception, then test succeeded. 
        }
    }
}