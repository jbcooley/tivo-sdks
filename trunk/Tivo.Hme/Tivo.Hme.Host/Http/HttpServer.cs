// Copyright (c) 2008 Josh Cooley

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Tivo.Hme.Host.Http
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
                HttpRequest request = null;
                try
                {
                    request = new HttpRequest(client);
                }
                catch (NotSupportedException)
                {
                    // this can happen when a bad request comes in
                    // leave request as null, close the client
                    // and accept another request.
                    NetworkStream stream = client.GetStream();
                    stream.Close();
                    client.Close();
                }

                // start accepting next one before raising event in case the event handlers take too long
                _listener.BeginAcceptTcpClient(OnConnectionReceived, null);
                if (request != null)
                {
                    OnHttpRequestReceived(new HttpRequestReceivedArgs(request));
                }
            }
            catch (SocketException)
            {
                // ignore socket exceptions.  Just don't try to accept another connection
            }
        }
    }
}
