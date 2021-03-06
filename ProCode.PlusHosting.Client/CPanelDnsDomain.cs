using System;

namespace ProCode.PlusHosting.Client
{
    public class CPanelDnsDomain
    {
        #region Fields
        private readonly CPanelDnsResourceRecords _resourceRecords;
        private readonly Uri domainUri;
        #endregion

        #region Constructors
        public CPanelDnsDomain(PlusHostingClient client, string name, Uri domainUri)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            this.domainUri = domainUri ?? throw new ArgumentNullException(nameof(domainUri));
            Name = name;
            _resourceRecords = new CPanelDnsResourceRecords(client, domainUri);
        }
        #endregion

        #region Properties
        public string Name { get; }
        /// <summary>
        /// Contains a list of resource records like SOA, NS, A, CNAME, ...
        /// </summary>
        public CPanelDnsResourceRecords ResourceRecords { get { return _resourceRecords; } }
        #endregion

        #region Methods
        public override string ToString()
        {
            return $"Domain: {Name} ({domainUri})";
        }
        #endregion
    }
}