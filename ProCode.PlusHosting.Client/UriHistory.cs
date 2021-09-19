using System;

namespace ProCode.PlusHosting.Client
{
    public class UriHistory
    {
        #region Constructors
        public UriHistory(Uri uri, DateTime? time = null)
        {
            Uri = uri;
            Time = time ?? DateTime.Now;
        }
        #endregion

        #region Properties
        public Uri Uri { get; }
        public DateTime Time { get; }
        #endregion
    }
}