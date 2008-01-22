// Copyright (c) 2008 Josh Cooley

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Tivo.Hme.Host
{
    /// <summary>
    /// Summary description for HmeReader
    /// </summary>
    internal class HmeReader : IDisposable
    {
        private Stream _input;
        private short _bytesLeft = 0;
        private byte[] _asyncBytes = new byte[1];
        private bool _useAsyncBytes = false;

        public HmeReader(Stream input)
        {
            _input = input;
        }

        #region IDisposable Members

        public void Dispose()
        {
            _input.Dispose();
        }

        #endregion

        public IAsyncResult BeginRead(AsyncCallback callback, object state)
        {
            return _input.BeginRead(_asyncBytes, 0, 1, callback, state);
        }

        public bool EndRead(IAsyncResult asyncResult)
        {
            _useAsyncBytes = (_input.EndRead(asyncResult) == 1);
            return _useAsyncBytes;
        }

        public bool ReadBoolean()
        {
            return Convert.ToBoolean(ChunkedReadByte());
        }

        public sbyte ReadSByte()
        {
            return unchecked((sbyte)ChunkedReadByte());
        }

        public int ReadInt32()
        {
            byte[] buffer = new byte[4];
            ChunckedReadBytes(buffer, 0, 4);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buffer);
            return BitConverter.ToInt32(buffer, 0);
        }

        public float ReadSingle()
        {
            byte[] buffer = new byte[4];
            ChunckedReadBytes(buffer, 0, 4);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buffer);
            return BitConverter.ToSingle(buffer, 0);
        }

        public long ReadInt64()
        {
            byte readByte;
            long value = 0;
            int count = 0;
            for (readByte = ChunkedReadByte(); (readByte & 0x80) != 0x80; readByte = ChunkedReadByte())
            {
                value |= ((uint)readByte << (7 * count));
                ++count;
            }
            value |= (((uint)readByte & 0x3F) << (7 * count));
            // check for sign
            if ((readByte & 0xC0) == 0xC0)
            {
                value = -value;
            }
            return value;
        }

        public ulong ReadUInt64()
        {
            byte readByte;
            ulong value = 0;
            int count = 0;
            for (readByte = ChunkedReadByte(); (readByte & 0x80) != 0x80; readByte = ChunkedReadByte())
            {
                value |= ((uint)readByte << (7 * count));
                ++count;
            }
            value |= ((ulong)(readByte & 0x7F) << (7 * count));
            return value;
        }

        public byte[] ReadBytes(int count)
        {
            byte[] buffer = new byte[count];
            ChunckedReadBytes(buffer, 0, count);
            return buffer;
        }

        public string ReadString()
        {
            int size = Convert.ToInt32(ReadUInt64());
            byte[] buffer = new byte[size];
            ChunckedReadBytes(buffer, 0, size);
            return Encoding.UTF8.GetString(buffer);
        }

        /// <summary>
        /// Will skip to start of next block based on
        /// zero terminators
        /// </summary>
        public void SkipToNext(Events.UnknownEventInfo unknownEventInfo)
        {
            short lookingForTerminator = _bytesLeft;
            do
            {
                _bytesLeft = lookingForTerminator;
                byte[] buffer = new byte[_bytesLeft];
                if (_input.Read(buffer, 0, _bytesLeft) != _bytesLeft)
                    throw new EndOfStreamException();
                unknownEventInfo.Add(buffer);
                _bytesLeft = 0;
                lookingForTerminator = ReadBytesLeft();
            } while (lookingForTerminator != 0);
        }

        private byte ChunkedReadByte()
        {
            if (_bytesLeft == 0)
            {
                _bytesLeft = ReadBytesLeft();
            }
            int readByte;
            if (_useAsyncBytes)
            {
                readByte = _asyncBytes[0];
                _useAsyncBytes = false;
            }
            else
            {
                readByte = _input.ReadByte();
            }
            --_bytesLeft;
            if (readByte == -1)
                throw new EndOfStreamException();
            return (byte)readByte;
        }

        private void ChunckedReadBytes(byte[] buffer, int offset, int count)
        {
            if (count == 0)
                return;
            if (_bytesLeft == 0)
                _bytesLeft = ReadBytesLeft();
            if (_useAsyncBytes)
            {
                buffer[offset] = _asyncBytes[0];
                _useAsyncBytes = false;
                ChunckedReadBytes(buffer, offset + 1, count - 1);
            }
            else
            {
                if (_bytesLeft < count)
                {
                    if (_input.Read(buffer, offset, _bytesLeft) != _bytesLeft)
                        throw new EndOfStreamException();
                    offset += _bytesLeft;
                    count -= _bytesLeft;
                    _bytesLeft = ReadBytesLeft();
                    ChunckedReadBytes(buffer, offset, count);
                }
                else
                {
                    if (_input.Read(buffer, offset, count) != count)
                        throw new EndOfStreamException();
                    _bytesLeft -= (short)count;
                }
            }
        }

        private short ReadBytesLeft()
        {
            return ReadInt16();
        }

        public void ReadTerminator()
        {
            short terminator = ReadInt16();
            if (terminator != 0)
                throw new InvalidOperationException();
        }

        private short ReadInt16()
        {
            byte[] buffer = new byte[2];
            if (_useAsyncBytes)
            {
                buffer[0] = _asyncBytes[0];
                _useAsyncBytes = false;
                if (_input.Read(buffer, 1, 1) != 1)
                    throw new EndOfStreamException();
            }
            else
            {
                if (_input.Read(buffer, 0, 2) != 2)
                    throw new EndOfStreamException();
            }
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buffer);
            return BitConverter.ToInt16(buffer, 0);
        }
    }
}
