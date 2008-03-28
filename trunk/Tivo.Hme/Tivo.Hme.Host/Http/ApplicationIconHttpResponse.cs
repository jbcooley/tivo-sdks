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

using System.IO;

namespace Tivo.Hme.Host.Http
{
    public class ApplicationIconHttpResponse : HttpResponse
    {
        private byte[] _icon;

        public ApplicationIconHttpResponse()
        {
            _icon = Properties.Resources.iconpng;
        }

        public ApplicationIconHttpResponse(byte[] icon)
        {
            _icon = icon;
        }

        public override void Write(Stream responseStream)
        {
            StreamWriter writer = new StreamWriter(responseStream);
            writer.WriteLine("HTTP/1.1 200 OK");
            writer.WriteLine("Content-type: image/png");
            writer.WriteLine("Content-Length: {0}", _icon.Length);
            writer.WriteLine("Connection: close");
            writer.WriteLine();
            writer.Flush();
            responseStream.Write(_icon, 0, _icon.Length);
            responseStream.Close();
        }
    }
}
