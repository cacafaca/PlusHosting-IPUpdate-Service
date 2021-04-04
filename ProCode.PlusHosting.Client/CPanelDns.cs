using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProCode.PlusHosting.Client
{
    /// <summary>
    /// Represents cPanel functionality from PlusHosting.
    /// </summary>
    public class CPanelDns
    {
        #region Fields
        private readonly PlusHostingClient client;
        private readonly CPanelDnsServices services;
        #endregion

        #region Constructors
        public CPanelDns(UserCredential userCredential)
        {
            client = new PlusHostingClient(userCredential);

            services = new CPanelDnsServices(client);
        }
        #endregion

        #region Finalizes
        ~CPanelDns()
        {
            client.LogoutAsync().Wait();
        }
        #endregion

        #region Properties
        public CPanelDnsServices Services { get { return services; } }
        public bool IsLoggedIn { get { return client.IsLoggedIn; } }
        #endregion

        #region Methods
        public void Logout()
        {
            client.LogoutAsync().Wait();
        }
        #endregion
    }
}