using System;
using System.IO;

namespace Tivo.Hme.Host
{
    class HmeApplicationHttpResponse : HttpResponse
    {
        public override void Write(Stream responseStream)
        {
            // start response
            StreamWriter writer = new StreamWriter(responseStream);
            writer.WriteLine("HTTP/1.1 200 OK");
            writer.WriteLine("Content-type: application/x-hme");
            writer.WriteLine();
            writer.Flush();
        }
    }
}
