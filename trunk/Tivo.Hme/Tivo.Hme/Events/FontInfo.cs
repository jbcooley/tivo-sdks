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
    class FontInfo : EventInfo
    {
        public const long Type = 6;
        private long _fontId;
        private float _ascent;
        private float _descent;
        private float _height;
        private float _lineGap;
        private long _metricsPerGlyph; // number of fields in GlyphInfo
        // count followed by data
        private List<GlyphInfo> _glyphInfo = new List<GlyphInfo>();

        public long FontId
        {
            get { return _fontId; }
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

        public List<GlyphInfo> GlyphInfo
        {
            get { return _glyphInfo; }
        }

        public override void Read(HmeReader reader)
        {
            _fontId = reader.ReadInt64();
            _ascent = reader.ReadSingle();
            _descent = reader.ReadSingle();
            _height = reader.ReadSingle();
            _lineGap = reader.ReadSingle();
            _metricsPerGlyph = reader.ReadInt64();
            long count = reader.ReadInt64();
            for (long i = 0; i < count; ++i)
            {
                _glyphInfo.Add(new GlyphInfo(reader.ReadInt64(), reader.ReadSingle(), reader.ReadSingle()));
                // eat up remaining metrics
                for (long j = Tivo.Hme.GlyphInfo.FieldCount; j < _metricsPerGlyph; ++j)
                {
                    reader.ReadSingle();
                }
            }
            reader.ReadTerminator();
        }

        public override void RaiseEvent(Application application)
        {
            application.OnFontInfoReceived(this);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(GetType().Name);
            builder.Append(": ");
            builder.AppendFormat("(FontId,{0})(Ascent,{1})(Descent,{2})(Height,{3})(LineGap,{4})(MetricsPerGlyph,{5}) ",
                FontId, Ascent, Descent, Height, LineGap, _metricsPerGlyph);
            foreach (GlyphInfo info in _glyphInfo)
            {
                builder.AppendFormat("(GlyphId,{0})(AdvanceWidth,{1})(BoundingWidth,{2})",
                    info.GlyphId, info.AdvanceWidth, info.BoundingWidth);
            }
            return builder.ToString();
        }
    }
}
