using System;
using System.Collections.Generic;
using System.Text;

namespace Tivo.Hme.Host.Services
{
    internal class HttpHandlerRegistryService : IHttpHandlerRegistryService
    {
        private HmeServer _server;
        private Dictionary<Uri, Action<HttpHandlerArgs>> _handlers = new Dictionary<Uri, Action<HttpHandlerArgs>>();

        public HttpHandlerRegistryService(HmeServer server)
        {
            _server = server;
            _server.NonApplicationRequestReceivedArgs += new EventHandler<NonApplicationRequestReceivedArgs>(_server_NonApplicationRequestReceivedArgs);
        }

        void _server_NonApplicationRequestReceivedArgs(object sender, NonApplicationRequestReceivedArgs e)
        {
            int prefixSize = _server.ApplicationPrefix.PathAndQuery.Length;
            var appRelative = new Uri(e.HttpRequest.RequestUri.OriginalString.Substring(prefixSize), UriKind.Relative);
            Action<HttpHandlerArgs> httpHandler = null;
            string registeredUri = null;
            if (!_handlers.TryGetValue(appRelative, out httpHandler))
            {
                int closestMatch = int.MaxValue;
                foreach (var entry in _handlers)
                {
                    if (appRelative.OriginalString.StartsWith(entry.Key.OriginalString))
                    {
                        int diff = appRelative.OriginalString.Length - entry.Key.OriginalString.Length;
                        if (closestMatch > diff)
                        {
                            closestMatch = diff;
                            httpHandler = entry.Value;
                            registeredUri = entry.Key.OriginalString;
                        }
                    }
                }
            }
            else
            {
                registeredUri = appRelative.OriginalString;
            }
            if (httpHandler != null)
            {
                httpHandler(new HttpHandlerArgs(e.HttpRequest, new HttpResponse(e.HttpRequest)) { RegisteredUri = registeredUri });
                // use null http response since httpHandler does all the processing.
                // can't set to null since that would indicate the event was unhandled.
                e.HttpResponse = NullHttpResponse.Instance;
            }
        }

        public void RegisterHandler(string relativeUri, Action<HttpHandlerArgs> handler)
        {
            if (string.IsNullOrEmpty(relativeUri))
                throw new ArgumentException("", "relativeUri");
            _handlers.Add(new Uri(relativeUri, UriKind.Relative), handler);
        }

        public void UnregisterHandler(string relativeUri)
        {
            if (string.IsNullOrEmpty(relativeUri))
                throw new ArgumentException("", "relativeUri");
            _handlers.Remove(new Uri(relativeUri, UriKind.Relative));
        }

        private class NullHttpResponse : Http.HttpResponse
        {
            public static NullHttpResponse Instance = new NullHttpResponse();

            private NullHttpResponse() { }

            public override void Write(System.IO.Stream responseStream)
            {
                // no-op
            }
        }
    }
}
