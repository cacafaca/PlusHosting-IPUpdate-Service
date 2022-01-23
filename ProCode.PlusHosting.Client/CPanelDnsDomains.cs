using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProCode.PlusHosting.Client
{
    public class CPanelDnsDomains
    {
        #region Fields
        private readonly Uri _serviceUri;
        private readonly PlusHostingClient _client;
        private IList<CPanelDnsDomain> _domainList;
        #endregion

        #region Constructors
        public CPanelDnsDomains(PlusHostingClient client, Uri serviceUri)
        {
            this._client = client ?? throw new ArgumentNullException(nameof(client));
            this._serviceUri = serviceUri ?? throw new ArgumentNullException(nameof(client));
            _domainList = new List<CPanelDnsDomain>();
        }
        #endregion

        #region Properties

        public IList<CPanelDnsDomain> List
        {
            get { return _domainList; }
            set { _domainList = value; }
        }

        #endregion

        #region Methods
        public async Task ReadAsync()
        {
            // Login to site, to collect data.
            if (!_client.IsLoggedIn)
                await _client.LoginAsync();

            _domainList.Clear();
            var domainUriList = await _client.GetCPanelDnsDomainUriListAsync(_serviceUri);
            if (domainUriList != null)
            {
                foreach (var domainUri in domainUriList)
                {
                    var domain = new CPanelDnsDomain(_client, domainUri.Name, domainUri.Uri);
                    _domainList.Add(domain);
                    await domain.ResourceRecords.ReadAsync();
                }
            }
        }
        #endregion
    }
}