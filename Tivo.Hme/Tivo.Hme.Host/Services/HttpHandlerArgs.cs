using System;
using System.Collections.Generic;
using System.Text;

namespace Tivo.Hme.Host.Services
{
    public sealed class HttpHandlerArgs
    {
        public HttpHandlerArgs(IHttpRequest request, IHttpResponse response)
        {
            Request = request;
            Response = response;
        }

        public IHttpRequest Request { get; private set; }
        public IHttpResponse Response { get; private set; }
        public string RegisteredUri { get; set; }
    }
}
