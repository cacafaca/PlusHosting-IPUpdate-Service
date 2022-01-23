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
        public const string TypeA = "A";

        private readonly PlusHostingClient _client;
        private readonly Uri domainUri;
        private readonly DomainResourceRecordUri _resourceRecordUri;
        #endregion

        #region Constructors
        public CPanelDnsResourceRecord(PlusHostingClient client, Uri domainUri, DomainResourceRecordUri resourceRecordUri)
        {
            this._client = client;
            this.domainUri = domainUri;
            this._resourceRecordUri = resourceRecordUri;
        }
        #endregion

        #region Properties
        public string Name { get { return _resourceRecordUri.Name; } }
        public string RecordType { get { return _resourceRecordUri.RecordType; } }
        public string Ttl { get { return _resourceRecordUri.Ttl; } }
        public string Data
        {
            get
            {
                return _resourceRecordUri.Data;
            }
            set
            {
                _resourceRecordUri.Data = value;
                _client.UpdateCPanelDnsDomainResourceRecordAsync(_resourceRecordUri, domainUri).Wait();
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
