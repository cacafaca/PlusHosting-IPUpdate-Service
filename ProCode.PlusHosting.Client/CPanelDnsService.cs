using System;
using System.Collections.Generic;

namespace ProCode.PlusHosting.Client
{
    public class CPanelDnsService
    {
        #region Fields
        public string Name { get; }
        private readonly CPanelDnsDomains domains;
        private readonly Uri serviceUri;
        #endregion

        #region Constructors
        public CPanelDnsService(PlusHostingClient client, string name, Uri serviceUri)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            this.serviceUri = serviceUri ?? throw new ArgumentNullException(nameof(serviceUri));
            Name = name;
            domains = new CPanelDnsDomains(client, serviceUri);
        }
        #endregion

        #region Properties
        public CPanelDnsDomains Domains { get { return domains; } }
        #endregion

        #region Methods
        public override string ToString()
        {
            return $"Service: {Name} ({serviceUri})";
        }
        #endregion
    }
}