using System;
using System.Collections.Generic;
using System.Text;

namespace Tivo.Hme.Host.Services
{
    public interface IHttpHandlerRegistryService
    {
        void RegisterHandler(string relativeUri, Action<HttpHandlerArgs> handler);
        void UnregisterHandler(string relativeUri);
    }
}
