namespace ProCode.PlusHosting.IpUpdate.Service
{
    public class LoginInfoPoco
    {
        public class Rootobject
        {
            public PlusHostingLoginInfo PlusHostingLoginInfo { get; set; }
            public PlusHostingRecord[] PlusHostingRecords { get; set; }
            public MailSmtpInfo MailSmtpInfo { get; set; }
        }

        public class PlusHostingLoginInfo
        {
            public string User { get; set; }
            public string Pass { get; set; }
        }

        public class MailSmtpInfo
        {
            public string Server { get; set; }
            public int Port { get; set; }
            public bool EnableSsl { get; set; }
            public string User { get; set; }
            public string Pass { get; set; }
            public string ReportTo { get; set; }
        }

        public class PlusHostingRecord
        {
            public string ServiceName { get; set; }
            public string DomainName { get; set; }
            public Resourcerecord ResourceRecord { get; set; }
        }

        public class Resourcerecord
        {
            public string Type { get; set; }
            public string Name { get; set; }
        }

    }
}
