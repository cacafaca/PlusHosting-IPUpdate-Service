using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProCode.PlusHosting.Client
{
    public class CPanelDns
    {
        private PlusHostingClient Client { get; }
        public CPanelDns(UserCredential userCredential)
        {
            Client = new PlusHostingClient(userCredential);

            services = new CPanelDnsServiceList(Client);
        }

        private readonly CPanelDnsServiceList services;
        public CPanelDnsServiceList Services { get { return services; } }

        /// <summary>
        /// After instantiating class you need to call LoadAsync method to read the data from PlusHosting site.
        /// </summary>
        public async void LoadAsync()
        {
            await Task.Run(() => services.LoadAsync());
        }
    }
}
