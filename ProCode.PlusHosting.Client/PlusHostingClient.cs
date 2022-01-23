using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace ProCode.PlusHosting.Client
{
    public class PlusHostingClient
    {
        #region Constants
        const int maxRetrysForUrl = 1;
        #endregion

        #region Fields
        HttpClientEnhanced client;
        readonly PlusHostingUriDictionary uriDictionary;
        readonly UserCredential userCredential;
        #endregion

        #region Constrictors
        public PlusHostingClient(UserCredential userCredential)
        {
            isLoggedIn = false;
            this.userCredential = userCredential;
            uriDictionary = new PlusHostingUriDictionary();
            client = GetNewClient();
        }
        #endregion

        private HttpClientEnhanced GetNewClient()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                CookieContainer = new CookieContainer()
            };
            client = new HttpClientEnhanced(handler);

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            client.DefaultRequestHeaders.Add("Accept-Language", "sr,en-US;q=0.7,en;q=0.3");
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");

            client.BaseAddress = uriDictionary.GetBase();
            client.DefaultRequestHeaders.Host = uriDictionary.GetBase().Host;

            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(
                productName: System.Reflection.Assembly.GetExecutingAssembly().GetName().Name,
                productVersion: System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()));

            return client;
        }

        /// <summary>
        /// Login.
        /// </summary>
        /// <returns></returns>
        public async Task LoginAsync()
        {
            if (IsLoggedIn == false)
            {
                var temporaryToken = await GetSecurityTokenAsync();

                // Generate content (login params).
                var requestPayload = string.Join("&",
                    "username=" + Uri.EscapeDataString(userCredential.Username),
                    "password=" + userCredential.Password,
                    "action=login",
                    "security_token=" + temporaryToken);
                HttpContent content = new StringContent(requestPayload);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");   // This is important!

                // Send POST command to login, because it needs login data to be secured.
                HttpResponseMessage responseMsg = await client.PostAsync(uriDictionary.GetLoginUri(), content);
                if (responseMsg.StatusCode == HttpStatusCode.OK)
                {
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    var responseContent = await responseMsg.Content.ReadAsStringAsync();
                    doc.LoadHtml(responseContent);

                    var possibleRcogintions = doc.DocumentNode.SelectNodes("//small[@class='text-gray']");
                    if (possibleRcogintions != null && possibleRcogintions.Where(x => x.InnerText.Trim() == "Dobrodošao").Count() > 0)
                    {
                        // Logged in successfully!!!
                        isLoggedIn = true;
                        Util.Trace.WriteLine("Logged in.");
                    }
                    else
                    {

                        if (doc.DocumentNode.SelectNodes("//script[@type='text/javascript']")?.Where(node => node.InnerText.Contains("ili lozinka su neta")).FirstOrDefault() != null)
                        {
                            throw new Exception("Wrong user and pass.");
                        }
                        else
                        {
                            throw new Exception("Can't determine if Login succeeded.");
                        }
                    }
                }
                else
                {
                    throw new ClientException("Failed logging. Response status: " + responseMsg.StatusCode, client, null);
                }
            }
        }

        /// <summary>
        /// Logout.
        /// </summary>
        /// <returns></returns>
        public async Task LogoutAsync()
        {
            if (IsLoggedIn == true)
            {
                // Send GET command to logout.
                HttpResponseMessage responseMsg = await client.GetAsync(uriDictionary.GetLogoutUri());
                if (responseMsg.StatusCode == HttpStatusCode.OK)
                {
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    var contentStr = await responseMsg.Content.ReadAsStringAsync();
                    doc.LoadHtml(contentStr);
                    var navigationNode = doc.DocumentNode.SelectNodes("/html/body/nav[1]");
                    if (navigationNode != null && navigationNode[0].ChildNodes.Where(node => node.InnerText.Contains("Ulogujte se / Register")).Count() > 0)
                    {
                        // Logged out successfully!!!
                        isLoggedIn = false;
                        Util.Trace.WriteLine("Logged out.");
                    }
                    else
                    {
                        throw new Exception("Can't determine if Logout succeeded.");
                    }
                }
                else
                {
                    throw new Exception($"Response Status Code in not OK. StatusCode={responseMsg.StatusCode}.");
                }
            }
        }

        string securityToken = string.Empty;

        /// <summary>
        /// Get security token from tag: <input type="hidden" name="security_token" value="62ee407a96e7e4ca9357319a7585e551" />
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetSecurityTokenAsync()
        {
            HttpResponseMessage responseMsg = await client.GetAsync(uriDictionary.GetBase());
            string token;
            if (responseMsg.StatusCode == HttpStatusCode.OK)
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                var contentStr = await responseMsg.Content.ReadAsStringAsync();
                doc.LoadHtml(contentStr);
                const string securityTokenXPath = "//input[@name='security_token']";
                var tokenNode = doc.DocumentNode.SelectSingleNode(securityTokenXPath);
                if (tokenNode != null)
                {
                    token = tokenNode.Attributes["value"].Value;
                }
                else
                {
                    throw new ClientException("Can't find XPath: " + securityTokenXPath, client, null);
                }
            }
            else
            {
                throw new ClientException("Failed reading base page to get security token. Response status: " + responseMsg.StatusCode, client, null);
            }

            return token;
        }



        /// <summary>
        /// Gets list of cPanels DNS from https://portal.plus.rs/clientarea/services/cpaneldns/. Probably will always be only one.
        /// </summary>
        public async Task<IList<ModelUri.ServiceUri>> GetCPanelDnsServiceUriListAsync()
        {
            List<ModelUri.ServiceUri> cpanelDnsServiceUriList = new List<ModelUri.ServiceUri>();

            // Send GET command to fetch list of cPanels. 
            HttpResponseMessage responseMsg = await client.GetAsync(uriDictionary.GetCPanelListUri());
            if (responseMsg.StatusCode == HttpStatusCode.OK)
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                var contentStr = await responseMsg.Content.ReadAsStringAsync();
                doc.LoadHtml(contentStr);
                var cpanelTableBodyNode = doc.DocumentNode.SelectSingleNode("//tbody[@id=\"updater\"]");
                if (cpanelTableBodyNode != null)
                {
                    foreach (var row in cpanelTableBodyNode.ChildNodes.Where(x => x.NodeType == HtmlAgilityPack.HtmlNodeType.Element))
                    {
                        var productNode = row.ChildNodes.Where(el => el.NodeType == HtmlAgilityPack.HtmlNodeType.Element).FirstOrDefault();
                        if (productNode != null)
                        {
                            var anchorNode = productNode.SelectSingleNode(".//a");  // dot(.) in ".//a" means search from current node, and not from root document node.
                            if (anchorNode != null)
                            {
                                var cpanelDnsNameNode = productNode.SelectSingleNode(".//i[2]");
                                if (cpanelDnsNameNode != null)
                                {
                                    cpanelDnsServiceUriList.Add(new ModelUri.ServiceUri(new Uri(uriDictionary.GetBase(), anchorNode.Attributes["href"].Value), cpanelDnsNameNode.InnerText.Trim()));
                                }
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("Can't find table with DNS services.");
                }
            }
            else
            {
                throw new Exception($"Response Status Code in not OK. StatusCode={responseMsg.StatusCode}.");
            }

            return cpanelDnsServiceUriList;
        }

        /// <summary>
        /// Get Domain list for requested cPanel Uri.
        /// </summary>
        /// <param name="serviceUri"></param>
        /// <returns></returns>
        public async Task<IList<ModelUri.DomainUri>> GetCPanelDnsDomainUriListAsync(Uri serviceUri)
        {
            IList<ModelUri.DomainUri> cpanelDnsDomainList = new List<ModelUri.DomainUri>();

            // Send GET command to fetch list of Domains for cPanel.

            HttpResponseMessage responseMsg = await client.GetAsync(serviceUri);
            if (responseMsg.StatusCode == HttpStatusCode.OK)
            {
                var contentStr = await responseMsg.Content.ReadAsStringAsync();
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(contentStr);
                var cpanelTableBodyNode = doc.DocumentNode.SelectSingleNode("/html/body/div[4]/section/div/form/table[1]/tbody[1]");
                if (cpanelTableBodyNode != null)
                {
                    foreach (var rowNode in cpanelTableBodyNode.ChildNodes.Where(x => x.NodeType == HtmlAgilityPack.HtmlNodeType.Element))
                    {
                        var domainNode = rowNode.SelectSingleNode(".//td[2]");
                        if (domainNode != null)
                        {
                            var anchorNode = domainNode.SelectSingleNode(".//a");  // dot(.) in ".//a" means search from current node, and not from root document node.
                            if (anchorNode != null)
                            {
                                cpanelDnsDomainList.Add(new ModelUri.DomainUri(new Uri(uriDictionary.GetBase(), anchorNode.Attributes["href"].Value), anchorNode.InnerText.Trim()));
                            }
                        }

                    }
                }
                else
                {
                    throw new Exception("Can't find table with DNS domains.");
                }

                if (securityToken == string.Empty)
                {
                    var securityTokenNode = doc.DocumentNode.SelectSingleNode("//*[@name='security_token']");
                    if (securityTokenNode != null)
                    {
                        securityToken = securityTokenNode.Attributes["value"].Value;
                    }
                }
            }
            else
            {
                throw new Exception($"Response Status Code in not OK. StatusCode={responseMsg.StatusCode}.");
            }

            return cpanelDnsDomainList;
        }

        /// <summary>
        /// Retrieve list of records of type SOA, NS, A, AAAA, CNAME, ...
        /// </summary>
        /// <param name="domainUri"></param>
        /// <returns></returns>
        public async Task<IList<ModelUri.DomainResourceRecordUri>> GetCPanelDnsDomainResourceRecordListAsync(Uri domainUri)
        {
            IList<ModelUri.DomainResourceRecordUri> cpanelDnsDomainResourceRecordList;

            // Send GET command to fetch list of Domains for cPanel.
            int retry = 1;
            while (true)
            {
                try
                {
                    HttpResponseMessage responseMsg = await client.GetAsync(domainUri);
                    if (responseMsg.StatusCode == HttpStatusCode.OK)
                    {
                        var contentStr = await responseMsg.Content.ReadAsStringAsync();
                        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                        doc.LoadHtml(contentStr);

                        cpanelDnsDomainResourceRecordList = GenerateResourceRecordListFromHtml(domainUri, doc);
                        break;  // Break out of the loop if all good.
                    }
                    else
                    {
                        throw new Exception($"Response Status Code in not OK. StatusCode={responseMsg.StatusCode}.");
                    }
                }
                catch (ClientException ex)
                {
                    if (retry < maxRetrysForUrl)
                        retry++;
                    else
                        throw ex;   // Give a chance three times to read and then re-throw exception.
                }
            }

            return cpanelDnsDomainResourceRecordList ?? new List<ModelUri.DomainResourceRecordUri>();
        }

        public async Task<IList<ModelUri.DomainResourceRecordUri>> UpdateCPanelDnsDomainResourceRecordAsync(ModelUri.DomainResourceRecordUri updateResourceRecord, Uri domainUri)
        {
            IList<ModelUri.DomainResourceRecordUri> cpanelDnsDomainResourceRecordList;

            string contentStr = string.Join("&", new string[] {
                "type=" + Uri.EscapeDataString(updateResourceRecord.RecordType),
                "dom=" + Uri.EscapeDataString(updateResourceRecord.DomainId),
                "name=" + Uri.EscapeDataString(updateResourceRecord.Name),
                "ttl=" + Uri.EscapeDataString(updateResourceRecord.Ttl),
                $"content{Uri.EscapeDataString("[0]")}={Uri.EscapeDataString(updateResourceRecord.Data)}",
                "submit=" + Uri.EscapeDataString("Pošalji"),
                "security_token=" + securityToken });
            HttpContent httpContent = new StringContent(contentStr);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");   // This is important!

            HttpResponseMessage responseMsg = await client.PostAsync(updateResourceRecord.EditUri.AbsolutePath, httpContent);
            if (responseMsg.StatusCode == HttpStatusCode.OK)
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                var responseContentStr = await responseMsg.Content.ReadAsStringAsync();
                doc.LoadHtml(responseContentStr);

                cpanelDnsDomainResourceRecordList = GenerateResourceRecordListFromHtml(domainUri, doc);
                var actualValue = cpanelDnsDomainResourceRecordList.Where(record => record.RecordType == updateResourceRecord.RecordType && record.Name == updateResourceRecord.Name).FirstOrDefault()?.Data;
                if (actualValue != null)
                {
                    if (actualValue != updateResourceRecord.Data)
                    {
                        throw new Exception($"Update failed! Data field is different. (sent value) '{updateResourceRecord.Data}' <> '{actualValue}' (actual value).");
                    }
                    else
                    {
                        Util.Trace.WriteLine($"Resource record updated: {updateResourceRecord}");
                    }
                }
                else
                    throw new Exception($"Can't find record (Type, Name)=({updateResourceRecord.RecordType}, {updateResourceRecord.Name}).");
            }
            else
            {
                throw new Exception($"Response Status Code in not OK. StatusCode={responseMsg.StatusCode}.");
            }

            return cpanelDnsDomainResourceRecordList ?? new List<ModelUri.DomainResourceRecordUri>();
        }

        IList<ModelUri.DomainResourceRecordUri> GenerateResourceRecordListFromHtml(Uri domainUri, HtmlAgilityPack.HtmlDocument doc)
        {
            IList<ModelUri.DomainResourceRecordUri> cpanelDnsDomainResourceRecordList = new List<ModelUri.DomainResourceRecordUri>();

            var domainXPath = $"//form[@action='{domainUri.LocalPath.TrimStart('/')}' and @method='POST']";
            var allTablesFormNode = doc.DocumentNode.SelectSingleNode(domainXPath); // <form> tag is container for all tables (SOA, NS, ...)
            if (allTablesFormNode != null)
            {
                foreach (var tableNode in allTablesFormNode.ChildNodes.Where(x => x.NodeType == HtmlAgilityPack.HtmlNodeType.Element))
                {
                    var tableHeaderNode = tableNode.SelectSingleNode(".//thead");
                    if (tableHeaderNode != null)
                    {
                        var tableBodyNode = tableNode.SelectSingleNode(".//tbody[2]");
                        if (tableBodyNode != null)
                        {
                            // Iterate through rows of table but only ones that have 6 columns. Other can contain other non-important data, like "Add new record" link at the end of the each table.
                            foreach (var rowNode in tableBodyNode.ChildNodes.Where(node => node.NodeType == HtmlAgilityPack.HtmlNodeType.Element &&
                                node.ChildNodes.Where(childNode => childNode.NodeType == HtmlAgilityPack.HtmlNodeType.Element).Count() == 6))
                            {
                                var editUriStr = rowNode.SelectSingleNode(".//td[6]")?.SelectSingleNode(".//a[@title='Urediti']")?.Attributes["href"].Value;
                                var deleteUriStr = rowNode.SelectSingleNode(".//td[6]")?.SelectSingleNode(".//a[@title='Izbrisati']")?.Attributes["href"].Value;

                                cpanelDnsDomainResourceRecordList.Add(new ModelUri.DomainResourceRecordUri()
                                {
                                    Name = rowNode.SelectSingleNode(".//td[1]").InnerText.Trim(),
                                    Ttl = rowNode.SelectSingleNode(".//td[3]").InnerText.Trim(),
                                    RecordType = rowNode.SelectSingleNode(".//td[4]").InnerText.Trim(),
                                    Data = rowNode.SelectSingleNode(".//td[5]").InnerText.Trim(),
                                    EditUri = editUriStr != null ? new Uri(uriDictionary.GetBase(), editUriStr) : null,
                                    DeleteUri = deleteUriStr != null ? new Uri(uriDictionary.GetBase(), deleteUriStr) : null,
                                    DomainId = HttpUtility.ParseQueryString(domainUri.AbsoluteUri).Get("domain_id")
                                });
                            }
                        }
                    }
                }
            }
            else
            {
                throw new ClientException($"Can't find XPath='{domainXPath}'.", client, doc.DocumentNode.OuterHtml);
            }

            return cpanelDnsDomainResourceRecordList;
        }

        private bool isLoggedIn;

        public bool IsLoggedIn
        {
            get { return isLoggedIn; }
        }

    }
}
