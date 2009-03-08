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
using System.Linq;
using System.Text;
using Tivo.Hme.Host.Http;

namespace Tivo.Hme.Host.Services
{
    class HttpResponseWrapper : MarshalByRefObject
    {
        private HttpListenerResponse _response;

        public HttpResponseWrapper(HttpListenerResponse response)
        {
            _response = response;
        }

        public long ContentLength64
        {
            get { return _response.ContentLength64; }
            set { _response.ContentLength64 = value; }
        }

        public string ContentType
        {
            get { return _response.ContentType; }
            set { _response.ContentType = value; }
        }

        public void AddHeader(string name, string value)
        {
            _response.AddHeader(name, value);
        }

        public void Write(byte[] data, int offset, int count)
        {
            _response.OutputStream.Write(data, offset, count);
        }

        public void SetStatus(int statusCode, string statusDescription)
        {
            _response.StatusCode = statusCode;
            _response.StatusDescription = statusDescription;
        }

        internal void Flush(bool finalFlush)
        {
            _response.OutputStream.Flush();
            if (finalFlush)
                _response.OutputStream.Close();
        }
    }
}
