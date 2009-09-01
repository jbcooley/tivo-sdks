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
using System.Net.Sockets;

namespace Tivo.Hmo
{
    public static class DiscoveryBeacon
    {
        private const int BeaconPort = 2190;
        private static Dictionary<string, string> NameLookup = new Dictionary<string, string>();
        // TODO: allow services to be appended to contents of BeaconPacket
        // this could be done by changing this field, or by concatenating this
        // with the dynamic portion
        private static readonly string BeaconPacket =
            string.Format("tivoconnect=1\n" +
            "method=broadcast\n" +
            "platform=pc/{0}\n" +
            "machine={1}\n" +
            "identity={2}\n" +
            "swversion=dnsw:{3}\n",
            Environment.OSVersion.VersionString, Environment.MachineName, GetIdentity(),
            System.Reflection.Assembly.GetCallingAssembly().GetName().Version);
        private static volatile string ServicesHeader = string.Empty;
        private static TivoConnectServicesCollection ServicesCollection = new TivoConnectServicesCollection();
        private static System.Threading.Timer SendBeaconTimer = new System.Threading.Timer(SendBeacon);
        private static System.Threading.Timer SlowBeaconTimer = new System.Threading.Timer(SlowBeacon);
        private static int IsFastBeacon;

        public static void Start()
        {
            StartBeacon();
            StartReceivingBroadcasts();
        }

        private static void StartBeacon()
        {
            if (System.Threading.Interlocked.Exchange(ref IsFastBeacon, 1) == 0)
            {
                SendBeaconTimer.Change(0, 5000);
                // change timer to send every minute after 30 seconds
                SlowBeaconTimer.Change(30000, System.Threading.Timeout.Infinite);
            }
        }

        private static void SlowBeacon(object state)
        {
            System.Threading.Interlocked.Exchange(ref IsFastBeacon, 0);
            // change timer to send every minute after 30 seconds
            SendBeaconTimer.Change(0, 60000);
        }

        private static void SendBeacon(object state)
        {
            UdpClient announce = new UdpClient();
            IPEndPoint broadcast = new IPEndPoint(IPAddress.Broadcast, BeaconPort);
            announce.EnableBroadcast = true;
            byte[] beaconBytes = Encoding.ASCII.GetBytes(BeaconPacket + ServicesHeader);
            announce.Send(beaconBytes, beaconBytes.Length, broadcast);
        }

        private static void StartReceivingBroadcasts()
        {
            UdpClient listen = new UdpClient();
            listen.EnableBroadcast = true;
            listen.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            IPEndPoint broadcast = new IPEndPoint(IPAddress.Any, BeaconPort);
            listen.Client.Bind(broadcast);
            listen.BeginReceive(BroadcastRecieved, listen);
        }

        public static string GetServer(string name, TimeSpan timeout)
        {
            return GetServer(name, (int)timeout.TotalMilliseconds);
        }

        public static string GetServer(string name, int millisecondsTimeout)
        {
            string server = null;
            System.Diagnostics.Stopwatch timeoutTimer = System.Diagnostics.Stopwatch.StartNew();
            do
            {
                lock (NameLookup)
                {
                    if (NameLookup.ContainsKey(name))
                        server = NameLookup[name];
                }
                if (server == null)
                {
                    System.Threading.Thread.Sleep(0); // yeild so another thread can possibly add a newly defined name
                }
            } while (server == null && timeoutTimer.ElapsedMilliseconds < millisecondsTimeout);
            timeoutTimer.Stop();
            if (server == null)
                throw new TimeoutException();
            return server;
        }

        public static IEnumerable<string> GetServerNames()
        {
            lock (NameLookup)
            {
                string[] names = new string[NameLookup.Count];
                NameLookup.Keys.CopyTo(names, 0);
                return names;
            }
        }

        public static TivoConnectServicesCollection Services
        {
            get { return ServicesCollection; }
        }

        public static event EventHandler<EventArgs> AvailableServersChanged;

        private static void OnAvailableServersChanged(EventArgs e)
        {
            EventHandler<EventArgs> handler = AvailableServersChanged;
            if (handler != null)
                handler(null, e);
        }

