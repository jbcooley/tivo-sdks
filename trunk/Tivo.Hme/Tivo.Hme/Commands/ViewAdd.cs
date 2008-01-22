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
    class ViewAdd : IViewCommand
    {
        private const long Command = 1;
        private long _newViewId;
        private long _parentViewId;
        private long _left;
        private long _top;
        private long _width;
        private long _height;
        private bool _visible;

        public ViewAdd(View parent, System.Drawing.Rectangle bounds, bool visible)
        {
            _parentViewId = parent.ViewId;
            _left = bounds.Left;
            _top = bounds.Top;
            _width = bounds.Width;
            _height = bounds.Height;
            _visible = visible;
        }

        public ViewAdd(long newViewId, View parent, System.Drawing.Rectangle bounds, bool visible)
        {
            _newViewId = newViewId;
            _parentViewId = parent.ViewId;
            _left = bounds.Left;
            _top = bounds.Top;
            _width = bounds.Width;
            _height = bounds.Height;
            _visible = visible;
        }

        #region IViewCommand Members

        public long ViewId
        {
            get { return _newViewId; }
            set { _newViewId = value; }
        }

        public void UseView(View view)
        {
            _newViewId = view.ViewId;
        }

        #endregion

        #region IHmeCommand Members

        public void SendCommand(HmeConnection connection)
        {
            connection.Writer.Write(Command);
            connection.Writer.Write(_newViewId);
            connection.Writer.Write(_parentViewId);
            connection.Writer.Write(_left);
            connection.Writer.Write(_top);
            connection.Writer.Write(_width);
            connection.Writer.Write(_height);
            connection.Writer.Write(_visible);
        }

        #endregion
    }
}
