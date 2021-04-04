using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProCode.PlusHosting.Client
{
    /// <summary>
    /// Need special class as a list in order to have LoadAsync method.
    /// </summary>
    public class CPanelDnsServices
    {
        #region Fields
        private readonly PlusHostingClient client;
        #endregion

        #region Constructors
        public CPanelDnsServices(PlusHostingClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }
        #endregion

        #region Methods
        public async Task<List<CPanelDnsService>> GetServiceListAsync()
        {
            // Login to site, to collect data.
            if (!client.IsLoggedIn)
                await client.LoginAsync();

            List<CPanelDnsService> serviceList = new List<CPanelDnsService>();
            var serviceUriList = await client.GetCPanelDnsServiceUriListAsync();
            if (serviceUriList != null)
            {
                foreach (var serviceUri in serviceUriList)
                {
                    serviceList.Add(new CPanelDnsService(client, serviceUri.Name, serviceUri.Uri));
                }
            }

            return serviceList;
        }
        #endregion
    }
}