        private static void BroadcastRecieved(IAsyncResult results)
        {
            UdpClient listen = results.AsyncState as UdpClient;
            try
            {
                IPEndPoint broadcast = new IPEndPoint(IPAddress.Any, BeaconPort);
                byte[] bytes = listen.EndReceive(results, ref broadcast);
                string message = Encoding.ASCII.GetString(bytes);
                using (System.IO.StringReader reader = new System.IO.StringReader(message))
                {
                    bool nameLookupChanged = false;
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] nameValue = line.Split('=');
                        if (nameValue.Length == 2)
                        {
                            if (StringComparer.OrdinalIgnoreCase.Compare(nameValue[0], "machine") == 0)
                            {
                                lock (NameLookup)
                                {
                                    if (NameLookup.ContainsKey(nameValue[1]))
                                    {
                                        NameLookup[nameValue[1]] = broadcast.Address.ToString();
                                        nameLookupChanged = true;
                                    }
                                    else
                                    {
                                        NameLookup.Add(nameValue[1], broadcast.Address.ToString());
                                        nameLookupChanged = true;
                                        // found new tivo, change broadcast to every 5 seconds.
                                        StartBeacon();
                                    }
                                }
                            }
                        }
                    }
                    if (nameLookupChanged)
                        OnAvailableServersChanged(EventArgs.Empty);
                }
            }
            finally
            {
                if (listen != null)
                {
                    listen.BeginReceive(BroadcastRecieved, listen);
                }
            }
        }

        private static Guid GetIdentity()
        {
            if (Properties.Settings.Default.BeaconIdentity == Guid.Empty)
            {
                Properties.Settings.Default.BeaconIdentity = Guid.NewGuid();
                Properties.Settings.Default.Save();
            }
            return Properties.Settings.Default.BeaconIdentity;
        }

        internal static void RefreshServices()
        {
            // TODO: make this threadsafe
            // could update Services collection after reading
            if (Services.Count == 0)
                ServicesHeader = string.Empty;
            else
            {
                StringBuilder headerBuilder = new StringBuilder();
                headerBuilder.Append("services=");
                bool first = true;
                foreach (TivoConnectService service in Services)
                {
                    if (!first)
                        headerBuilder.Append(",");
                    headerBuilder.Append(service);
                }
                headerBuilder.Append("\n");
                ServicesHeader = headerBuilder.ToString();
            }
            // trigger fast beacon due to service changes
            StartBeacon();
        }
    }

    public struct TivoConnectService
    {
        public TivoConnectService(string name)
            : this()
        {
            Name = name;
        }

        public string Name { get; set; }
        public int? Port { get; set; }
        public string Protocol { get; set; }

        public override string ToString()
        {
            StringBuilder serviceHeaderEntry = new StringBuilder();
            serviceHeaderEntry.Append(Name);
            if (Port != null)
            {
                serviceHeaderEntry.Append(":");
                serviceHeaderEntry.Append((int)Port);
            }
            if (!string.IsNullOrEmpty(Protocol))
            {
                serviceHeaderEntry.Append("/");
                serviceHeaderEntry.Append(Protocol);
            }
            return serviceHeaderEntry.ToString();
        }
    }

    public class TivoConnectServicesCollection : IList<TivoConnectService>, System.Collections.IList
    {
        private List<TivoConnectService> _innerList = new List<TivoConnectService>();

        #region IList<TivoConnectService> Members

        public int IndexOf(TivoConnectService item)
        {
            lock (SyncRoot)
            {
                return _innerList.IndexOf(item);
            }
        }

        public void Insert(int index, TivoConnectService item)
        {
            lock (SyncRoot)
            {
                if (item.Name == null)
                    // name must not be null
                    throw new ArgumentException();
                _innerList.Insert(index, item);
            }
            DiscoveryBeacon.RefreshServices();
        }

        public void RemoveAt(int index)
        {
            lock (SyncRoot)
            {
                _innerList.RemoveAt(index);
                DiscoveryBeacon.RefreshServices();
            }
        }

        public TivoConnectService this[int index]
        {
            get
            {
                lock (SyncRoot)
                {
                    return _innerList[index];
                }
            }
            set
            {
                lock (SyncRoot)
                {
                    if (value.Name == null)
                        // name must not be null
                        throw new ArgumentException();
                    _innerList[index] = value;
                    DiscoveryBeacon.RefreshServices();
                }
            }
        }

        #endregion

        #region ICollection<TivoConnectService> Members

        public void Add(TivoConnectService item)
        {
            lock (SyncRoot)
            {
                if (item.Name == null)
                    // name must not be null
                    throw new ArgumentException();
                _innerList.Add(item);
                DiscoveryBeacon.RefreshServices();
            }
        }

        public void Clear()
        {
            lock (SyncRoot)
            {
                _innerList.Clear();
                DiscoveryBeacon.RefreshServices();
            }
        }

        public bool Contains(TivoConnectService item)
        {
            lock (SyncRoot)
            {
                return _innerList.Contains(item);
            }
        }

        public void CopyTo(TivoConnectService[] array, int arrayIndex)
        {
            lock (SyncRoot)
            {
                _innerList.CopyTo(array, arrayIndex);
            }
        }

        public int Count
        {
            get { lock(SyncRoot) return _innerList.Count; }
        }

        bool ICollection<TivoConnectService>.IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(TivoConnectService item)
        {
            lock (SyncRoot)
            {
                bool remove = _innerList.Remove(item);
                DiscoveryBeacon.RefreshServices();
                return remove;
            }
        }

        #endregion

        #region IEnumerable<TivoConnectService> Members

        public IEnumerator<TivoConnectService> GetEnumerator()
        {
            lock (SyncRoot)
            {
                return _innerList.GetEnumerator();
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            // lock happens in other function
            return GetEnumerator();
        }

        #endregion

        #region IList Members

        int System.Collections.IList.Add(object value)
        {
            int add;
            ThrowIfWrongType(value);
            lock (SyncRoot)
            {
                add = ((System.Collections.IList)_innerList).Add(value);
            }
            DiscoveryBeacon.RefreshServices();
            return add;
        }

        bool System.Collections.IList.Contains(object value)
        {
            lock (SyncRoot)
            {
                return ((System.Collections.IList)_innerList).Contains(value);
            }
        }

        int System.Collections.IList.IndexOf(object value)
        {
            lock (SyncRoot)
            {
                return ((System.Collections.IList)_innerList).IndexOf(value);
            }
        }

        void System.Collections.IList.Insert(int index, object value)
        {
            ThrowIfWrongType(value);
            lock (SyncRoot)
            {
                ((System.Collections.IList)_innerList).Insert(index, value);
            }
            DiscoveryBeacon.RefreshServices();
        }

        bool System.Collections.IList.IsFixedSize
        {
            get { return false; }
        }

        bool System.Collections.IList.IsReadOnly
        {
            get { return false; }
        }

        void System.Collections.IList.Remove(object value)
        {
            lock (SyncRoot)
            {
                ((System.Collections.IList)_innerList).Remove(value);
            }
            DiscoveryBeacon.RefreshServices();
        }

        object System.Collections.IList.this[int index]
        {
            get
            {
                lock (SyncRoot)
                {
                    return ((System.Collections.IList)_innerList)[index];
                }
            }
            set
            {
                ThrowIfWrongType(value);
                lock (SyncRoot)
                {
                    ((System.Collections.IList)_innerList)[index] = value;
                }
                DiscoveryBeacon.RefreshServices();
            }
        }

        #endregion

        #region ICollection Members

        void System.Collections.ICollection.CopyTo(Array array, int index)
        {
            lock (SyncRoot)
            {
                ((System.Collections.IList)_innerList).CopyTo(array, index);
            }
        }

        public bool IsSynchronized
        {
            get { return true; }
        }

        public object SyncRoot
        {
            get { return ((System.Collections.ICollection)_innerList).SyncRoot; }
        }

        #endregion

        private static void ThrowIfWrongType(object value)
        {
            if (!(value is TivoConnectService))
            {
                throw new ArgumentException();
            }
            if (((TivoConnectService)value).Name == null)
            {
                // name must not be null
                throw new ArgumentException();
            }
        }
    }
}
