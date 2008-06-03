//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3031
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tivo.Has
{
    
    public interface IHmeStream
    {
        bool CanRead
        {
            get;
        }
        bool CanWrite
        {
            get;
        }
        bool CanTimeout
        {
            get;
        }
        int ReadTimeout
        {
            get;
            set;
        }
        int WriteTimeout
        {
            get;
            set;
        }
        int Read(byte[] buffer, int offset, int count);
        int ReadByte();
        IHmeAsyncResult BeginRead(byte[] buffer, int offset, int count, IHmeAsyncCallback callback);
        int EndRead(IHmeAsyncResult asyncResult);
        void Write(byte[] buffer, int offset, int count);
        void WriteByte(byte value);
        IHmeAsyncResult BeginWrite(byte[] buffer, int offset, int count, IHmeAsyncCallback callback);
        void EndWrite(IHmeAsyncResult asyncResult);
        void Close();
        void Flush();
    }
}

