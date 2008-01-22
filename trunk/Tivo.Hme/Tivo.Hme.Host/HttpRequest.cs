using System;
using System.Net.Sockets;
using System.IO;
using System.Text;

namespace Tivo.Hme.Host
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
            while ((readByte = _stream.ReadByte()) != '\n')
            {
                if (readByte != '\r')
                    builder.Append((char)readByte);
            }
            return builder.ToString();
        }
    }
}
