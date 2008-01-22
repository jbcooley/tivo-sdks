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
using System.Drawing;
using Tivo.Hme.Host;

namespace Tivo.Hme.Commands
{
    class ViewSetBounds : IViewCommand
    {
        private const long Command = 2;
        private long _viewId;
        private long _left;
        private long _top;
        private long _width;
        private long _height;
        // for the animation
        private float _ease;
        private TimeSpan _duration;

        public ViewSetBounds(View view)
        {
            _viewId = view.ViewId;
            _left = view.Bounds.Left;
            _top = view.Bounds.Top;
            _width = view.Bounds.Width;
            _height = view.Bounds.Height;
            _ease = 0;
            _duration = TimeSpan.Zero;
        }

        public ViewSetBounds(Rectangle bounds)
        {
            _left = bounds.Left;
            _top = bounds.Top;
            _width = bounds.Width;
            _height = bounds.Height;
            _ease = 0;
            _duration = TimeSpan.Zero;
        }

        public ViewSetBounds(Rectangle bounds, float ease, TimeSpan duration)
        {
            _left = bounds.Left;
            _top = bounds.Top;
            _width = bounds.Width;
            _height = bounds.Height;
            _ease = ease;
            _duration = duration;
        }

        public ViewSetBounds(Point location, float ease, TimeSpan duration)
        {
            _left = location.X;
            _top = location.Y;
            _width = -1;
            _height = -1;
            _ease = ease;
            _duration = duration;
        }

        public ViewSetBounds(Size size, float ease, TimeSpan duration)
        {
            _left = -1;
            _top = -1;
            _width = size.Width;
            _height = size.Height;
            _ease = ease;
            _duration = duration;
        }

        #region IViewCommand Members

        public long ViewId
        {
            get { return _viewId; }
            set { _viewId = value; }
        }

        public void UseView(View view)
        {
            _viewId = view.ViewId;
            if (_left == -1) _left = view.Bounds.Left;
            if (_top == -1) _top = view.Bounds.Top;
            if (_width == -1) _width = view.Bounds.Width;
            if (_height == -1) _height = view.Bounds.Height;
        }

        #endregion

        #region IHmeCommand Members

        public void SendCommand(HmeConnection connection)
        {
            long animationId = 0;
            if (_duration != TimeSpan.Zero)
                animationId = connection.Application.GetResourceId(new Resource(_ease, _duration));
            connection.Writer.Write(Command);
            connection.Writer.Write(_viewId);
            connection.Writer.Write(_left);
            connection.Writer.Write(_top);
            connection.Writer.Write(_width);
            connection.Writer.Write(_height);
            connection.Writer.Write(animationId);
        }

        #endregion
    }
}
