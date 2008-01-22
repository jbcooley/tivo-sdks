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
    public struct Margin : IEquatable<Margin>
    {
        private int _left;
        private int _top;
        private int _right;
        private int _bottom;

        public Margin(int uniformMargin)
            : this(uniformMargin, uniformMargin, uniformMargin, uniformMargin)
        {
        }

        public Margin(int left, int top, int right, int bottom)
        {
            _left = left;
            _top = top;
            _right = right;
            _bottom = bottom;
        }

        public int Left
        {
            get { return _left; }
            set { _left = value; }
        }

        public int Top
        {
            get { return _top; }
            set { _top = value; }
        }

        public int Right
        {
            get { return _right; }
            set { _right = value; }
        }

        public int Bottom
        {
            get { return _bottom; }
            set { _bottom = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj is Margin)
                return this == (Margin)obj;
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Margin m1, Margin m2)
        {
            return m1._left == m2._left &&
                m1._top == m2._top &&
                m1._right == m2._right &&
                m1._bottom == m2._bottom;
        }

        public static bool operator !=(Margin m1, Margin m2)
        {
            return !(m1 == m2);
        }

        public override string ToString()
        {
            return string.Concat(_left.ToString(), ",", _top.ToString(), ",", _right.ToString(), ",", _bottom.ToString());
        }

        #region IEquatable<Margin> Members

        public bool Equals(Margin other)
        {
            return this == other;
        }

        #endregion
    }
}
