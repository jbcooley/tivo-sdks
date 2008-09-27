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

namespace Tivo.Hme.Commands
{
    enum Transition : long
    {
        Forward = 1,
        Back = 2,
        Teleport = 3
    }

    class ReceiverTransition : IHmeCommand
    {
        private const long Command = 61;
        private const long _rootStreamId = 1;

        private const sbyte endMarker = 0;
        private const sbyte stringMarker = 1;
        private const sbyte treeMarker = 2;

        private string _destination;
        private Transition _type;
        private TivoTree _parameters;
        private byte[] _savedData;

        public ReceiverTransition(string destination, Transition type, TivoTree parameters, byte[] savedData)
        {
            // back - destination and data ignored
            // teleport - data ignored
            _destination = destination;
            _type = type;
            _parameters = parameters;
            _savedData = savedData;
        }

        #region IHmeCommand Members

        public void SendCommand(HmeConnection connection)
        {
            connection.Writer.Write(Command);
            connection.Writer.Write(_rootStreamId);
            connection.Writer.Write(_destination ?? string.Empty);
            connection.Writer.Write((long)_type);
            if (_parameters != null)
                WriteParameters(connection, _parameters);
            else
                connection.Writer.Write(string.Empty);
            if (_savedData == null)
            {
                connection.Writer.Write((long)0);
            }
            else
            {
                connection.Writer.Write(_savedData.LongLength);
                connection.Writer.Write(_savedData);
            }
        }

        #endregion

        private static void WriteParameters(HmeConnection connection, TivoTree _parameters)
        {
            // TODO: values must be sorted when there is a child (ie, this is a dictionary entry)
            foreach (string key in _parameters)
            {
                connection.Writer.Write(key);
                if (_parameters.GetValueCount(key) == 0)
                {
                    connection.Writer.Write(endMarker);
                }
                else
                {
                    foreach (var child in _parameters.GetValues(key))
                    {
                        TivoTree childTree = child as TivoTree;
                        if (childTree == null)
                        {
                            connection.Writer.Write(stringMarker);
                            connection.Writer.Write(GetFirstAndOnly(child));
                        }
                        else
                        {
                            connection.Writer.Write(treeMarker);
                            WriteParameters(connection, childTree);
                        }
                    }
                    connection.Writer.Write(endMarker);
                }
            }
            connection.Writer.Write(string.Empty);
        }

        private static string GetFirstAndOnly(IEnumerable<string> items)
        {
            bool gotFirst = false;
            string first = null;
            foreach (var item in items)
            {
                if (gotFirst)
                    throw new InvalidOperationException();
                first = item;
                gotFirst = true;
            }
            if (!gotFirst)
                throw new InvalidOperationException();
            return first;
        }
    }
}
