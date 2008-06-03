using System;
using System.IO;
using System.Threading;
using Tivo.Hme.Host;

namespace Tivo.Has.AddIn
{
    class HmeConnectionWrapper : IHmeConnection
    {
        public HmeConnectionWrapper(IHmeStream inputStream, IHmeStream outputStream)
        {
            HmeStream hmeInputStream = new HmeStream(inputStream);
            HmeStream hmeOutputStream = new HmeStream(outputStream);
            HmeConnection = new HmeConnection(hmeInputStream, hmeOutputStream, ConnectionSyncronizationType.System);
        }

        public HmeConnection HmeConnection { get; private set; }

        #region IHmeConnection Members

        public string CommandReceivedName
        {
            get { return ((IHmeConnectionSyncronizationInfo)HmeConnection).CommandReceivedName; }
        }

        public string EventReceivedName
        {
            get { return ((IHmeConnectionSyncronizationInfo)HmeConnection).EventReceivedName; }
        }

        #endregion
    }
}
