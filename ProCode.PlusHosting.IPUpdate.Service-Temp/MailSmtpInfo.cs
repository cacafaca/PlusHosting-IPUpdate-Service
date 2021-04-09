using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace ProCode.PlusHosting.IpUpdate.Service
{
    class MailSmtpInfo
    {
        #region Fields
        readonly string server;
        readonly int port;
        readonly bool enableSsl;
        readonly string user;
        readonly SecureString pass;
        readonly string reportTo;
        #endregion

        #region Constructors
        public MailSmtpInfo(LoginInfoPoco.MailSmtpInfo mailSmtpInfo)
        {
            this.server = mailSmtpInfo.Server;
            this.port = mailSmtpInfo.Port;
            this.enableSsl = mailSmtpInfo.EnableSsl;
            this.user = mailSmtpInfo.User;
            this.pass = new SecureString();
            foreach (char passChar in mailSmtpInfo.Pass)
            {
                this.pass.AppendChar(passChar);
            }
            this.reportTo = mailSmtpInfo.ReportTo;
        }
        #endregion

        #region Properties
        public string Server { get { return server; } }
        public int Port { get { return port; } }
        public bool EnableSsl { get { return enableSsl; } }
        public string User { get { return user; } }
        public string Pass { get { return GetPass(); } }
        public string ReportTo { get { return reportTo; } }
        #endregion

        #region Methods
        private string GetPass()
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(pass);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }
        #endregion
    }
}
