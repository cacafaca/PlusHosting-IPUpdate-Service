using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace ProCode.PlusHosting.Client
{
    public class UserCredential
    {
        #region Fields
        private readonly string username;
        private readonly SecureString password;
        #endregion

        #region Constructors
        public UserCredential(string username, string password)
        {
            this.username = username.Trim();
            this.password = new SecureString();
            foreach (char passChar in password)
            {
                this.password.AppendChar(passChar);
            }

        }
        #endregion

        #region Properties
        public string Username { get { return username; } }
        public string Password { get { return GetPassword(); } }
        #endregion

        #region Methods
        private string GetPassword()
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(password);
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
