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
using System.Text;
using System.Net;
using System.Threading;
using Tivo.Hme.Host.Http;
using System.Diagnostics;

namespace Tivo.Hme.Host
{
    public class HmeApplicationConnectedEventArgs : EventArgs
    {
        private Application _application;
        private Uri _baseUri;

        public Application Application
        {
            get { return _application; }
            set { _application = value; }
        }

        public Uri BaseUri
        {
            get { return _baseUri; }
            set { _baseUri = value; }
        }
	
    }

    public class NonApplicationRequestReceivedArgs : HttpRequestReceivedArgs
    {
        private HttpResponse _response;

        public NonApplicationRequestReceivedArgs(HttpRequest request)
            : base(request)
        {
        }

        public HttpResponse HttpResponse
        {
            get { return _response; }
            set { _response = value; }
        }
    }

    /// <summary>
    /// Server Options for application
    /// </summary>
    [Flags]
    public enum HmeServerOptions
    {
        /// <summary>
        /// Advertised through bonjour
        /// </summary>
        AdvertiseOnLocalNetwork = 0,
        /// <summary>
        /// Don't publish this on bonjour
        /// </summary>
        NoAdvertise = 1
    }

    /// <summary>
    /// Manages sockets for new application connections.  Uses asyncronous io for
    /// connecting and reading from the sockets.  Has one thread to watch for events
    /// and commands for all applications.  Uses thread pool to send commands and
    /// raise events.
    /// </summary>
    public class HmeServer
    {
        private string _name;
        private Uri _applicationPrefix;
        private short _usePort;
        private Mono.Zeroconf.RegisterService _service;
        private bool _started;
        private bool _advertise;
        private IHmeApplicationPump _pump;

        public HmeServer(string name, Uri applicationPrefix, HmeServerOptions options, IHmeApplicationPump pump)
        {
            _name = name;
            _applicationPrefix = applicationPrefix;
            _pump = pump;

            _usePort = 7688;
            if (applicationPrefix.IsAbsoluteUri)
            {
                _usePort = (short)applicationPrefix.Port;
            }
            lock (_portServers)
            {
                HmePortServer portServer;
                if (!_portServers.TryGetValue(_usePort, out portServer))
                {
                    portServer = new HmePortServer(_usePort);
                    _portServers.Add(_usePort, portServer);
                }
                portServer.HttpServer.HttpRequestReceived += new EventHandler<HttpRequestReceivedArgs>(HttpServer_HttpRequestReceived);
            }

            _advertise = (options & HmeServerOptions.AdvertiseOnLocalNetwork) == HmeServerOptions.AdvertiseOnLocalNetwork;
        }

        public HmeServer(string name, Uri applicationPrefix, HmeServerOptions options)
            : this(name, applicationPrefix, options, new StandardHmeApplicationPump())
        {
        }

        public HmeServer(string name, Uri applicationPrefix)
            : this(name, applicationPrefix, HmeServerOptions.AdvertiseOnLocalNetwork)
        {
        }

        public void Start()
        {
            if (_service == null && _advertise)
            {
                _service = new Mono.Zeroconf.RegisterService();
                _service.Name = _name;
                _service.Port = _usePort;
                _service.RegType = "_tivo-hme._tcp";
                _service.TxtRecord = new Mono.Zeroconf.TxtRecord();
                _service.TxtRecord.Add("version", "0.40");
                _service.TxtRecord.Add("path", _applicationPrefix.AbsolutePath);
                _service.Register();

            }
            _started = true;
        }

        public void Stop()
        {
            _started = false;
            if (_service != null)
            {
                _service.Dispose();
                _service = null;
            }
        }

        public event EventHandler<HmeApplicationConnectedEventArgs> ApplicationConnected;
        public event EventHandler<NonApplicationRequestReceivedArgs> NonApplicationRequestReceivedArgs;

        protected virtual void OnNonApplicationRequestReceived(NonApplicationRequestReceivedArgs e)
        {
            ServerLog.Write(TraceEventType.Verbose, "Enter HmeServer.OnNonApplicationRequestReceived");
            EventHandler<NonApplicationRequestReceivedArgs> handler = NonApplicationRequestReceivedArgs;
            if (handler != null)
            {
                handler(this, e);
            }
            if (e.HttpResponse == null)
            {
                e.HttpResponse = new ApplicationIconHttpResponse();
            }
            ServerLog.Write(TraceEventType.Verbose, "Exit HmeServer.OnNonApplicationRequestReceived");
        }

        protected virtual void OnHmeApplicationRequestReceived(HttpRequestReceivedArgs e)
        {
            ServerLog.Write(TraceEventType.Verbose, "Enter HmeServer.OnHmeApplicationRequestReceived");
            e.HttpRequest.WriteResponse(new HmeApplicationHttpResponse());
            HmeConnection connection = new HmeConnection(e.HttpRequest.Stream, e.HttpRequest.Stream);
            HmeApplicationConnectedEventArgs args = new HmeApplicationConnectedEventArgs();
            args.Application = connection.Application;
            args.BaseUri = new Uri("http://" + e.HttpRequest.Headers[HttpRequestHeader.Host] + _applicationPrefix.AbsolutePath);

            OnApplicationConnected(args);
            _pump.AddHmeConnection(connection);
            ServerLog.Write(TraceEventType.Verbose, "Exit HmeServer.OnHmeApplicationRequestReceived");
        }

