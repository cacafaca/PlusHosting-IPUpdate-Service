using ProCode.PlusHosting.Client;
using System;
using System.IO;
using System.Text.Json;

namespace ProCode.PlusHosting.IpUpdate.Service
{
    class LoginInfo
    {
        #region Constants
        public const string ConfigFileName = "LoginInfo.json";
        public const string PlusHostingLoginInfoSectionName = "PlusHostingLoginInfo";
        public const string MailSmtpInfoSectionName = "MailSmtpInfo";
        #endregion

        #region Fields
        #endregion

        #region Constructors
        public LoginInfo()
        {
            try
            {
                LoginInfoPoco.Rootobject loginInfo = JsonSerializer.Deserialize<LoginInfoPoco.Rootobject>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFileName)));
                UserCredential = new UserCredential(loginInfo.PlusHostingLoginInfo.User, loginInfo.PlusHostingLoginInfo.Pass);
                PlusHostingRecords = loginInfo.PlusHostingRecords;
                MailSmtpInfo = new MailSmtpInfo(loginInfo.MailSmtpInfo);
                Util.Trace.WriteLine($"Success reading configuration file: {ConfigFileName}");
            }
            catch (Exception ex)
            {
                Util.Trace.WriteLine($"Error reading configuration file: {ex.Message}");
                throw ex;
            }
        }
        #endregion

        #region Properties
        public UserCredential UserCredential { get; }
        public MailSmtpInfo MailSmtpInfo { get; }
        public LoginInfoPoco.PlusHostingRecord[] PlusHostingRecords { get; }
        #endregion


    }
}
