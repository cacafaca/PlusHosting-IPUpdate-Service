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
        private List<CPanelDnsService> _serviceList;
        #endregion

        #region Constructors
        public CPanelDnsServices(PlusHostingClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            _serviceList = new List<CPanelDnsService>();
        }
        #endregion

        #region Properties

        public List<CPanelDnsService> List
        {
            get { return _serviceList; }
            set { _serviceList = value; }
        }

        #endregion

        #region Methods
        public async Task ReadAsync()
        {
            // Login to site, to collect data.
            if (!client.IsLoggedIn)
                await client.LoginAsync();
                        
            _serviceList.Clear();
            var serviceUriList = await client.GetCPanelDnsServiceUriListAsync();
            if (serviceUriList != null)
            {
                foreach (var serviceUri in serviceUriList)
                {
                    var service = new CPanelDnsService(client, serviceUri.Name, serviceUri.Uri);
                    _serviceList.Add(service);
                    await service.Domains.ReadAsync();
                }
            }
        }
        #endregion
    }
}