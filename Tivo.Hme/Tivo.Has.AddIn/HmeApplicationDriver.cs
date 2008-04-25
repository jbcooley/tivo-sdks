using System;
using System.IO;
using System.Threading;
using Tivo.Hme.Host;

namespace Tivo.Has.AddIn
{
    sealed class HmeApplicationDriver : IHmeApplicationDriver
    {
        #region IHmeApplicationDriver Members

        public event EventHandler<ApplicationEndedEventArgs> ApplicationEnded;

        public IHmeConnection CreateHmeConnection(IHmeApplicationIdentity identity, Stream inputStream, Stream outputStream)
        {
            return new HmeConnectionWrapper(inputStream, outputStream);
        }

        public void HandleEventsAsync(IHmeConnection connection)
        {
            HmeConnectionWrapper wrapper = connection as HmeConnectionWrapper;
            if (wrapper != null)
            {
                wrapper.HmeConnection.BeginHandleEvent(ApplicationEventsHandled, wrapper.HmeConnection);
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

        private void ApplicationEventsHandled(IAsyncResult result)
        {
            HmeConnection connection = (HmeConnection)result.AsyncState;
            // TODO: move this exception logic into Begin and End HandleEvent.
            try
            {
                connection.EndHandleEvent(result);
                if (connection.Application.IsConnected)
                    connection.BeginHandleEvent(ApplicationEventsHandled, result.AsyncState);
                else
                    OnApplicationEnded(MyApplicationEndedEventArgs.Empty);
            }
            catch (IOException ex)
            {
                // just a disconnect so not a critical event
                //ServerLog.Write(ex);
                connection.Application.CloseDisconnected();
                OnApplicationEnded(MyApplicationEndedEventArgs.Empty);
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
    }
}
