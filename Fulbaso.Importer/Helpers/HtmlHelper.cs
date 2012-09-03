using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fulbaso.Importer
{
    internal static class HtmlHelper
    {
        internal static string FindFirstByClass(this string html, string css)
        {
            string result = "";
            var init = html.IndexOf("class=\"" + css + "\"");

            while (init > 0)
            {
                if (html.Substring(init, 1) == "<") return html.Substring(init);
                init--;
            }

            return result;
        }

        internal static string FindFirstByAttr(this string html, string attr)
        {
            var aux = html.Substring(html.IndexOf(attr + "=\""));
            aux = aux.Substring(aux.IndexOf("=") + 2);
            aux = aux.Substring(0, aux.IndexOf('\"'));
            
            return aux;
        }

        internal static string IgnoreFirst(this string html)
        {
            return html.Substring(html.Substring(1).IndexOf("<"));
        }
    }
}
