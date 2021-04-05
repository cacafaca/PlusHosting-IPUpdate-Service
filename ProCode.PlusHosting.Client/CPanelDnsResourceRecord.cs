using ProCode.PlusHosting.Client.ModelUri;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProCode.PlusHosting.Client
{
    public class CPanelDnsResourceRecord
    {
        #region Fields
        private readonly PlusHostingClient client;
        private readonly Uri domainUri;
        private readonly DomainResourceRecordUri resourceRecordUri;
        #endregion

        #region Constructors
        public CPanelDnsResourceRecord(PlusHostingClient client, Uri domainUri, DomainResourceRecordUri resourceRecordUri)
        {
            this.client = client;
            this.domainUri = domainUri;
            this.resourceRecordUri = resourceRecordUri;
        }
        #endregion

        #region Properties
        public string Name { get { return resourceRecordUri.Name; } }
        public string RecordType { get { return resourceRecordUri.RecordType; } }
        public string Ttl { get { return resourceRecordUri.Ttl; } }
        public string Data
        {
            get
            {
                return resourceRecordUri.Data;
            }
            set
            {
                resourceRecordUri.Data = value;
                client.UpdateCPanelDnsDomainResourceRecordAsync(resourceRecordUri, domainUri).Wait();
            }
        }
        #endregion

        #region Methods
        public override string ToString()
        {
            return $"Resource record: Type={RecordType}; Name={Name}; Data={Data.Replace("\r", " ").Replace("\n", " ")}";
        }
        #endregion
    }
}
