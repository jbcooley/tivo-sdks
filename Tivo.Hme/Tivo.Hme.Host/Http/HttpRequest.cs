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
using System.Net.Sockets;
using System.IO;
using System.Text;

namespace Tivo.Hme.Host.Http
{
    public class HttpRequest
    {
        private HttpHeaderCollection _headers = new HttpHeaderCollection();
        private NetworkStream _stream;
        private Uri _requestUri;

        internal HttpRequest(TcpClient request)
        {
            _stream = request.GetStream();
            string firstLine = GetNextLine();
            if (!firstLine.StartsWith("GET"))
            {
                throw new NotSupportedException();
            }
            string[] firstLineTokens = firstLine.Split(' ');
            if (firstLineTokens.Length != 3)
            {
                throw new NotSupportedException();
            }
            _requestUri = new Uri(firstLineTokens[1], UriKind.RelativeOrAbsolute);
            string headerLine;
            while ((headerLine = GetNextLine()).Length != 0)
            {
                _headers.Add(headerLine);
            }
        }

        public void WriteResponse(HttpResponse httpResponse)
        {
            httpResponse.Write(Stream);
        }

        public HttpHeaderCollection Headers
        {
            get { return _headers; }
        }

        public NetworkStream Stream
        {
            get { return _stream; }
        }

        public Uri RequestUri
        {
            get { return _requestUri; }
        }

        private string GetNextLine()
        {
            StringBuilder builder = new StringBuilder();
            int readByte;
            while ((readByte = _stream.ReadByte()) != '\n' && readByte != -1)
            {
                if (readByte != '\r')
                    builder.Append((char)readByte);
            }
            return builder.ToString();
        }
    }
}
