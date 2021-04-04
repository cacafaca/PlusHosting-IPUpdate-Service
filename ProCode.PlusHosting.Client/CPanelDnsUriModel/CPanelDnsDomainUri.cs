using System;

namespace ProCode.PlusHosting.Client
{
    public class CPanelDnsDomainUri
    {
        public Uri Uri { get; }
        public string Name { get; }
        public CPanelDnsDomainUri(Uri uri, string name)
        {
            Uri = uri;
            Name = name;
        }
    }
}