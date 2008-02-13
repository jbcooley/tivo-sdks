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
    public class TextView : View
    {
        private string _text;
        private TextStyle _style;
        private Color _color;
        private TextLayout _layout;

        public TextView(string text, TextStyle style, Color color)
            : this(text, style, color, 0)
        {
        }

        public TextView(string text, TextStyle style, Color color, TextLayout layout)
        {
            Update(text, style, color, layout);
        }

        public string Text
        {
            get { return _text; }
            // call Update command when setting text
            // this will internally update the member
            set { Update(value); }
        }

        public TextStyle Style
        {
            get { return _style; }
        }

        public Color Color
        {
            get { return _color; }
        }

        public TextLayout Layout
        {
            get { return _layout; }
        }

        public void Update(string text)
        {
            Update(text, _style, _color, _layout);
        }

        public void Update(string text, TextStyle style, Color color)
        {
            Update(text, style, color, _layout);
        }

        public void Update(string text, TextStyle style, Color color, TextLayout layout)
        {
            if (ResourceId != 0 && Application != null)
            {
                Application.ReleaseResourceId(ResourceId);
            }
            _text = text;
            _style = style;
            _color = color;
            _layout = layout;
            if (Application != null)
            {
                Create();
            }
        }

        protected override void OnNewApplication()
        {
            Create();
            base.OnNewApplication();
        }

        private void Create()
        {
            // ensure color gets created before text
            Application.GetResourceId(new Resource(_color));
            ResourceId = Application.GetResourceId(new Resource(_text, _style, _color));
            PostCommand(new Commands.ViewSetResource(ViewId, ResourceId, (long)Layout));
        }
    }
}
