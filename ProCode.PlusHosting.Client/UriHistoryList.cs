using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProCode.PlusHosting.Client
{
    public class UriHistoryList : List<UriHistory>
    {
        #region Constants
        const int maxLength = 20;
        #endregion

        #region Methods
        public new void Add(UriHistory uriHistory)
        {
            if (Count > maxLength - 1)
                Remove(this.Last());
            base.Add(uriHistory);
        }

        public void Add(Uri uri)
        {
            Add(new UriHistory(uri));
        }
        #endregion
    }
}
