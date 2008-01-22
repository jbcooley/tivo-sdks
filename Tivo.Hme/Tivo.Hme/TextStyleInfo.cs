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
using System.Collections.ObjectModel;
using System.Drawing;

namespace Tivo.Hme
{
    public class GlyphInfo
    {
        private long _glyphId;
        private float _glyphAdvanceWidth;
        private float _glyphBoundingWidth;
        internal const int FieldCount = 3;

        internal GlyphInfo(long id, float advanceWidth, float boundingWidth)
        {
            _glyphId = id;
            _glyphAdvanceWidth = advanceWidth;
            _glyphBoundingWidth = boundingWidth;
        }

        public long GlyphId
        {
            get { return _glyphId; }
        }

        public float AdvanceWidth
        {
            get { return _glyphAdvanceWidth; }
        }

        public float BoundingWidth
        {
            get { return _glyphBoundingWidth; }
        }
    }

    public class GlyphInfoCollection : KeyedCollection<long, GlyphInfo>
    {
        public GlyphInfo this[char c]
        {
            get { return this[(long)c]; }
        }

        protected override long GetKeyForItem(GlyphInfo item)
        {
            return item.GlyphId;
        }
    }

    public class TextStyleInfo
    {
        private float _ascent;
        private float _descent;
        private float _height;
        private float _lineGap;
        private GlyphInfoCollection _glyphInfo = new GlyphInfoCollection();

        public TextStyleInfo(float ascent, float descent, float height, float lineGap, IEnumerable<GlyphInfo> glyphInfo)
        {
            _ascent = ascent;
            _descent = descent;
            _height = height;
            _lineGap = lineGap;
            foreach (GlyphInfo item in glyphInfo)
            {
                _glyphInfo.Add(item);
            }
        }

        public float Ascent
        {
            get { return _ascent; }
        }

        public float Descent
        {
            get { return _descent; }
        }

        public float Height
        {
            get { return _height; }
        }

        public float LineGap
        {
            get { return _lineGap; }
        }

        public GlyphInfoCollection GlyphInfo
        {
            get { return _glyphInfo; }
        }

        public SizeF MeasureText(string text)
        {
            float width = 0;
            int remainingChars = text.Length;
            foreach (char c in text)
            {
                // use bounding width for just the last char.
                // all other chars use advance width
                if (--remainingChars == 0)
                {
                    // add the actual width of the character
                    width += GlyphInfo[c].BoundingWidth;
                }
                else
                {
                    // add the amount of space between this character and the next.
                    width += GlyphInfo[c].AdvanceWidth;
                }
            }

            return new SizeF(width, Height);
        }
    }
}