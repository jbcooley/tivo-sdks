using System;
using System.Threading;
using System.AddIn.Contract;
using System.AddIn.Pipeline;

namespace Tivo.Has.Contracts
{
    public interface IHmeConnectionContract : IContract
    {
        string CommandReceivedName { get; }
        string EventReceivedName { get; }
    }
}
