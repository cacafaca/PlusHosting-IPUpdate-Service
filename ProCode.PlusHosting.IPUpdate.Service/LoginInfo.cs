using Microsoft.Extensions.Configuration;
using ProCode.PlusHosting.Client;
using System.Security;

namespace ProCode.PlusHosting.IpUpdate.Service
{
    class LoginInfo
    {
        #region Constants
#if !DEBUG
        public const string FileName = "LoginInfoRelease.json";
#else
        public const string FileName = "LoginInfoDebug.json";
#endif
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
