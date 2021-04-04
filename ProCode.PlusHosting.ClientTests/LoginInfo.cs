using Microsoft.Extensions.Configuration;
using ProCode.PlusHosting.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace ProCode.PlusHosting.ClientTests
{
    class LoginInfo
    {
        #region Constants
        public const string FileName = "LoginInfo.json";
        #endregion

        public UserCredential UserCredential { get; }
        public LoginInfo()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(System.AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile(LoginInfo.FileName).Build();
            string user = config.GetSection("User").Value;
            var pass = new SecureString();
            foreach (char passChar in config.GetSection("Pass").Value.ToCharArray())
            {
                pass.AppendChar(passChar);
            }
            UserCredential = new UserCredential(user, pass);
        }
    }
}
