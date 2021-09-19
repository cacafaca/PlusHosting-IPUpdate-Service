using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProCode.PlusHosting.Client
{
    public class HttpClientEnhanced : HttpClient
    {
        #region Fields
        UriHistoryList uriHistoryList = new UriHistoryList();
        #endregion

        #region Constructors
        public HttpClientEnhanced(HttpMessageHandler handler) : base(handler) { }
        #endregion

        #region Properties
        public UriHistoryList UriHistory { get { return uriHistoryList; } }
        #endregion

        #region Methods
        /// <summary>
        /// Calls base function but logs URI in history list.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        new public async Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content)
        {
            uriHistoryList.Add(requestUri);
            return await base.PostAsync(requestUri, content);
        }

        /// <summary>
        /// Calls base function but logs URI in history list.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        new public async Task<HttpResponseMessage> GetAsync(Uri requestUri)
        {
            uriHistoryList.Add(requestUri);
            return await base.GetAsync(requestUri);
        }
        #endregion
    }
}
