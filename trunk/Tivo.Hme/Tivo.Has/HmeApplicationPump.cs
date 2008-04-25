using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tivo.Hme.Host;
using System.Threading;

namespace Tivo.Has
{
    class HmeApplicationPump
    {
        private static Dictionary<IHmeConnection, IHmeApplicationDriver> _connections = new Dictionary<IHmeConnection, IHmeApplicationDriver>();
        private static AutoResetEvent _connectionAdded = new AutoResetEvent(false);
        private static AutoResetEvent _connectionRemoved = new AutoResetEvent(false);

        static HmeApplicationPump()
        {
            System.Threading.Thread thread = new System.Threading.Thread(DispatchApplicationCommandsAndEvents);
            thread.IsBackground = true;
            thread.Start();
        }

        private IHmeApplicationDriver _driver;
        public HmeApplicationPump(IHmeApplicationDriver driver)
        {
            _driver = driver;
        }

        public void AddHmeConnection(IHmeConnection connection)
        {
            lock (_connections)
            {
                _connections.Add(connection, _driver);
            }
            _connectionAdded.Set();
            _driver.HandleEventsAsync(connection);
        }

        private static void RemoveHmeConnection(IHmeConnection connection)
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
            List<IHmeConnection> runningConnections = new List<IHmeConnection>();
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
                        foreach (var entry in _connections)
                        {
                            connectionWaitHandleList.Add(entry.Key.CommandReceived);
                            connectionWaitHandleList.Add(entry.Key.EventReceived);
                            runningConnections.Add(entry.Key);
                        }
                    }
                    connectionWaitHandles = connectionWaitHandleList.ToArray();
                }
                else
                {
                    // working on local copy of connections rather than member
                    // in order to avoid races to get a lock on the _connections field
                    int connectionIndex = (handleIndex - 2) / 2;
                    IHmeConnection connection = runningConnections[connectionIndex];
                    IHmeApplicationDriver driver = _connections[connection];
                    driver.RunOneAsync(connection);
                }
            }
        }
    }
}
