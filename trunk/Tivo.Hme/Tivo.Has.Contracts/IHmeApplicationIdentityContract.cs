using System;
using System.AddIn.Contract;
using System.AddIn.Pipeline;

namespace Tivo.Has.Contracts
{
    public interface IHmeApplicationIdentityContract : IContract
    {
        string Name { get; }
        byte[] Icon { get; }
        Uri EndPoint { get; }
    }
}
