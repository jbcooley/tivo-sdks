using System;
using System.Collections.Generic;
using System.AddIn.Contract;

namespace Tivo.Has.Contracts
{
    public interface IHmeStreamContract : IContract
    {
        bool CanRead { get; }
        bool CanWrite { get; }
        bool CanTimeout { get; }
        int ReadTimeout { get; set; }
        int WriteTimeout { get; set; }

        int Read(byte[] buffer, int offset, int count);
        int ReadByte();
        IHmeAsyncResultContract BeginRead(byte[] buffer, int offset, int count, IHmeAsyncCallbackContract callback);
        int EndRead(IHmeAsyncResultContract asyncResult);
        void Write(byte[] buffer, int offset, int count);
        void WriteByte(byte value);
        IHmeAsyncResultContract BeginWrite(byte[] buffer, int offset, int count, IHmeAsyncCallbackContract callback);
        void EndWrite(IHmeAsyncResultContract asyncResult);
        void Close();
        void Flush();
    }

    public interface IHmeAsyncResultContract : IContract
    {
        bool CompletedSynchronously { get; }
        bool IsCompleted { get; }
    }

    public interface IHmeAsyncCallbackContract : IContract
    {
        void AsyncCallback(IHmeAsyncResultContract asyncResult);
    }
}
