using Microsoft.Extensions.Configuration;
using ProCode.PlusHosting.Client;

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
            var config = new ConfigurationBuilder()
                .SetBasePath(System.AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile(ConfigFileName).Build();

            UserCredential = new UserCredential(config.GetSection($"{PlusHostingLoginInfoSectionName}:User").Value,
                config.GetSection($"{PlusHostingLoginInfoSectionName}:Pass").Value);

            MailSmtpInfo = new MailSmtpInfo(
                server: config.GetSection($"{MailSmtpInfoSectionName}:Server").Value,
                port: int.Parse(config.GetSection($"{MailSmtpInfoSectionName}:Port").Value),
                enableSsl: bool.Parse(config.GetSection($"{MailSmtpInfoSectionName}:EnableSsl").Value ?? "true"),
                user: config.GetSection($"{MailSmtpInfoSectionName}:User").Value,
                pass: config.GetSection($"{MailSmtpInfoSectionName}:Pass").Value,
                reportTo: config.GetSection($"{MailSmtpInfoSectionName}:ReportTo").Value
            );
        }
        #endregion

        #region Properties
        public UserCredential UserCredential { get; }
        public MailSmtpInfo MailSmtpInfo { get; }
        #endregion


    }
}
