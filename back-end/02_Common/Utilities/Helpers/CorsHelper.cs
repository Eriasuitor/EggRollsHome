using System;
using Newegg.API.Common;
using Newegg.API.HttpExtensions;

namespace Newegg.MIS.API.Utilities.Helpers
{
    public static class CorsHelper
    {
        /// <summary>
        /// To Use This method for CORS, Integrated Mode must be used in IIS 7
        /// </summary>
        /// <param name="req"></param>
        /// <param name="res"></param>
        public static void AddResponseHeadersForCors(HttpRequestWrapper req, HttpResponseWrapper res)
        {
            var origin = req.Headers[HttpHeaders.Origin];
            if (origin != null)
            {
                var existingOrigin = res.Headers[HttpHeaders.AllowOrigin];
                if (null == existingOrigin ||
                    string.Compare(existingOrigin, origin, StringComparison.InvariantCultureIgnoreCase) != 0)
                {
                    res.AddHeader(HttpHeaders.AllowOrigin, origin);
                }
            }

            var requestHeaders = req.Headers[HttpHeaders.RequestHeaders];
            if (requestHeaders == null) return;

            var existingRequestHeaders = res.Headers[HttpHeaders.AllowHeaders];
            if (null != existingRequestHeaders &&
                string.Compare(existingRequestHeaders,
                               requestHeaders,
                               StringComparison.InvariantCultureIgnoreCase) == 0) return;

            res.AddHeader(HttpHeaders.AllowHeaders, requestHeaders);
        }
    }
}
