using System;
using System.Threading;
using System.AddIn.Contract;
using System.AddIn.Pipeline;

namespace Tivo.Has.Contracts
{
    [AddInContract]
    public interface IHmeConnectionContract : IContract
    {
        WaitHandle CommandReceived { get; }
        WaitHandle EventReceived { get; }
    }
}
