using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Text;

namespace YSharp.SDK
{
    public static class HttpRequestExtension
    {
        public static string ToAbsolutelyDomain(this Microsoft.AspNetCore.Http.HttpRequest request)
        {
            return string.Format("{0}://{1}", request.Scheme, request.Host.Value);
        }
    }
}
