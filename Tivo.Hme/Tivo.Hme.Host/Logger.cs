using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Tivo.Hme.Host
{
    internal static class ServerLog
    {
        private static TraceSource _log = new TraceSource("ServerStatus");
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
