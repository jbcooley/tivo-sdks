using System;
using System.Collections.Generic;
using System.Text;

namespace Tivo.Hme.Host
{
    public abstract class HmeServerApplication
    {
        protected HmeServerApplication()
        {
        }

        public abstract void OnApplicationStart(HmeApplicationConnectedEventArgs e);
        public abstract void OnApplicationEnd();
    }
}