        protected virtual void OnApplicationConnected(HmeApplicationConnectedEventArgs args)
        {
            EventHandler<HmeApplicationConnectedEventArgs> handler = ApplicationConnected;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        private void HttpServer_HttpRequestReceived(object sender, HttpRequestReceivedArgs e)
        {
            if (_started &&
                StringComparer.InvariantCultureIgnoreCase.Compare(e.HttpRequest.RequestUri.OriginalString, _applicationPrefix.AbsolutePath) == 0)
            {
                OnHmeApplicationRequestReceived(e);
            }
            else if (_started && e.HttpRequest.RequestUri.OriginalString.StartsWith(_applicationPrefix.AbsolutePath, StringComparison.InvariantCultureIgnoreCase))
            {
                NonApplicationRequestReceivedArgs args = new NonApplicationRequestReceivedArgs(e.HttpRequest);
                OnNonApplicationRequestReceived(args);
                e.HttpRequest.WriteResponse(args.HttpResponse);
            }
        }

        private static Dictionary<short, HmePortServer> _portServers = new Dictionary<short, HmePortServer>();

        private class StandardHmeApplicationPump : IHmeApplicationPump
        {
            private static List<HmeConnection> _connections = new List<HmeConnection>();
            private static AutoResetEvent _connectionAdded = new AutoResetEvent(false);
            private static AutoResetEvent _connectionRemoved = new AutoResetEvent(false);

            static StandardHmeApplicationPump()
            {
                System.Threading.Thread thread = new System.Threading.Thread(DispatchApplicationCommandsAndEvents);
                thread.IsBackground = true;
                thread.Start();
            }

            private static void AddHmeConnection(HmeConnection connection)
            {
                lock (_connections)
                {
                    _connections.Add(connection);
                }
                _connectionAdded.Set();
                connection.BeginHandleEvent(ApplicationEventsHandled, connection);
            }

            private static void RemoveHmeConnection(HmeConnection connection)
            {
                lock (_connections)
                {
                    _connections.Remove(connection);
                }
                _connectionRemoved.Set();
            }

            private static void DispatchApplicationCommandsAndEvents()
            {
                List<WaitHandle> connectionWaitHandleList = new List<WaitHandle>();
                List<HmeConnection> runningConnections = new List<HmeConnection>();
                connectionWaitHandleList.Add(_connectionAdded);
                connectionWaitHandleList.Add(_connectionRemoved);
                WaitHandle[] connectionWaitHandles = connectionWaitHandleList.ToArray();
                while (true)
                {
                    int handleIndex = WaitHandle.WaitAny(connectionWaitHandles);
                    // must clear collections during add and remove since
                    // other threads may change the _applications collection
                    // before we get a lock on the _applications field
                    if (handleIndex == 0 || handleIndex == 1)
                    {
                        runningConnections.Clear();
                        connectionWaitHandleList.Clear();
                        connectionWaitHandleList.Add(_connectionAdded);
                        connectionWaitHandleList.Add(_connectionRemoved);
                        lock (_connections)
                        {
                            foreach (HmeConnection connection in _connections)
                            {
                                connectionWaitHandleList.Add(connection.CommandReceived);
                                connectionWaitHandleList.Add(connection.EventReceived);
                                runningConnections.Add(connection);
                            }
                        }
                        connectionWaitHandles = connectionWaitHandleList.ToArray();
                    }
                    else
                    {
                        // working on local copy of connections rather than member
                        // in order to avoid races to get a lock on the _connections field
                        int connectionIndex = (handleIndex - 2) / 2;
                        ThreadPool.QueueUserWorkItem(ProcessApplicationCommands, runningConnections[connectionIndex]);
                    }
                }
            }

            private static void ApplicationEventsHandled(IAsyncResult result)
            {
                HmeConnection connection = (HmeConnection)result.AsyncState;

                connection.EndHandleEvent(result);
                if (connection.Application.IsConnected)
                    connection.BeginHandleEvent(ApplicationEventsHandled, result.AsyncState);
                else
                    RemoveHmeConnection(connection);
            }

            private static void ProcessApplicationCommands(object hmeConnection)
            {
                HmeConnection connection = (HmeConnection)hmeConnection;
                if (!connection.RunOne())
                    RemoveHmeConnection(connection);
            }

            #region IHmeApplicationPump Members

            void IHmeApplicationPump.AddHmeConnection(HmeConnection connection)
            {
                AddHmeConnection(connection);
            }

            #endregion
        }

        private class HmePortServer
        {
            private HttpServer _httpServer;

            public HmePortServer(int listenPort)
            {
                _httpServer = new HttpServer(System.Net.IPAddress.Any, listenPort);
                _httpServer.Start();
            }

            public HttpServer HttpServer
            {
                get { return _httpServer; }
            }
        }
    }
}
