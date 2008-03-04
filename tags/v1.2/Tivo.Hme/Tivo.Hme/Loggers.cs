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
using System.Diagnostics;

namespace Tivo.Hme
{
    internal static class ProtocolLog
    {
        private static TraceSource _log = new TraceSource("HmeProtocol");
        private const int commandTraceId = 1;
        private const int eventTraceId = 2;

        public static void Write(TraceEventType traceType, Commands.IHmeCommand data)
        {
            _log.TraceData(traceType, commandTraceId, data);
        }

        public static void Write(Commands.IHmeCommand data)
        {
            Write(TraceEventType.Verbose, data);
        }

        public static void Write(TraceEventType traceType, Events.EventInfo data)
        {
            _log.TraceData(traceType, eventTraceId, data);
        }

        public static void Write(Events.EventInfo data)
        {
            Write(TraceEventType.Verbose, data);
        }
    }

    internal static class StatusLog
    {
        private static TraceSource _log = new TraceSource("ApplicationStatus");
        private const int messageTraceId = 1;
        private const int exceptionTraceId = 2;

        public static void Write(TraceEventType traceType, string message)
        {
            _log.TraceData(traceType, messageTraceId, message);
        }

        public static void Write(string message)
        {
            Write(TraceEventType.Verbose, message);
        }

        public static void Write(TraceEventType traceType, Exception exception)
        {
            _log.TraceData(traceType, exceptionTraceId, exception);
        }

        public static void Write(Exception exception)
        {
            Write(TraceEventType.Verbose, exception);
        }
    }
}
