// Copyright (c) 2009 Josh Cooley

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
using System.Linq;
using System.Text;
using System.Web;
using System.Net;

namespace Tivo.Hme.Host.Services
{
    [Serializable]
    class HttpRequestData
    {
        public string HttpVerb { get; set; }
        public string HttpVersion { get; set; }
        public Uri RequestUrl { get; set; }
        public IPEndPoint RemoteEndPoint { get; set; }
        public string VirtualDirectory { get; set; }
        public string RelativePagePath { get; set; }
    }

    class HmeHostWorkerRequest : HttpWorkerRequest
    {
        private HttpRequestData _requestData;
        private HttpResponseWrapper _response;

        public HmeHostWorkerRequest(HttpRequestData requestData, HttpResponseWrapper response)
        {
            _requestData = requestData;
            _response = response;
        }

        public override void EndOfRequest()
        {
        }

        public override void FlushResponse(bool finalFlush)
        {
            _response.Flush(finalFlush);
        }

        public override string GetHttpVerbName()
        {
            return _requestData.HttpVerb;
        }

        public override string GetHttpVersion()
        {
            return _requestData.HttpVersion;
        }

        public override string GetLocalAddress()
        {
            return _requestData.RequestUrl.DnsSafeHost;
        }

        public override int GetLocalPort()
        {
            return _requestData.RequestUrl.Port;
        }

        public override string GetQueryString()
        {
            return _requestData.RequestUrl.Query;
        }

        public override string GetRawUrl()
        {
            return _requestData.RequestUrl.OriginalString;
        }

        public override string GetRemoteAddress()
        {
            return _requestData.RemoteEndPoint.Address.ToString();
        }

        public override int GetRemotePort()
        {
            return _requestData.RemoteEndPoint.Port;
        }

        public override string GetUriPath()
        {
            return "/" + _requestData.RelativePagePath;
            //return _requestData.RequestUrl.AbsolutePath;
        }

        public override void SendKnownResponseHeader(int index, string value)
        {
            switch (index)
            {
                case HeaderContentType:
                    _response.ContentType = value;
                    break;
                case HeaderContentLength:
                    long contentLength;
                    if (long.TryParse(value, out contentLength))
                        _response.ContentLength64 = contentLength;
                    else
                        DefaultKnownResponseHeader(index, value);
                    break;
                default:
                    DefaultKnownResponseHeader(index, value);
                    break;
            }
        }

        private void DefaultKnownResponseHeader(int index, string value)
        {
            string knownResponseHeaderName = HttpWorkerRequest.GetKnownResponseHeaderName(index);
            SendUnknownResponseHeader(knownResponseHeaderName, value);
        }

        public override void SendResponseFromFile(IntPtr handle, long offset, long length)
        {
            Microsoft.Win32.SafeHandles.SafeFileHandle safeHandle =
                new Microsoft.Win32.SafeHandles.SafeFileHandle(handle, false);
            System.IO.Stream stream = new System.IO.FileStream(safeHandle, System.IO.FileAccess.Read);

            // TODO: see if there is more efficient way to do this
            // may be one that doesn't allocate all the memory up front
            byte[] data = new byte[length];
            stream.Seek(offset, System.IO.SeekOrigin.Begin);
            stream.Read(data, 0, data.Length);
            SendResponseFromMemory(data, data.Length);
        }

        public override void SendResponseFromFile(string filename, long offset, long length)
        {
            // TODO: see if there is more efficient way to do this
            // may be one that doesn't allocate all the memory up front
            using (System.IO.Stream stream = System.IO.File.OpenRead(filename))
            {
                byte[] data = new byte[length];
                stream.Seek(offset, System.IO.SeekOrigin.Begin);
                stream.Read(data, 0, data.Length);
                SendResponseFromMemory(data, data.Length);
            }
        }

        public override void SendResponseFromMemory(byte[] data, int length)
        {
            _response.Write(data, 0, length);
        }

        public override void SendStatus(int statusCode, string statusDescription)
        {
            _response.SetStatus(statusCode, statusDescription);
        }

        public override void SendUnknownResponseHeader(string name, string value)
        {
            _response.AddHeader(name, value);
        }

        public override void SendCalculatedContentLength(int contentLength)
        {
            base.SendCalculatedContentLength(contentLength);
        }

        public override void SendCalculatedContentLength(long contentLength)
        {
            base.SendCalculatedContentLength(contentLength);
        }
    }
}
