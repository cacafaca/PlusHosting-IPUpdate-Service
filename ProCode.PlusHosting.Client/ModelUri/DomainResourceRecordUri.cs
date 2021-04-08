using System;

namespace ProCode.PlusHosting.Client.ModelUri
{
    public class DomainResourceRecordUri
    {
        public string RecordType { get; set; }
        public string Name { get; set; }
        public string Ttl { get; set; }
        public string Data { get; set; }
        public string DomainId { get; set; }
        public Uri EditUri { get; set; }
        public Uri DeleteUri { get; set; }
        public override string ToString()
        {
            return $"Type={RecordType};Name={Name};Data={Data}";
        }
    }
}
