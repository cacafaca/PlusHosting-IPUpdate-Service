using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProCode.PlusHosting.Client
{
    public class CPanelDnsResourceRecord
    {
        #region Properties
        public string Name { get; set; }
        public string RecordType { get; set; }
        public string Ttl { get; set; }
        public string Data { get; set; }
        #endregion

        #region Methods
        public override string ToString()
        {
            return $"Resource record: Type={RecordType}; Name={Name}; Data={Data.Replace("\r", " ").Replace("\n", " ")}";
        }
        #endregion
    }
}
