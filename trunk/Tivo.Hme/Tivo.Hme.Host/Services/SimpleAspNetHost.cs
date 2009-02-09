using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using System.Web;
using System.IO;
using System.Collections.Specialized;

namespace Tivo.Hme.Host.Services
{
    class SimpleAspNetHost : MarshalByRefObject
    {
        public void ProcessRequest(HttpRequestData requestData, HttpResponseWrapper response)
        {
            try
            {
                var wr = new HmeHostWorkerRequest(requestData, response);
                HttpRuntime.ProcessRequest(wr);
            }
            catch (IOException)
            {
                // ignore cases where the client closes the connection
            }
        }
    }
}
