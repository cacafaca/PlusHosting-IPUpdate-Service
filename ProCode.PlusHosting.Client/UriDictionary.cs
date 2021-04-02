using System;

namespace ProCode.PlusHosting.Client
{
    public class UriDictionary
    {
        #region Constants
        const string baseUriStr = "https://portal.plus.rs";
        #endregion

        readonly Uri baseUri;
        public UriDictionary()
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
        
    }
}
