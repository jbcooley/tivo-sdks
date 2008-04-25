using System;
using System.IO;
using System.Threading;
using Tivo.Hme.Host;

namespace Tivo.Has.AddIn
{
    class HmeConnectionWrapper : IHmeConnection
    {
        public HmeConnectionWrapper(Stream inputStream, Stream outputStream)
        {
            HmeConnection = new HmeConnection(inputStream, outputStream);
        }

        public HmeConnection HmeConnection { get; private set; }

        #region IHmeConnection Members

        public WaitHandle CommandReceived
        {
            get { return HmeConnection.CommandReceived; }
        }

        public WaitHandle EventReceived
        {
            get { return HmeConnection.EventReceived; }
        }

        #endregion
    }
}
