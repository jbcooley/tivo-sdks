using System;
using System.IO;

namespace Tivo.Hme.Host
{
    class ApplicationIconHttpResponse : HttpResponse
    {
        public override void Write(Stream responseStream)
        {
            byte[] image = File.ReadAllBytes(@"C:\npgsql\icon.png");
            StreamWriter writer = new StreamWriter(responseStream);
            writer.WriteLine("HTTP/1.1 200 OK");
            writer.WriteLine("Content-type: image/png");
            writer.WriteLine("Content-Length: {0}", image.Length);
            writer.WriteLine("Connection: close");
            writer.WriteLine();
            writer.Flush();
            responseStream.Write(image, 0, image.Length);
            responseStream.Close();
        }
    }
}
