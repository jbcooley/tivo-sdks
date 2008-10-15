using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections.Specialized;
using System.Net;

namespace Tivo.Hme.Host.Services
{
    internal class HttpResponse : IHttpResponse
    {
        private Http.HttpRequest _request;

        internal HttpResponse(Http.HttpRequest request)
        {
            _request = request;
        }

        #region IHttpResponse Members

        public Stream GetResponseStream(int statusCode, NameValueCollection headers)
        {
            ResponseHeaders response = new ResponseHeaders(_request.Protocol, statusCode, headers);
            _request.WriteResponse(response);
            return response.ResponseStream;
        }

        #endregion

        private class ResponseHeaders : Http.HttpResponse
        {
            // rfc2616 - section 6.1.1
            static string[][] ReasonPhrase = new string[][]
            {
                new string[] { "Continue", "Switching Protocols" }, // 100 - 101
                new string[] { "OK", "Created", "Accepted", "Non-Authoritative Information",
                    "No Content", "Reset Content", "Partial Content" }, // 200 - 206
                new string[] { "Multiple Choices", "Moved Permanently", "Found", "See Other",
                    "Not Modified", "Use Proxy", string.Empty, "Temporary Redirect" }, // 300 - 307
                new string[] { "Bad Request", "Unauthorized", "Payment Required", "Forbidden",
                    "Not Found", "Method Not Allowed", "Not Acceptable", "Proxy Authentication Required",
                    "Request Timeout", "Conflict", "Gone", "Length Required", "Precondition Failed",
                    "Request Entity Too Large", "Request-URI Too Large", "Unsupported Media Type",
                    "Request range not satisfiable", "Expectation Failed" }, // 400 - 417
                new string[] { "Internal Server Error", "Not Implemented", "Bad Gateway",
                    "Service Unavailable", "Gateway Time-out", "HTTP Version not supported" } // 500 - 505
            };

            private string _protocol;
            private int _statusCode;
            private string _reasonPhrase;
            private Http.HttpHeaderCollection _headers;

            public ResponseHeaders(string protocol, int statusCode, NameValueCollection headers)
            {
                _protocol = protocol;
                _statusCode = statusCode;
                _reasonPhrase = ReasonPhrase[statusCode / 100 - 1][statusCode % 100];
                _headers = new Http.HttpHeaderCollection();
                if (headers != null)
                {
                    _headers.Add(headers);
                }
                if (StringComparer.InvariantCultureIgnoreCase.Compare(_headers[HttpResponseHeader.Connection], "close") != 0)
                {
                    _headers[HttpResponseHeader.Connection] = "close";
                }
            }

            public override void Write(Stream responseStream)
            {
                // not going to dispose of writer
                // since we aren't done with stream.
                StreamWriter writer = new StreamWriter(responseStream);
                writer.WriteLine("{0} {1} {2}", _protocol, _statusCode, _reasonPhrase);
                writer.Write(_headers.ToString());
                writer.Flush();
                // not really honoring contract here
                // but I don't care since this is an internal
                // implementation that is compatible with the
                // implementation of the HttpRequest.WriteResponse method
                ResponseStream = responseStream;
            }

            public Stream ResponseStream { get; private set; }
        }

    }
}
