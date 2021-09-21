using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProCode.PlusHosting.Client
{
    public class ClientException : Exception
    {
        #region Fields
        string history;
        string html;
        #endregion

        #region Constructors
        public ClientException(string message, HttpClientEnhanced client, string html) : base(message)
        {
            history = string.Join(Environment.NewLine, client.UriHistory.OrderByDescending(uh => uh.Time).Select(uh => $"{uh.Uri.AbsoluteUri} ({uh.Time})"));
            this.html = html;
        }
        #endregion

        #region Methods
        public override string ToString()
        {
            return
$@"*** Message ***
{Message}

*** Uri history ***
{history}

*** Html source ***
{html}

*** Stack trace ***
{StackTrace}";            
        }
        #endregion
    }
}
