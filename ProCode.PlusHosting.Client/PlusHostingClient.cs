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
    public class PlusHostingClient
    {
        #region Constants
        #endregion

        HttpClient client;
        readonly UriDictionary uriDictionary;
        readonly UserCredential userCredential;

        public PlusHostingClient(UserCredential userCredential)
        {
            this.userCredential = userCredential;
            uriDictionary = new UriDictionary();
            client = GetNewClient();
        }

        private HttpClient GetNewClient()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                CookieContainer = new CookieContainer()
            };
            client = new HttpClient(handler);

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            client.DefaultRequestHeaders.Add("Accept-Language", "sr,en-US;q=0.7,en;q=0.3");
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
            var token = await GetSecurityTokenAsync();

            // Generate content (login params).
            var requestPayload = $"username={Uri.EscapeDataString(userCredential.GetUsername())}&password={userCredential.GetPassword()}&action=login&security_token={token}";
            HttpContent content = new StringContent(requestPayload);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");   // This is important!

            // Send POST command to login, because it needs login data to be secured.
            HttpResponseMessage responseMsg = await client.PostAsync(uriDictionary.GetLoginUri(), content);
            if (responseMsg.StatusCode == HttpStatusCode.OK)
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                var responseContent = await responseMsg.Content.ReadAsStringAsync();
                doc.LoadHtml(responseContent);

                var logoutAnchor = doc.DocumentNode.SelectSingleNode("//a[@href='?action=logout' and @class='main-menu-login']");
                if (logoutAnchor != null && logoutAnchor.InnerText.Trim().ToLower() == "logout")
                {
                    // Logged in successfully!!!
                }
                else
                {
                    var loginAnchor = doc.DocumentNode.SelectSingleNode("//a[@href='https://portal.plus.rs/root' and @class='main-menu-login']");
                    if (loginAnchor != null && loginAnchor.InnerText.Trim().ToLower() == "login")
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
                throw new NotImplementedException("Throw some meaningful exception.");
            }
        }

        /// <summary>
        /// Logout.
        /// </summary>
        /// <returns></returns>
        public async Task LogoutAsync()
        {
            // Send GET command to logout.
            HttpResponseMessage responseMsg = await client.GetAsync(uriDictionary.GetLogoutUri());
            if (responseMsg.StatusCode == HttpStatusCode.OK)
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                var contentStr = await responseMsg.Content.ReadAsStringAsync();
                doc.LoadHtml(contentStr);
                var loginAnchor = doc.DocumentNode.SelectSingleNode("//a[@href='https://portal.plus.rs/root' and @class='main-menu-login']");
                if (loginAnchor != null && loginAnchor.InnerText.Trim().ToLower() == "login")
                {
                    // Logged out successfully!!!
                }
                else
                {
                    throw new Exception("Can't determine if Login succeeded.");
                }
            }
            else
            {
                throw new Exception($"Response Status Code in not OK. StatusCode={responseMsg.StatusCode}.");
            }
        }

        /// <summary>
        /// Get security token from tag: <input type="hidden" name="security_token" value="62ee407a96e7e4ca9357319a7585e551" />
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetSecurityTokenAsync()
        {
            string token = string.Empty;

            HttpResponseMessage responseMsg = await client.GetAsync(uriDictionary.GetLoginUri());
            if (responseMsg.StatusCode == HttpStatusCode.OK)
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                var contentStr = await responseMsg.Content.ReadAsStringAsync();
                doc.LoadHtml(contentStr);
                var tokenNode = doc.DocumentNode.SelectSingleNode("//input[@name='security_token']");
                if (tokenNode != null)
                {
                    token = tokenNode.Attributes["value"].Value;
                }
            }
            else
            {

            }

            return token;
        }

        /// <summary>
        /// Gets list of cPanes from https://portal.plus.rs/clientarea/services/cpaneldns/. Probably will always be only one.
        /// </summary>
        public void GetCPanelDnsList()
        {

        }
    }
}
