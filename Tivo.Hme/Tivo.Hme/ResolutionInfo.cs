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

namespace Tivo.Hme
{
    public class ResolutionInfo : IEquatable<ResolutionInfo>
    {
        private long _horizontal;
        private long _vertical;
        private long _pixelAspectWidth;
        private long _pixelAspectHeight;
        internal const int FieldCount = 4;

        internal ResolutionInfo(long horizontal, long vertical, long pixelAspectWidth, long pixelAspectHeight)
        {
            _horizontal = horizontal;
            _vertical = vertical;
            _pixelAspectWidth = pixelAspectWidth;
            _pixelAspectHeight = pixelAspectHeight;
        }

        public long Horizontal
        {
            get { return _horizontal; }
            set { _horizontal = value; }
        }

        public long Vertical
        {
            get { return _vertical; }
            set { _vertical = value; }
        }

        public long PixelAspectWidth
        {
            get { return _pixelAspectWidth; }
            set { _pixelAspectWidth = value; }
        }

        public long PixelAspectHeight
        {
            get { return _pixelAspectHeight; }
            set { _pixelAspectHeight = value; }
        }

        public override string ToString()
        {
            return string.Format("Resolution {0}x{1} {2}:{3}", Horizontal, Vertical, PixelAspectWidth, PixelAspectHeight);
        }

        public override bool Equals(object obj)
        {
            if (obj is ResolutionInfo)
                return Equals((ResolutionInfo)obj);
            return false;
        }

        public override int GetHashCode()
        {
            return _horizontal.GetHashCode() ^ _vertical.GetHashCode() ^ _pixelAspectHeight.GetHashCode() ^ _pixelAspectWidth.GetHashCode();
        }

        #region IEquatable<ResolutionInfo> Members

        public bool Equals(ResolutionInfo other)
        {
            return this._horizontal == other._horizontal &&
                this._vertical == other._vertical &&
                this._pixelAspectHeight == other._pixelAspectHeight &&
                this._pixelAspectWidth == other._pixelAspectWidth;
        }

        #endregion

        public static bool operator ==(ResolutionInfo lhs, ResolutionInfo rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(ResolutionInfo lhs, ResolutionInfo rhs)
        {
            return !lhs.Equals(rhs);
        }
    }
}
