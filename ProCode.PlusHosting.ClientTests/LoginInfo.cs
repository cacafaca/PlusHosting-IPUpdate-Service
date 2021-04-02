using Microsoft.Extensions.Configuration;
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

        public string User { get; }
        public SecureString Pass { get; }
        public LoginInfo()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(System.AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile(LoginInfo.FileName).Build();
            User = config.GetSection(nameof(User)).Value;
            Pass = new SecureString();
            foreach (char passChar in config.GetSection(nameof(Pass)).Value.ToCharArray())
            {
                Pass.AppendChar(passChar);
            }

        }
    }
}
