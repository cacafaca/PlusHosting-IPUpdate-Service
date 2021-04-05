using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ProCode.PlusHosting.Client
{
    /// <summary>
    /// Public IP services
    /// http://bot.whatismyipaddress.com/
    /// </summary>
    public class MyIpClient
    {
        public async Task<IPAddress> GetMyIp_bot_whatismyipaddress_com()
        {
            return await Task.Run(() => {
                var externalIp = new WebClient().DownloadString("http://bot.whatismyipaddress.com/").Trim();
                return IPAddress.Parse(externalIp);
            });
        }
        public async Task<IPAddress> GetMyIp_ipv4_icanhazip_com()
        {            
            return await Task.Run(()=> {
                var externalIp = new WebClient().DownloadString("https://ipv4.icanhazip.com/").Trim();
                return IPAddress.Parse(externalIp); 
            });
        }
    }
}
