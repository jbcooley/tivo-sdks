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
using System.Diagnostics;
using System.IO;
using System.Threading;
using Tivo.Hme.Host.Http;
using Tivo.Hme.Host.Services;

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

    public class HttpConnectionEventArgs : EventArgs
    {
        public HttpConnectionEventArgs(HttpListenerContext context)
        {
            Context = context;
        }

        public HttpListenerContext Context { get; private set; }
    }

    public class HmeApplicationIconRequestedArgs : EventArgs
    {
        public byte[] Icon { get; set; }
        public string ContentType { get; set; }
        public Uri BaseUri { get; set; }
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
    public class HmeServer : System.ComponentModel.Design.IServiceContainer
    {
        private string _name;
        private short _usePort;
        private Mono.Zeroconf.RegisterService _service;
        private bool _started;
        private bool _advertise;
        private IHmeApplicationPump _pump;
        private string iconUri;
        private System.ComponentModel.Design.IServiceContainer _serviceContainer;

        public HmeServer(string name, Uri applicationPrefix, HmeServerOptions options, IHmeApplicationPump pump, IServiceProvider parentProvider)
        {
            _name = name;
            ApplicationPrefix = applicationPrefix;
            _pump = pump;
            iconUri = applicationPrefix.AbsolutePath + "icon.png";
            _advertise = (options & HmeServerOptions.AdvertiseOnLocalNetwork) == HmeServerOptions.AdvertiseOnLocalNetwork;
            _serviceContainer = new System.ComponentModel.Design.ServiceContainer(parentProvider);
            _serviceContainer.AddService(typeof(IHttpApplicationHostPool), new HttpApplicationHostPool());

            _usePort = 7688;
            if (applicationPrefix.IsAbsoluteUri)
            {
                _usePort = (short)applicationPrefix.Port;
            }

            // everything important to initialization must happen before here since the events can
            // be raised immediately.
            lock (_portServers)
            {
                HmePortServer portServer;
                if (!_portServers.TryGetValue(_usePort, out portServer))
                {
                    portServer = new HmePortServer(_usePort);
                    _portServers.Add(_usePort, portServer);
                }
                portServer.ConnectionReceived += HttpServer_HttpConnectionReceived;
            }
        }

        public HmeServer(string name, Uri applicationPrefix, HmeServerOptions options, IHmeApplicationPump pump)
            : this(name, applicationPrefix, options, pump, null)
        {
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
                _service.TxtRecord.Add("path", ApplicationPrefix.AbsolutePath);
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

        #region IServiceProvider Members

        public virtual object GetService(Type serviceType)
        {
            return _serviceContainer.GetService(serviceType);
        }

        #endregion

        #region IServiceContainer Members

        void System.ComponentModel.Design.IServiceContainer.AddService(Type serviceType, System.ComponentModel.Design.ServiceCreatorCallback callback, bool promote)
        {
            _serviceContainer.AddService(serviceType, callback, promote);
        }

        void System.ComponentModel.Design.IServiceContainer.AddService(Type serviceType, System.ComponentModel.Design.ServiceCreatorCallback callback)
        {
            _serviceContainer.AddService(serviceType, callback);
        }

        void System.ComponentModel.Design.IServiceContainer.AddService(Type serviceType, object serviceInstance, bool promote)
        {
            _serviceContainer.AddService(serviceType, serviceInstance, promote);
        }

        void System.ComponentModel.Design.IServiceContainer.AddService(Type serviceType, object serviceInstance)
        {
            _serviceContainer.AddService(serviceType, serviceInstance);
        }

        void System.ComponentModel.Design.IServiceContainer.RemoveService(Type serviceType, bool promote)
        {
            _serviceContainer.RemoveService(serviceType, promote);
        }

        void System.ComponentModel.Design.IServiceContainer.RemoveService(Type serviceType)
        {
            _serviceContainer.RemoveService(serviceType);
        }

        #endregion

        public Uri ApplicationPrefix { get; private set; }

        public event EventHandler<HmeApplicationConnectedEventArgs> ApplicationConnected;
        public event EventHandler<HttpConnectionEventArgs> NonApplicationRequestReceived;
        public event EventHandler<HmeApplicationIconRequestedArgs> ApplicationIconRequested;

        protected virtual void OnNonApplicationRequestReceived(HttpConnectionEventArgs e)
        {
            ServerLog.Write(TraceEventType.Verbose, "Enter HmeServer.OnNonApplicationRequestReceived");
            EventHandler<HttpConnectionEventArgs> handler = NonApplicationRequestReceived;
            if (handler != null)
            {
                handler(this, e);
            }
            ServerLog.Write(TraceEventType.Verbose, "Exit HmeServer.OnNonApplicationRequestReceived");
        }

        protected virtual void OnHmeApplicationIconRequested(HmeApplicationIconRequestedArgs e)
        {
            EventHandler<HmeApplicationIconRequestedArgs> handler = ApplicationIconRequested;
            if (handler != null)
                handler(this, e);
            if (e.Icon == null)
            {
                // provide default icon
                e.Icon = Properties.Resources.iconpng;
                e.ContentType = "image/png";
            }
        }

        protected virtual void OnHmeApplicationRequestReceived(HttpListenerContext context)
        {
            ServerLog.Write(TraceEventType.Verbose, "Enter HmeServer.OnHmeApplicationRequestReceived");
            HmeApplicationHttpResponse.BeginResponse(context);
            HmeConnection connection = new HmeConnection(context.Request.InputStream, context.Response.OutputStream);
            HmeApplicationConnectedEventArgs args = new HmeApplicationConnectedEventArgs();
            args.Application = connection.Application;
            args.BaseUri = BuildBaseUri(context);

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

        private void HttpServer_HttpConnectionReceived(object sender, HttpConnectionEventArgs e)
        {
            if (_started &&
                ((e.Context.Request.Url.IsAbsoluteUri && StringComparer.InvariantCultureIgnoreCase.Compare(e.Context.Request.Url.AbsolutePath, ApplicationPrefix.AbsolutePath) == 0) ||
                StringComparer.InvariantCultureIgnoreCase.Compare(e.Context.Request.Url.OriginalString, ApplicationPrefix.AbsolutePath) == 0))
            {
                OnHmeApplicationRequestReceived(e.Context);
            }
            else if (_started &&
                ((e.Context.Request.Url.IsAbsoluteUri && e.Context.Request.Url.AbsolutePath.StartsWith(ApplicationPrefix.AbsolutePath, StringComparison.InvariantCultureIgnoreCase)) ||
                e.Context.Request.Url.OriginalString.StartsWith(ApplicationPrefix.AbsolutePath, StringComparison.InvariantCultureIgnoreCase)))
            {
                if (StringComparer.InvariantCultureIgnoreCase.Compare(e.Context.Request.Url.LocalPath, iconUri) == 0)
                {
                    HmeApplicationIconRequestedArgs iconArgs = new HmeApplicationIconRequestedArgs { BaseUri = BuildBaseUri(e.Context) };
                    OnHmeApplicationIconRequested(iconArgs);
                    e.Context.Response.ContentType = iconArgs.ContentType;
                    e.Context.Response.ContentLength64 = iconArgs.Icon.LongLength;
                    e.Context.Response.OutputStream.Write(iconArgs.Icon, 0, iconArgs.Icon.Length);
                    //e.Context.Response.Close();
                }
                else
                {
                    OnNonApplicationRequestReceived(e);
                }
            }
        }

        protected Uri BuildBaseUri(HttpListenerContext context)
        {
            string[] hostParts = (context.Request.UserHostName ?? context.Request.Url.Authority).Split(':');
            UriBuilder builder = new UriBuilder("http", hostParts[0]);
            if (hostParts.Length == 2)
                builder.Port = int.Parse(hostParts[1]);
            builder.Path = ApplicationPrefix.AbsolutePath;
            return builder.Uri;
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
            private static HttpListener _httpListener = new HttpListener();

            static HmePortServer()
            {
                _httpListener.Start();
                _httpListener.BeginGetContext(OnConnectionReceived, null);
            }

            public HmePortServer(int listenPort)
            {
                _httpListener.Prefixes.Add("http://+:" + listenPort + "/");
            }

            public EventHandler<HttpConnectionEventArgs> ConnectionReceived;

            private static void OnConnectionReceived(IAsyncResult asyncResult)
            {
                // start next one before processing
                _httpListener.BeginGetContext(OnConnectionReceived, null);
                // process connection received
                HttpListenerContext context = _httpListener.EndGetContext(asyncResult);
                HmePortServer portServer;
                if (_portServers.TryGetValue((short)context.Request.Url.Port, out portServer))
                {
                    portServer.RaiseConnectionReceived(context);
                }
            }

            private void RaiseConnectionReceived(HttpListenerContext context)
            {
                EventHandler<HttpConnectionEventArgs> handler = ConnectionReceived;
                if (handler != null)
                    handler(this, new HttpConnectionEventArgs(context));
            }
        }
    }
}
