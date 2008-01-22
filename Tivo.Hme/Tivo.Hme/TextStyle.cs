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
using System.Drawing;

namespace Tivo.Hme
{
    public struct TextStyle
    {
        private string _name;
        private FontStyle _style;
        private float _weight;

        // font names available by default
        // default - id 10
        // system - id 11
        public TextStyle(string fontName, FontStyle fontStyle, float fontWeight)
        {
            if (fontWeight < 0 || fontWeight > 256)
                throw new ArgumentOutOfRangeException("fontWeight", fontWeight, "Fonts with negative size or a size greater than 256 are not supported");
            _name = fontName;
            _style = fontStyle;
            _weight = fontWeight;
        }

        public string Name
        {
            get { return _name; }
        }

        public FontStyle Style
        {
            get { return _style; }
        }

        public float Weight
        {
            get { return _weight; }
        }

        public override string ToString()
        {
            return _name + "-" + _style.ToString() + "-" + _weight.ToString();
        }
    }
}
