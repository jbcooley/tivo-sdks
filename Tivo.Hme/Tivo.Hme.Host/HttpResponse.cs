using System;
using System.Collections.Generic;
using System.Text;

namespace Tivo.Hme.Host
{
    public abstract class HttpResponse
    {
        public abstract void Write(System.IO.Stream responseStream);
    }
}
