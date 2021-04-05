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
    public class MyIpClient
    {
        public async Task<IPAddress> GetMyIp_whatismyipaddress_com()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                CookieContainer = new CookieContainer()
            };
            HttpClient client = new HttpClient(handler);

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            client.DefaultRequestHeaders.Add("Accept-Language", "sr,en-US;q=0.7,en;q=0.3");
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");

            Uri baseUri = new Uri("https://whatismyipaddress.com/");

            client.BaseAddress = baseUri;
            client.DefaultRequestHeaders.Host = baseUri.Host;

            //client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(
            //    productName: System.Reflection.Assembly.GetExecutingAssembly().GetName().Name,
            //    productVersion: System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()));

            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:87.0) Gecko/20100101 Firefox/87.0"); // Jedi vnago bre!

            HttpResponseMessage googleResponse = await client.GetAsync(baseUri);
            if (googleResponse.StatusCode == HttpStatusCode.OK)
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                var contentStr = await googleResponse.Content.ReadAsStringAsync();
                doc.LoadHtml(contentStr);
                var myIpNode = doc.DocumentNode.SelectSingleNode("/html/body/div/div/div/div/div/article/div/div/div[1]/div/div[2]/div/div/div/div/div/div[2]/div[1]/div[1]/p[2]/span[2]/a");
                if (myIpNode != null)
                {
                    string myIpStr = myIpNode.InnerText.Trim();
                    return IPAddress.Parse(myIpStr);
                }
                else
                {
                    throw new Exception("Can't parse Google page.");
                }
            }
            else
            {
                throw new Exception($"Server response is not OK: {googleResponse.StatusCode}");
            }
        }
        public async Task<IPAddress> GetMyIp_ipv4_icanhazip_com()
        {            
            return await Task<IPAddress>.Run(()=> {
                var externalIp = new WebClient().DownloadString("https://ipv4.icanhazip.com/").TrimEnd();
                return IPAddress.Parse(externalIp); 
            });
        }
    }
}
