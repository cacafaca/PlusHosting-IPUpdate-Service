using System;

namespace ProCode.PlusHosting.Client
{
    public class PlusHostingUriDictionary
    {
        #region Constants
        const string baseUriStr = "https://portal.plus.rs";
        #endregion

        readonly Uri baseUri;
        public PlusHostingUriDictionary()
        {
            baseUri = new Uri(baseUriStr);
        }
        internal Uri GetLoginUri()
        {
            return new Uri(baseUri, "/index.php");
        }

        internal Uri GetLogoutUri()
        {
            return new Uri(baseUri, "/?action=logout");
        }

        public Uri GetBase()
        {
            return baseUri;
        }

        public Uri GetCPanelListUri()
        {
            return new Uri(baseUri, "/clientarea/services/cpaneldns/");
        }
    }
}
