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
    public class ColorView : View
    {
        private Color _color;

        public ColorView(Color color)
        {
            Update(color);
        }

        public Color Color
        {
            get { return _color; }
            set { Update(value); }
        }

        public void Update(Color color)
        {
            if (ResourceId != 0 && Application != null)
            {
                Application.ReleaseResourceId(ResourceId);
            }
            _color = color;
            if (Application != null)
            {
                ResourceId = Application.GetResourceId(new Resource(_color));
                PostCommand(new Commands.ViewSetResource(ViewId, ResourceId, 0));
            }
        }

        protected override void OnNewApplication()
        {
            ResourceId = Application.GetResourceId(new Resource(_color));
            PostCommand(new Commands.ViewSetResource(ViewId, ResourceId, 0));
            base.OnNewApplication();
        }
    }
}
