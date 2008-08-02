using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Tivo.Has.AddIn
{
    class HmeStream : Stream
    {
        private IHmeStream _innerStream;

        public HmeStream(IHmeStream innerStream)
        {
            _innerStream = innerStream;
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            HmeAsyncResult result = new HmeAsyncResult();
            result.AsyncState = state;
            result.ReadBuffer = buffer;
            result.Offset = offset;
            result.Count = count;
            result.AsyncResult = _innerStream.BeginRead(count, new HmeAsyncCallback(callback, result));
            return result;
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            HmeAsyncResult result = new HmeAsyncResult();
            result.AsyncState = state;
            result.AsyncResult = _innerStream.BeginWrite(buffer, offset, count, new HmeAsyncCallback(callback, result));
            return result;
        }

        public override bool CanRead
        {
            get { return _innerStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanTimeout
        {
            get { return _innerStream.CanTimeout; }
        }

        public override bool CanWrite
        {
            get { return _innerStream.CanWrite; }
        }

        // definately needed -- see Close
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _innerStream.Close();
            }
            base.Dispose(disposing);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            HmeAsyncResult result = (HmeAsyncResult)asyncResult;
            int readCount = _innerStream.EndRead(result.AsyncResult);
            Array.Copy(_innerStream.ReadBuffer, 0, result.ReadBuffer, result.Offset, readCount);
            return readCount;
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            HmeAsyncResult result = (HmeAsyncResult)asyncResult;
            _innerStream.EndWrite(result.AsyncResult);
        }

        public override void Flush()
        {
            _innerStream.Flush();
        }

        // not available on network stream
        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        // not available on network stream
        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int readCount = _innerStream.Read(count);
            Array.Copy(_innerStream.ReadBuffer, 0, buffer, offset, readCount);
            return readCount;
        }

        public override int ReadByte()
        {
            return _innerStream.ReadByte();
        }

        public override int ReadTimeout
        {
            get { return _innerStream.ReadTimeout; }
            set { _innerStream.ReadTimeout = value; }
        }

        // not available on network stream
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        // not available on network stream
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _innerStream.Write(buffer, offset, count);
        }

        public override void WriteByte(byte value)
        {
            _innerStream.WriteByte(value);
        }

        public override int WriteTimeout
        {
            get { return _innerStream.WriteTimeout; }
            set { _innerStream.WriteTimeout = value; }
        }

        private class HmeAsyncResult : IAsyncResult
        {
            public IHmeAsyncResult AsyncResult { get; set; }
            public byte[] ReadBuffer { get; set; }
            public int Offset { get; set; }
            public int Count { get; set; }

            #region IAsyncResult Members

            public object AsyncState { get; set; }

            public System.Threading.WaitHandle AsyncWaitHandle
            {
                get { throw new NotImplementedException(); }
            }

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

        private class HmeAsyncCallback : IHmeAsyncCallback
        {
            private AsyncCallback _callback;
            private HmeAsyncResult _result;

            public HmeAsyncCallback(AsyncCallback callback, HmeAsyncResult result)
            {
                _callback = callback;
                _result = result;
            }

            #region IHmeAsyncCallback Members

            public void AsyncCallback(IHmeAsyncResult asyncResult)
            {
                // could happen syncronously
                if (_result.AsyncResult == null)
                    _result.AsyncResult = asyncResult;
                _callback(_result);
            }

            #endregion
        }
    }
}
