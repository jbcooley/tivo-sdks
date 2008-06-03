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
using System.IO;
using System.Threading;

namespace Tivo.Hme.Host
{
    public enum ConnectionSyncronizationType
    {
        Local,
        System
    }

    public sealed class HmeConnection : IDisposable, IHmeConnectionSyncronizationInfo
    {
        private Application _application;
        private HmeWriter _writer;
        private HmeReader _reader;
        private string _eventReceivedName;
        private string _commandReceivedName;
        private EventWaitHandle _eventReceived;
        private EventWaitHandle _commandReceived;
        private Queue<Events.EventInfo> _events = new Queue<Events.EventInfo>();
        private Queue<Commands.IHmeCommand> _commands = new Queue<Commands.IHmeCommand>();
        // used for locks
        private object _processCommands = new object();
        private object _processEvents = new object();

        public HmeConnection(Stream inputStream, Stream outputStream)
            : this(inputStream, outputStream, ConnectionSyncronizationType.Local)
        {
        }

        public HmeConnection(Stream inputStream, Stream outputStream, ConnectionSyncronizationType syncronizationType)
        {
            if (syncronizationType == ConnectionSyncronizationType.Local)
            {
                _eventReceivedName = null;
                _commandReceivedName = null;
            }
            else
            {
                _eventReceivedName = Guid.NewGuid().ToString();
                _commandReceivedName = Guid.NewGuid().ToString();
            }
            _eventReceived = new EventWaitHandle(false, EventResetMode.AutoReset, _eventReceivedName);
            _commandReceived = new EventWaitHandle(false, EventResetMode.AutoReset, _commandReceivedName);

            // write handshake to output stream
            byte[] handshake = new byte[] {
                // magic number
                0x53, 0x42, 0x54, 0x56,
                // reserved
                0x00, 0x00,
                // major version
                0,
                // minor version
                44
            };
            outputStream.Write(handshake, 0, handshake.Length);
            outputStream.Flush();
            // check for handshake on input stream
            byte[] response = new byte[handshake.Length];
            int read = HmeReader.ReadAll(inputStream, response, 0, response.Length);
            // check magic number
            if (handshake[0] != response[0] ||
                handshake[1] != response[1] ||
                handshake[2] != response[2] ||
                handshake[3] != response[3])
            {
                throw new IOException("Handshake failed");
            }
            _writer = new HmeWriter(outputStream);
            _reader = new HmeReader(inputStream);
            _application = new Application(this);
        }

        #region IDisposable Members

        public void Dispose()
        {
            ProcessCommandsAndEvents();
            _reader.Dispose();
            _writer.Dispose();
        }

        #endregion

        public Application Application
        {
            get { return _application; }
        }

        public WaitHandle EventReceived
        {
            get { return _eventReceived; }
        }

        public WaitHandle CommandReceived
        {
            get { return _commandReceived; }
        }

        #region IHmeConnectionSyncronizationInfo Members

        string IHmeConnectionSyncronizationInfo.EventReceivedName
        {
            get { return _eventReceivedName; }
        }

        string IHmeConnectionSyncronizationInfo.CommandReceivedName
        {
            get { return _commandReceivedName; }
        }

        #endregion

        public void Run()
        {
            try
            {
                while (Application.IsConnected)
                {
                    WaitHandle.WaitAny(new WaitHandle[] { _eventReceived, _commandReceived }, 1000, false);
                    ProcessCommandsAndEvents();
                }
            }
            catch (IOException)
            {
                Application.IsConnected = false;
            }
        }

        public bool RunOne()
        {
            try
            {
                if (Application.IsConnected)
                {
                    ProcessCommandsAndEvents();
                }
            }
            catch (IOException)
            {
                Application.IsConnected = false;
            }
            return Application.IsConnected;
        }

        public void HandleEvents()
        {
            try
            {
                while (true)
                {
                    long eventType = _reader.ReadInt64();
                    ProcessEvent(eventType);
                }
            }
            catch (IOException)
            {
                Application.IsConnected = false;
            }
        }

        #region BeginHandleEventErrorResult

        private class BeginHandleEventErrorResult : IAsyncResult
        {
            private WaitHandle _handle;
            private object _asyncState;
            private Exception _exception;

            public BeginHandleEventErrorResult(object asyncState, Exception exception)
            {
                _asyncState = asyncState;
                _exception = exception;
            }

