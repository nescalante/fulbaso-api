using System.Web;

namespace Fulbaso.Common
{
    public static class HttpRequestBaseExtensions
    {
        /// <summary>
        /// Returns a boolean value indicating whether this request can render debugging
        /// information to the response such as exception details or profiling results.
        /// </summary>
        public static bool CanDisplayDebuggingDetails(this HttpRequestBase request)
        {
            return request.IsLocal;
        }
    }
}
