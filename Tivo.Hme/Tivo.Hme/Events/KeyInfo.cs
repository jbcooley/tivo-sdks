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
    enum KeyAction : long
    {
        Press = 1,
        Repeat = 2,
        Release = 3
    }

    class KeyInfo : EventInfo
    {
        public const long Type = 4;
        private long _resourceId;
        private KeyAction _keyAction;
        private long _keyCode;
        private long _rawCode;

        public KeyInfo() { }

        internal KeyInfo(long resourceId, KeyAction action, KeyCode keyCode, long rawCode)
        {
            _resourceId = resourceId;
            _keyAction = action;
            _keyCode = (long)keyCode;
            _rawCode = rawCode;
        }

        public long ResourceId
        {
            get { return _resourceId; }
        }

        public KeyAction KeyAction
        {
            get { return _keyAction; }
        }

        public long KeyCode
        {
            get { return _keyCode; }
        }

        public long RawCode
        {
            get { return _rawCode; }
        }

        public override void Read(HmeReader reader)
        {
            _resourceId = reader.ReadInt64();
            _keyAction = (KeyAction)reader.ReadInt64();
            _keyCode = reader.ReadInt64();
            _rawCode = reader.ReadInt64();
            reader.ReadTerminator();
        }

        public override void RaiseEvent(Application application)
        {
            application.OnKeyInfoReceived(this);
        }

        internal void Write(HmeWriter writer)
        {
            writer.Write(Type);
            writer.Write(_resourceId);
            writer.Write((long)_keyAction);
            writer.Write(_keyCode);
            writer.Write(_rawCode);
        }

        public override string ToString()
        {
            return string.Format("{0}: (ResourceId,{1})(KeyAction,{2})(KeyCode,{3})(RawCode,{4})",
                GetType().Name, ResourceId, KeyAction, KeyCode, RawCode);
        }
    }
}
