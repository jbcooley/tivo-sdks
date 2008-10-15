using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;

namespace Tivo.Hme.Host.Services
{
    public interface IHttpResponse
    {
        Stream GetResponseStream(int statusCode, NameValueCollection headers);
    }
}
