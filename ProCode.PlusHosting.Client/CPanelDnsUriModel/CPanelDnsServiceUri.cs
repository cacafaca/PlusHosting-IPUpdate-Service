using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProCode.PlusHosting.Client
{
    public class CPanelDnsServiceUri
    {
        public Uri Uri { get;  }
        public string Name { get;  }
        public CPanelDnsServiceUri(Uri uri, string name)
        {
            Uri = uri;
            Name = name;
        }
    }
}
