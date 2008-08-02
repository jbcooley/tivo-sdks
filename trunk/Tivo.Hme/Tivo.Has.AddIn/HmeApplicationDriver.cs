using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Tivo.Hme.Host;

namespace Tivo.Has.AddIn
{
    [System.AddIn.AddIn("Application Driver")]
    public sealed class HmeApplicationDriver : IHmeApplicationDriver
    {
        private List<IHmeApplicationIdentity> _applicationIdentities = new List<IHmeApplicationIdentity>();
        // jump through hoops to make this streams work across appdomains
        // without sharing types
        private static Dictionary<Guid, HmeApplicationDriver> _instances = new Dictionary<Guid, HmeApplicationDriver>();
        private Guid _thisGuid = Guid.NewGuid();
        private Dictionary<Guid, HmeConnection> _connections = new Dictionary<Guid, HmeConnection>();

        public HmeApplicationDriver()
        {
            for (int i = 0; i < Properties.Settings.Default.ApplicationName.Count; ++i)
            {
                _applicationIdentities.Add(new HmeApplicationIdentity(i));
            }
            _instances.Add(_thisGuid, this);
        }

        #region IHmeApplicationDriver Members

        public IHasApplicationConfigurator GetApplicationConfiguration()
        {
            return new HasApplicationConfigurator();
        }

        public IList<IHmeApplicationIdentity> ApplicationIdentities
        {
            get { return _applicationIdentities; }
        }

        public event EventHandler<ApplicationEndedEventArgs> ApplicationEnded;

        public IHmeConnection CreateHmeConnection(IHmeApplicationIdentity identity, IHmeStream inputStream, IHmeStream outputStream)
        {
            if (identity == null)
                throw new ArgumentNullException("identity");

            HmeConnectionWrapper wrapper = new HmeConnectionWrapper(inputStream, outputStream);
            ((HmeApplicationIdentity)identity).CreateApplication(wrapper.HmeConnection);

            return wrapper;
        }

        public void HandleEventsAsync(IHmeConnection connection)
        {
            HmeConnectionWrapper wrapper = connection as HmeConnectionWrapper;
            if (wrapper != null)
            {
                ApplicationEventsData data = new ApplicationEventsData { DriverGuid = _thisGuid, ConnectionGuid = Guid.NewGuid() };
                _connections.Add(data.ConnectionGuid, wrapper.HmeConnection);
                wrapper.HmeConnection.BeginHandleEvent(ApplicationEventsHandled, data);
            }
        }

        public void RunOneAsync(IHmeConnection connection)
        {
            HmeConnectionWrapper wrapper = connection as HmeConnectionWrapper;
            if (wrapper != null)
            {
                ThreadPool.QueueUserWorkItem(ProcessApplicationCommands, wrapper.HmeConnection);
            }
        }

        #endregion

        private void OnApplicationEnded(ApplicationEndedEventArgs e)
        {
            EventHandler<ApplicationEndedEventArgs> handler = ApplicationEnded;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private static void ApplicationEventsHandled(IAsyncResult result)
        {
            // this is done so that only guids cross appdomain boundary
            ApplicationEventsData eventsData = (ApplicationEventsData)result.AsyncState;
            HmeApplicationDriver driver = _instances[eventsData.DriverGuid];
            HmeConnection connection = driver._connections[eventsData.ConnectionGuid];

            connection.EndHandleEvent(result);
            if (connection.Application.IsConnected)
            {
                connection.BeginHandleEvent(ApplicationEventsHandled, result.AsyncState);
            }
            else
            {
                driver._connections.Remove(eventsData.ConnectionGuid);
                driver.OnApplicationEnded(MyApplicationEndedEventArgs.Empty);
            }
        }

        private void ProcessApplicationCommands(object hmeConnection)
        {
            HmeConnection connection = (HmeConnection)hmeConnection;
            if (!connection.RunOne())
                OnApplicationEnded(MyApplicationEndedEventArgs.Empty);
        }

        private sealed class MyApplicationEndedEventArgs : ApplicationEndedEventArgs
        {
            private static MyApplicationEndedEventArgs _empty = new MyApplicationEndedEventArgs();

            public static new ApplicationEndedEventArgs Empty
            {
                get { return _empty; }
            }
        }

        [Serializable]
        private class ApplicationEventsData
        {
            public Guid DriverGuid { get; set; }
            public Guid ConnectionGuid { get; set; }
        }
    }
}
