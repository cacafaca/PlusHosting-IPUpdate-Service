using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProCode.PlusHosting.Client
{
    public class CPanelDnsServiceList : List<CPanelDnsService>
    {
        public PlusHostingClient Client { get; }
        public CPanelDnsServiceList(PlusHostingClient client)
        {
            this.Client = client ?? throw new ArgumentNullException(nameof(client));            
        }
        
        public async void LoadAsync()
        {
            Clear();
            AddRange(await GetServiceListAsync());
        }

        async Task<IList<CPanelDnsService>> GetServiceListAsync()
        {
            IList<CPanelDnsService> serviceList = new List<CPanelDnsService>();

            if (!Client.IsLoggedIn) await Client.LoginAsync();
            var serviceUriList = await Client.GetCPanelDnsServiceUriListAsync();
            if (serviceUriList != null)
                foreach (var serviceUri in serviceUriList)
                {
                    var domainUriList = await Client.GetCPanelDnsDomainUriListAsync(serviceUri.Uri);
                    //serviceList.Add(new CPanelDnsService(serviceUri.Name, ));
                }
            return serviceList;
        }

    }
}