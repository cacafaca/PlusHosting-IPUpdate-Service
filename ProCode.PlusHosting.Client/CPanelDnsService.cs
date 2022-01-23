using System;

namespace ProCode.PlusHosting.Client
{
    public class CPanelDnsService
    {
        #region Fields
        private readonly CPanelDnsDomains _domains;
        private readonly Uri serviceUri;
        #endregion

        #region Constructors
        public CPanelDnsService(PlusHostingClient client, string name, Uri serviceUri)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            this.serviceUri = serviceUri ?? throw new ArgumentNullException(nameof(serviceUri));
            Name = name;
            _domains = new CPanelDnsDomains(client, serviceUri);
        }
        #endregion

        #region Properties
        public string Name { get; }
        public CPanelDnsDomains Domains { get { return _domains; } }
        #endregion

        #region Methods
        public override string ToString()
        {
            return $"Service: {Name} ({serviceUri})";
        }
        #endregion
    }
}