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
    /// <summary>
    /// Resolution and aspect ratio
    /// </summary>
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

        /// <summary>
        /// Horizontal resolution
        /// </summary>
        public long Horizontal
        {
            get { return _horizontal; }
            set { _horizontal = value; }
        }

        /// <summary>
        /// Vertical resolution
        /// </summary>
        public long Vertical
        {
            get { return _vertical; }
            set { _vertical = value; }
        }

        /// <summary>
        /// pixel aspect ratio - width
        /// </summary>
        public long PixelAspectWidth
        {
            get { return _pixelAspectWidth; }
            set { _pixelAspectWidth = value; }
        }

        /// <summary>
        /// pixel aspect ratio - height
        /// </summary>
        public long PixelAspectHeight
        {
            get { return _pixelAspectHeight; }
            set { _pixelAspectHeight = value; }
        }

        /// <summary>
        /// Returns a readable version of the resolution data.
        /// </summary>
        /// <returns>Returns a readable version of the resolution data.</returns>
        public override string ToString()
        {
            return string.Format("Resolution {0}x{1} {2}:{3}", Horizontal, Vertical, PixelAspectWidth, PixelAspectHeight);
        }

        /// <summary>
        /// Tests for equality between two ResolutionInfo
        /// </summary>
        /// <param name="obj">Must be a ResolutionInfo</param>
        /// <returns>true if the two represent the same resolution and aspect ratio; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj is ResolutionInfo)
                return Equals((ResolutionInfo)obj);
            return false;
        }

        /// <summary>
        /// returns the hash code such that two equal resources have the same hash code value.
        /// </summary>
        /// <returns>a hash code</returns>
        public override int GetHashCode()
        {
            return _horizontal.GetHashCode() ^ _vertical.GetHashCode() ^ _pixelAspectHeight.GetHashCode() ^ _pixelAspectWidth.GetHashCode();
        }

        #region IEquatable<ResolutionInfo> Members

        /// <summary>
        /// Tests for equality between two ResolutionInfo
        /// </summary>
        /// <param name="other">Must be a ResolutionInfo</param>
        /// <returns>true if the two represent the same resolution and aspect ratio; false otherwise.</returns>
        public bool Equals(ResolutionInfo other)
        {
            return this._horizontal == other._horizontal &&
                this._vertical == other._vertical &&
                this._pixelAspectHeight == other._pixelAspectHeight &&
                this._pixelAspectWidth == other._pixelAspectWidth;
        }

        #endregion

        /// <summary>
        /// Tests for equality between two ResolutionInfo
        /// </summary>
        /// <param name="lhs">a ResolutionInfo</param>
        /// <param name="rhs">a ResolutionInfo</param>
        /// <returns>true if the two represent the same resolution and aspect ratio; false otherwise.</returns>
        public static bool operator ==(ResolutionInfo lhs, ResolutionInfo rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Tests for inequality between two ResolutionInfo
        /// </summary>
        /// <param name="lhs">a ResolutionInfo</param>
        /// <param name="rhs">a ResolutionInfo</param>
        /// <returns>true if the two represent the different resolution or aspect ratio; false otherwise.</returns>
        public static bool operator !=(ResolutionInfo lhs, ResolutionInfo rhs)
        {
            return !lhs.Equals(rhs);
        }
    }
}
