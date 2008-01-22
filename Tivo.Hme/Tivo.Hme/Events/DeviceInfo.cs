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
using Tivo.Hme.Host;

namespace Tivo.Hme.Events
{
    class DeviceInfo : EventInfo
    {
        public const long Type = 1;
        private const long _rootStreamId = 1;
        // count followed by string string ...
        private Dictionary<string, string> _deviceInfo = new Dictionary<string, string>();

        public Dictionary<string, string> Info
        {
            get { return _deviceInfo; }
        }

        public override void Read(HmeReader reader)
        {
            long streamId = reader.ReadInt64();
            System.Diagnostics.Debug.Assert(_rootStreamId == streamId);
            long count = reader.ReadInt64();
            for (long i = 0; i < count; ++i)
            {
                _deviceInfo.Add(reader.ReadString(), reader.ReadString());
            }
            reader.ReadTerminator();
        }

        public override void RaiseEvent(Application application)
        {
            application.OnDeviceInfoReceived(this);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(GetType().Name);
            builder.Append(": ");
            foreach (KeyValuePair<string, string> entry in _deviceInfo)
            {
                builder.AppendFormat("({0},{1})", entry.Key, entry.Value);
            }
            return builder.ToString();
        }
    }
}
