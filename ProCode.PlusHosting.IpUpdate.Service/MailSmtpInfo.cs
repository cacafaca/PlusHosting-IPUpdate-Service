using System;
using System.Runtime.InteropServices;
using System.Security;

namespace ProCode.PlusHosting.IpUpdate.Service
{
    public class MailSmtpInfo
    {
        #region Fields
        readonly string _server;
        readonly int _port;
        readonly bool _enableSsl;
        readonly string _user;
        readonly SecureString _pass;
        readonly string _reportTo;
        #endregion

        #region Constructors
        public MailSmtpInfo(LoginInfoPoco.MailSmtpInfo mailSmtpInfo)
        {
            _server = mailSmtpInfo.Server;
            _port = mailSmtpInfo.Port;
            _enableSsl = mailSmtpInfo.EnableSsl;
            _user = mailSmtpInfo.User;
            _pass = new SecureString();
            foreach (char passChar in mailSmtpInfo.Pass)
            {
                this._pass.AppendChar(passChar);
            }
            this._reportTo = mailSmtpInfo.ReportTo;
        }
        #endregion

        #region Properties
        public string Server { get { return _server; } }
        public int Port { get { return _port; } }
        public bool EnableSsl { get { return _enableSsl; } }
        public string User { get { return _user; } }
        public string Pass { get { return GetPass(); } }
        public string ReportTo { get { return _reportTo; } }
        #endregion

        #region Methods
        private string GetPass()
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(_pass);
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
