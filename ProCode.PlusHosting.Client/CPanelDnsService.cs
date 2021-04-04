using System;
using System.Collections.Generic;

namespace ProCode.PlusHosting.Client
{
    public class CPanelDnsService
    {
        public string Name { get; }
        public CPanelDnsService(string name, IList<CPanelDnsDomain> domainList)
        {
            Name = name;
        }
        public IList<CPanelDnsDomain> GetDomains()
        {
            return null;
        }
    }
}