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
