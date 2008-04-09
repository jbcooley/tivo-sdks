using System;
using System.Collections.Generic;
using System.Text;

namespace Tivo.Hme.Host
{
    public interface IHmeApplicationPump
    {
        void AddHmeConnection(HmeConnection connection);
    }
}
