using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Tivo.Has
{
    class HmeStream : IHmeStream
    {
        private Stream _innerStream;
        private byte[] _readBuffer = new byte[20];

        public HmeStream(Stream innerStream)
        {
            _innerStream = innerStream;
        }

        #region IHmeStream Members

        public bool CanRead
        {
            get { return _innerStream.CanRead; }
        }

        public bool CanWrite
        {
            get { return _innerStream.CanWrite; }
        }

        public bool CanTimeout
        {
            get { return _innerStream.CanTimeout; }
        }

        public int ReadTimeout
        {
            get { return _innerStream.ReadTimeout; }
            set { _innerStream.ReadTimeout = value; }
        }

        public int WriteTimeout
        {
            get { return _innerStream.WriteTimeout; }
            set { _innerStream.WriteTimeout = value; }
        }

        public byte[] ReadBuffer
        {
            get { return _readBuffer; }
        }

        public int Read(int count)
        {
            EnsureBuffer(count);
            return _innerStream.Read(_readBuffer, 0, count);
        }

        public int ReadByte()
        {
            int byteRead = _innerStream.ReadByte();
            if (byteRead == -1)
                return -1;
            _readBuffer[0] = (byte)byteRead;
            return byteRead;
        }

        public IHmeAsyncResult BeginRead(int count, IHmeAsyncCallback callback)
        {
            HmeAsyncResult result = new HmeAsyncResult();
            result.Callback = callback;
            EnsureBuffer(count);
            result.AsyncResult = _innerStream.BeginRead(_readBuffer, 0, count, AsyncCallbackWrapper, result);
            return result;
        }

        public int EndRead(IHmeAsyncResult asyncResult)
        {
            HmeAsyncResult result = (HmeAsyncResult)asyncResult;
            return _innerStream.EndRead(result.AsyncResult);
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            _innerStream.Write(buffer, offset, count);
        }

        public void WriteByte(byte value)
        {
            _innerStream.WriteByte(value);
        }

        public IHmeAsyncResult BeginWrite(byte[] buffer, int offset, int count, IHmeAsyncCallback callback)
        {
            HmeAsyncResult result = new HmeAsyncResult();
            result.Callback = callback;
            result.AsyncResult = _innerStream.BeginWrite(buffer, offset, count, AsyncCallbackWrapper, result);
            return result;
        }

        public void EndWrite(IHmeAsyncResult asyncResult)
        {
            HmeAsyncResult result = (HmeAsyncResult)asyncResult;
            _innerStream.EndWrite(result.AsyncResult);
        }

        public void Close()
        {
            _innerStream.Close();
        }

        public void Flush()
        {
            _innerStream.Flush();
        }

        #endregion

        private void EnsureBuffer(int count)
        {
            // TODO: consider limiting the maximum size
            // the reader will repeat the read if
            // not all requested data is read
            if (_readBuffer.Length < count)
                _readBuffer = new byte[count];
        }

        private void AsyncCallbackWrapper(IAsyncResult asyncResult)
        {
            HmeAsyncResult hmeAsyncResult = (HmeAsyncResult)asyncResult.AsyncState;
            // AsyncResult could be null if operation completes syncronously
            if (hmeAsyncResult.AsyncResult == null)
                hmeAsyncResult.AsyncResult = asyncResult;
            hmeAsyncResult.Callback.AsyncCallback(hmeAsyncResult);
        }

        private class HmeAsyncResult : IHmeAsyncResult
        {
            public IAsyncResult AsyncResult { get; set; }
            public IHmeAsyncCallback Callback { get; set; }

            #region IHmeAsyncResult Members

            public bool CompletedSynchronously
            {
                get { return AsyncResult.CompletedSynchronously; }
            }

            public bool IsCompleted
            {
                get { return AsyncResult.IsCompleted; }
            }

            #endregion
        }
    }
}
