using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Tivo.Hme.Host
{
    public class HttpRequestReceivedArgs : EventArgs
    {
        private HttpRequest _httpRequest;

        public HttpRequestReceivedArgs(HttpRequest httpRequest)
        {
            _httpRequest = httpRequest;
        }

        public HttpRequest HttpRequest
        {
            get { return _httpRequest; }
        }
    }

    public class HttpServer
    {
        private TcpListener _listener;

        public HttpServer(IPAddress localAddress, int listenPort)
        {
            _listener = new TcpListener(localAddress, listenPort);
        }

        public void Start()
        {
            _listener.Start();
            _listener.BeginAcceptTcpClient(OnConnectionReceived, null);
        }

        public void Stop()
        {
            _listener.Stop();
        }

        public EventHandler<HttpRequestReceivedArgs> HttpRequestReceived;

        protected virtual void OnHttpRequestReceived(HttpRequestReceivedArgs e)
        {
            EventHandler<HttpRequestReceivedArgs> handler = HttpRequestReceived;
            if (handler != null)
                handler(this, e);
        }

        private void OnConnectionReceived(IAsyncResult asyncResult)
        {
            try
            {
                TcpClient client = _listener.EndAcceptTcpClient(asyncResult);
                HttpRequest request = new HttpRequest(client);

                // start accepting next one before raising event in case the event handlers take too long
                _listener.BeginAcceptTcpClient(OnConnectionReceived, null);
                OnHttpRequestReceived(new HttpRequestReceivedArgs(request));
            }
            catch (SocketException)
            {
                // ignore socket exceptions.  Just don't try to accept another connection
            }
        }
    }
}
