using System;
using System.Collections.Specialized;
using System.Net;

namespace NuGet
{
    class MonoProxyAuthenticationRequiredResponse : IHttpWebResponse
    {
        public MonoProxyAuthenticationRequiredResponse ()
        {
            Headers = new NameValueCollection ();
            StatusCode = HttpStatusCode.ProxyAuthenticationRequired;
        }

        public void Dispose ()
        {
        }

        public string AuthType { get; private set; }
        public NameValueCollection Headers { get; private set; }
        public Uri ResponseUri { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }
    }
}
