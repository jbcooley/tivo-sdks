using System;
using System.Collections.Specialized;
using System.IO;

namespace Tivo.Hme.Host.Services
{
    public interface IHttpRequest
    {
        string Action { get; }
        string Protocol { get; }
        Uri RequestUri { get; }
        NameValueCollection Headers { get; }
        Stream Stream { get; }
    }
}
