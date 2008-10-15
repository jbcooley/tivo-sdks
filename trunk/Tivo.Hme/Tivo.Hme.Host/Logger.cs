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
            _log.Flush();
        }

        public static void Write(string message)
        {
            Write(TraceEventType.Verbose, message);
        }

        public static void Write(TraceEventType traceType, Exception exception)
        {
            _log.TraceData(traceType, exceptionTraceId, exception);
            _log.Flush();
        }

        public static void Write(Exception exception)
        {
            Write(TraceEventType.Verbose, exception);
        }
    }

    internal static class HttpLog
    {
        private static TraceSource _log = new TraceSource("HttpProtocol");
        private const int httpInProtocolTraceId = 1;
        private const int httpOutProtocolTraceId = 2;

        public static void WriteHttpIn(string message)
        {
            _log.TraceData(TraceEventType.Information, httpInProtocolTraceId, message);
            _log.Flush();
        }

        public static void WriteHttpOut(string message)
        {
            _log.TraceData(TraceEventType.Information, httpOutProtocolTraceId, message);
            _log.Flush();
        }
    }
}
