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
        #endregion

        #region Constructor
        public CPanelDnsResourceRecords(PlusHostingClient client, Uri domainUri)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.domainUri = domainUri ?? throw new ArgumentNullException(nameof(domainUri));
        }
        #endregion

        public async Task<IList<CPanelDnsResourceRecord>> GetResourceRecirdListAsync()
        {
            // Login to site, to collect data.
            if (!client.IsLoggedIn)
                await client.LoginAsync();

            IList<CPanelDnsResourceRecord> resourceRecordList = new List<CPanelDnsResourceRecord>();
            var resourceRecordUriList = await client.GetCPanelDnsDomainResourceRecordListAsync(domainUri);
            if (resourceRecordUriList != null)
            {
                foreach (var resourceRecordUri in resourceRecordUriList)
                {
                    resourceRecordList.Add(new CPanelDnsResourceRecord(client, domainUri, resourceRecordUri));
                }
            }

            return resourceRecordList;
        }
    }
}