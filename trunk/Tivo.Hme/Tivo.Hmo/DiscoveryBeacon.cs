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
        private static readonly string BeaconPacket =
            string.Format("tivoconnect=1\n" +
            "method=broadcast\n" +
            "platform=pc/{0}\n" +
            "machine={1}\n" +
            "identity={2}\n" +
            "swversion=dnsw:{3}\n",
            Environment.OSVersion.VersionString, Environment.MachineName, Guid.NewGuid(),
            System.Reflection.Assembly.GetCallingAssembly().GetName().Version);
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
            byte[] beaconBytes = Encoding.ASCII.GetBytes(BeaconPacket);
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
                                    }
                                    else
                                    {
                                        NameLookup.Add(nameValue[1], broadcast.Address.ToString());
                                        // found new tivo, change broadcast to every 5 seconds.
                                        StartBeacon();
                                    }
                                }
                            }
                        }
                    }
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
    }
}
