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
    public sealed class HmeConnection : IDisposable
    {
        private Application _application;
        private HmeWriter _writer;
        private HmeReader _reader;
        private AutoResetEvent _eventReceived = new AutoResetEvent(false);
        private AutoResetEvent _commandReceived = new AutoResetEvent(false);
        private List<Events.EventInfo> _events = new List<Events.EventInfo>();
        private List<Commands.IHmeCommand> _commands = new List<Commands.IHmeCommand>();
        // used for locks
        private object _processCommands = new object();
        private object _processEvents = new object();

        public HmeConnection(Stream inputStream, Stream outputStream)
        {
            // write handshake to output stream
            byte[] handshake = new byte[] {
                // magic number
                0x53, 0x42, 0x54, 0x56,
                // reserved
                0x00, 0x00,
                // major version
                0,
                // minor version
                40
            };
            outputStream.Write(handshake, 0, handshake.Length);
            outputStream.Flush();
            // check for handshake on input stream
            byte[] response = new byte[handshake.Length];
            int read = inputStream.Read(response, 0, response.Length);
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

        public void Run()
        {
            try
            {
                while (Application.IsRunning)
                {
                    WaitHandle.WaitAny(new WaitHandle[] { _eventReceived, _commandReceived }, 1000, false);
                    ProcessCommandsAndEvents();
                }
            }
            catch (IOException)
            {
                Application.IsRunning = false;
            }
        }

        public bool RunOne()
        {
            try
            {
                if (Application.IsRunning)
                {
                    ProcessCommandsAndEvents();
                }
            }
            catch (IOException)
            {
                Application.IsRunning = false;
            }
            return Application.IsRunning;
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
                Application.IsRunning = false;
            }
        }

        public IAsyncResult BeginHandleEvent(AsyncCallback callback, object state)
        {
            return _reader.BeginRead(callback, state);
        }

        public void EndHandleEvent(IAsyncResult asyncResult)
        {
            try
            {
                Application.IsRunning &= _reader.EndRead(asyncResult);
                if (Application.IsRunning)
                {
                    long eventType = _reader.ReadInt64();
                    ProcessEvent(eventType);
                }

            }
            catch (IOException)
            {
                Application.IsRunning = false;
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
                _commands.Add(command);
                _commandReceived.Set();
            }
        }

        internal void PostCommandBatch(IEnumerable<Commands.IHmeCommand> batch)
        {
            lock (_commands)
            {
                _commands.AddRange(batch);
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
                        foreach (Commands.IHmeCommand command in _commands)
                        {
                            SendCommand(command);
                        }
                        _commands.Clear();
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
                        foreach (Events.EventInfo eventInfo in _events)
                        {
                            eventInfo.RaiseEvent(Application);
                        }
                        _events.Clear();
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
                _events.Add(eventInfo);
                _eventReceived.Set();
            }
            ProtocolLog.Write(eventInfo);
        }
    }
}
