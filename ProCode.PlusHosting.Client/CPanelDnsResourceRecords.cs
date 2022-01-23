using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProCode.PlusHosting.Client
{
    public class CPanelDnsResourceRecords
    {
        #region Fields
        private readonly PlusHostingClient client;
        private readonly Uri domainUri;
        private List<CPanelDnsResourceRecord> _resourceRecordList;
        #endregion

        #region Constructor
        public CPanelDnsResourceRecords(PlusHostingClient client, Uri domainUri)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.domainUri = domainUri ?? throw new ArgumentNullException(nameof(domainUri));
            _resourceRecordList = new List<CPanelDnsResourceRecord>();
        }
        #endregion

        #region Properties
        public List<CPanelDnsResourceRecord> List { get { return _resourceRecordList; } }
        #endregion

        #region Methods
        public async Task ReadAsync()
        {
            // Login to site, to collect data.
            if (!client.IsLoggedIn)
                await client.LoginAsync();

            _resourceRecordList.Clear();
            var resourceRecordUriList = await client.GetCPanelDnsDomainResourceRecordListAsync(domainUri);
            if (resourceRecordUriList != null)
            {
                foreach (var resourceRecordUri in resourceRecordUriList)
                {
                    _resourceRecordList.Add(new CPanelDnsResourceRecord(client, domainUri, resourceRecordUri));
                }
            }
        }
        #endregion
    }
}