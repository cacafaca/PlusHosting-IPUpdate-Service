using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProCode.PlusHosting.Client.ModelUri
{
    public class ServiceUri
    {
        public Uri Uri { get;  }
        public string Name { get;  }
        public ServiceUri(Uri uri, string name)
        {
            Uri = uri;
            Name = name;
        }
        public override string ToString()
        {
            return $"{Name}({Uri})";
        }
    }
}
