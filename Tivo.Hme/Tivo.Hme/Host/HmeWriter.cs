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
    class HmeWriter : IDisposable
    {
        private Stream _output;
        private byte[] _smallBuffer = new byte[50];
        private int _bufferUsed = 0;

        public HmeWriter(Stream output)
        {
            _output = output;
        }

        #region IDisposable Members

        public void Dispose()
        {
            _output.Dispose();
        }

        #endregion

        public void Write(bool value)
        {
            CheckFlushBuffer(1);
            _smallBuffer[_bufferUsed++] = Convert.ToByte(value);
        }

        public void Write(sbyte value)
        {
            CheckFlushBuffer(1);
            _smallBuffer[_bufferUsed++] = unchecked((byte)value);
        }

        public void Write(int value)
        {
            CheckFlushBuffer(4);
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            Array.Copy(bytes, 0, _smallBuffer, _bufferUsed, 4);
            _bufferUsed += 4;
        }

        public void Write(float value)
        {
            CheckFlushBuffer(4);
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            Array.Copy(bytes, 0, _smallBuffer, _bufferUsed, 4);
            _bufferUsed += 4;
        }

        public void Write(long value)
        {
            CheckFlushBuffer(10);
            bool negative = false;
            if (value < 0)
            {
                negative = true;
                value = -value;
            }

            // the last byte can only have
            // six value bits since the 7th
            // bit will be the sign.
            while (value > 0x3F)
            {
                _smallBuffer[_bufferUsed++] = (byte)(value & 0x7F);
                value >>= 7;
            }

            if (negative)
            {
                _smallBuffer[_bufferUsed++] = (byte)(value | 0xC0);
            }
            else
            {
                _smallBuffer[_bufferUsed++] = (byte)(value | 0x80);
            }
        }

        public void Write(ulong value)
        {
            CheckFlushBuffer(10);
            while (value > 0x7F)
            {
                _smallBuffer[_bufferUsed++] = (byte)(value & 0x7F);
                value >>= 7;
            }
            _smallBuffer[_bufferUsed++] = (byte)(value | 0x80);
        }

        public void Write(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            CheckFlushBuffer(bytes.Length);
            Write((ulong)bytes.LongLength);
            Write(bytes);
        }

        private void CheckFlushBuffer(int neededBytes)
        {
            if (_bufferUsed + neededBytes > _smallBuffer.Length)
                Flush();
        }

        public void Write(byte[] data)
        {
            if (data.Length + _bufferUsed <= _smallBuffer.Length)
            {
                Array.Copy(data, 0, _smallBuffer, _bufferUsed, data.Length);
                _bufferUsed += data.Length;
            }
            else
            {
                Flush();
                for (int offset = 0; offset < data.Length; offset += ushort.MaxValue)
                {
                    if (data.Length - offset > ushort.MaxValue)
                    {
                        SendBytes(data, offset, ushort.MaxValue);
                    }
                    else
                    {
                        SendBytes(data, offset, (ushort)(data.Length - offset));
                    }
                }
            }
        }

        public void WriteTerminator()
        {
            Flush();
            byte[] data = { 0, 0 };
            _output.Write(data, 0, data.Length);
            _output.Flush();
        }

        private void Flush()
        {
            if (_bufferUsed != 0)
            {
                SendBytes(_smallBuffer, 0, (ushort)_bufferUsed);
                _bufferUsed = 0;
            }
            _output.Flush();
        }

        private void SendBytes(byte[] buffer, int offset, ushort bufferUsed)
        {
            short size = System.Net.IPAddress.HostToNetworkOrder((short)bufferUsed);
            byte[] bytes = BitConverter.GetBytes(size);
            _output.Write(bytes, 0, bytes.Length);
            _output.Write(buffer, offset, bufferUsed);
        }
    }
}
