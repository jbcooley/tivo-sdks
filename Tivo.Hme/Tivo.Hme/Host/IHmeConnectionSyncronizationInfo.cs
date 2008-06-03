using System;
using System.Collections.Generic;
using System.Text;

namespace Tivo.Hme.Host
{
    public interface IHmeConnectionSyncronizationInfo
    {
        string EventReceivedName { get; }
        string CommandReceivedName { get; }
    }
}
