using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProCode.PlusHosting.Client
{
    public class CPanelDnsDomains
    {
        #region Fields
        private readonly Uri serviceUri;
        private readonly PlusHostingClient client;
        #endregion

        #region Constructors
        public CPanelDnsDomains(PlusHostingClient client, Uri serviceUri)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.serviceUri = serviceUri ?? throw new ArgumentNullException(nameof(client));
        }
        #endregion

        #region Methods
        public async Task<IList<CPanelDnsDomain>> GetDomainListAsync()
        {
            // Login to site, to collect data.
            if (!client.IsLoggedIn)
                await client.LoginAsync();

            IList<CPanelDnsDomain> domainList = new List<CPanelDnsDomain>();
            var domainUriList = await client.GetCPanelDnsDomainUriListAsync(serviceUri);
            if (domainUriList != null)
            {
                foreach (var domainUri in domainUriList)
                {
                    domainList.Add(new CPanelDnsDomain(client, domainUri.Name, domainUri.Uri));
                }
            }

            return domainList;
        }
        #endregion
    }
}