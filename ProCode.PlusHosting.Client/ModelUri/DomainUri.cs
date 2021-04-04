using System;

namespace ProCode.PlusHosting.Client.ModelUri
{
    public class DomainUri
    {
        public Uri Uri { get; }
        public string Name { get; }
        public DomainUri(Uri uri, string name)
        {
            Uri = uri;
            Name = name;
        }
    }
}