            public Exception Exception
            {
                get { return _exception; }
            }

            #region IAsyncResult Members

            public object AsyncState
            {
                get { return _asyncState; }
            }

            public WaitHandle AsyncWaitHandle
            {
                get
                {
                    if (_handle == null)
                        Interlocked.CompareExchange(ref _handle, new Mutex(), null);
                    return _handle;
                }
            }

            public bool CompletedSynchronously
            {
                get { return true; }
            }

            public bool IsCompleted
            {
                get { return true; }
            }

            #endregion
        }

        #endregion

        public IAsyncResult BeginHandleEvent(AsyncCallback callback, object state)
        {
            try
            {
                return _reader.BeginRead(callback, state);
            }
            catch (IOException ex)
            {
                BeginHandleEventErrorResult result = new BeginHandleEventErrorResult(state, ex);
                callback(result);
                return result;
            }
        }

        public void EndHandleEvent(IAsyncResult asyncResult)
        {
            try
            {
                if (asyncResult is BeginHandleEventErrorResult)
                {
                    throw ((BeginHandleEventErrorResult)asyncResult).Exception;
                }
                Application.IsConnected &= _reader.EndRead(asyncResult);
                if (Application.IsConnected)
                {
                    long eventType = _reader.ReadInt64();
                    ProcessEvent(eventType);
                }
            }
            catch (IOException ex)
            {
                StatusLog.Write(ex);
                Application.IsConnected = false;
                Application.CloseDisconnected();
            }
        }

        internal HmeWriter Writer
        {
            get { return _writer; }
        }

        internal void SendCommand(Commands.IHmeCommand command)
        {
            command.SendCommand(this);
            _writer.WriteTerminator();
            ProtocolLog.Write(command);
        }

        internal void PostCommand(Commands.IHmeCommand command)
        {
            lock (_commands)
            {
                _commands.Enqueue(command);
                _commandReceived.Set();
            }
        }

        internal void PostCommandBatch(IEnumerable<Commands.IHmeCommand> batch)
        {
            lock (_commands)
            {
                foreach (Commands.IHmeCommand command in batch)
                {
                    _commands.Enqueue(command);
                }
                _commandReceived.Set();
            }
        }

        private void ProcessCommandsAndEvents()
        {
            // only enter this block if it's not already being processed
            if (Monitor.TryEnter(_processCommands))
            {
                try
                {
                    // need second lock because that's what really protects the collection
                    lock (_commands)
                    {
                        Application.CommandThreadId = Thread.CurrentThread.ManagedThreadId;
                        while (_commands.Count != 0)
                        {
                            SendCommand(_commands.Dequeue());
                        }
                        Application.CommandThreadId = 0;
                    }
                }
                finally
                {
                    Monitor.Exit(_processCommands);
                }
            }
            // only enter this block if it's not already being processed
            if (Monitor.TryEnter(_processEvents))
            {
                try
                {
                    // need second lock because that's what really protects the collection
                    lock (_events)
                    {
                        while (_events.Count != 0)
                        {
                            _events.Dequeue().RaiseEvent(Application);
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(_processEvents);
                }
            }
        }

        private void ProcessEvent(long eventType)
        {
            Events.EventInfo eventInfo;
            switch (eventType)
            {
                case Events.ApplicationInfo.Type:
                    eventInfo = new Events.ApplicationInfo();
                    break;
                case Events.DeviceInfo.Type:
                    eventInfo = new Events.DeviceInfo();
                    break;
                case Events.FontInfo.Type:
                    eventInfo = new Events.FontInfo();
                    break;
                case Events.IdleInfo.Type:
                    eventInfo = new Events.IdleInfo();
                    break;
                case Events.KeyInfo.Type:
                    eventInfo = new Events.KeyInfo();
                    break;
                case Events.ResourceInfo.Type:
                    eventInfo = new Events.ResourceInfo();
                    break;
                case Events.DisplayInfo.Type:
                    eventInfo = new Events.DisplayInfo();
                    break;
                default:
                    eventInfo = new Events.UnknownEventInfo(eventType);
                    break;
            }
            eventInfo.Read(_reader);
            PostEvent(eventInfo);
        }

        private void PostEvent(Events.EventInfo eventInfo)
        {
            lock (_events)
            {
                _events.Enqueue(eventInfo);
                _eventReceived.Set();
            }
            ProtocolLog.Write(eventInfo);
        }
    }
}
