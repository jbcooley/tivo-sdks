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
    class ApplicationParametersInfo : EventInfo
    {
        public const long Type = 7;
        private const long _rootStreamId = 1;

        private const sbyte endMarker = 0;
        private const sbyte stringMarker = 1;
        private const sbyte treeMarker = 2;

        private TivoTree _parameters = new TivoTree();
        private byte[] _data;

        public TivoTree Parameters
        {
            get { return _parameters; }
        }

        public byte[] Data
        {
            get { return _data; }
        }

        public override void Read(HmeReader reader)
        {
            long streamId = reader.ReadInt64();
            System.Diagnostics.Debug.Assert(_rootStreamId == streamId);

            TivoTree currentLevel = _parameters;
            ReadParameters(reader, currentLevel);
            _data = reader.ReadBytes((int)reader.ReadInt64());
            reader.ReadTerminator();
        }

        private static void ReadParameters(HmeReader reader, TivoTree currentLevel)
        {
            for (string value = reader.ReadString(); value.Length != 0; value = reader.ReadString())
            {
                currentLevel.AddKey(value);
                for (sbyte marker = reader.ReadSByte(); marker != endMarker; marker = reader.ReadSByte())
                {
                    switch (marker)
                    {
                        case stringMarker:
                            currentLevel.Add(value, reader.ReadString());
                            break;
                        case treeMarker:
                            TivoTree tree = new TivoTree();
                            ReadParameters(reader, tree);
                            currentLevel.Add(value, tree);
                            break;
                    }
                }
            }
        }

        public override void RaiseEvent(Application application)
        {
            application.OnApplicationParametersInfoReceived(this);
        }
    }
}
