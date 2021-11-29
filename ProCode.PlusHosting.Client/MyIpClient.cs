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
    /// Public IP services:
    /// http://bot.whatismyipaddress.com/
    /// https://www.ipadresa.com/ - Powered by Plus Hosting! Probably this would be the best choice to use.
    /// https://ipv4.icanhazip.com/
    /// </summary>
    public class MyIpClient
    {
        public async Task<IPAddress> GetMyIp_ipv4_icanhazip_com()
        {
            return await Task.Run(() =>
            {
                var externalIp = new WebClient().DownloadString("https://ipv4.icanhazip.com/").Trim();
                return IPAddress.Parse(externalIp);
            });
        }

        /// <summary>
        /// This address belongs to Plus Hosting. They rented me a domain. :)
        /// </summary>
        /// <returns></returns>
        public async Task<IPAddress> GetMyIp_www_ipadresa_com()
        {
            return await Task.Run(() =>
            {
                var responseContent = new WebClient().DownloadString("https://www.ipadresa.com/").Trim();
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(responseContent);
                var publicIpAddressNode = doc.DocumentNode.SelectSingleNode("/h1");
                string publicIpAddress = null;
                if (publicIpAddressNode != null)
                {
                    publicIpAddress = publicIpAddressNode.InnerText.Trim();
                }
                return !string.IsNullOrEmpty(publicIpAddress) ? IPAddress.Parse(publicIpAddress) : null;
            });
        }

        public async Task<IPAddress> GetMyIp()
        {
            try
            {
                return await GetMyIp_www_ipadresa_com();
            }
            catch
            {
                return await GetMyIp_ipv4_icanhazip_com();  // Do not catch, this last attempt. It will be catch elsewhere.
            }
        }
    }
}
