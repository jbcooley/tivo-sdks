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
using Tivo.Hme.Commands;

namespace Tivo.Hme
{
    public sealed class Animation
    {
        private List<IViewCommand> _commands = new List<IViewCommand>();

        public static Animation SetBounds(Rectangle newBounds, float ease, TimeSpan duration)
        {
            Animation animation = new Animation();
            animation.AddSetBounds(newBounds, ease, duration);
            return animation;
        }

        public void AddSetBounds(Rectangle newBounds, float ease, TimeSpan duration)
        {
            _commands.Add(new ViewSetBounds(newBounds, ease, duration));
        }

        public static Animation Move(Point newLocation, float ease, TimeSpan duration)
        {
            Animation animation = new Animation();
            animation.AddMove(newLocation, ease, duration);
            return animation;
        }

        public void AddMove(Point newLocation, float ease, TimeSpan duration)
        {
            _commands.Add(new ViewSetBounds(newLocation, ease, duration));
        }

        public static Animation Resize(Size newSize, float ease, TimeSpan duration)
        {
            Animation animation = new Animation();
            animation.AddResize(newSize, ease, duration);
            return animation;
        }

        public void AddResize(Size newSize, float ease, TimeSpan duration)
        {
            _commands.Add(new ViewSetBounds(newSize, ease, duration));
        }

        public static Animation Scale(SizeF newScale, float ease, TimeSpan duration)
        {
            Animation animation = new Animation();
            animation.AddScale(newScale, ease, duration);
            return animation;
        }

        public void AddScale(SizeF newScale, float ease, TimeSpan duration)
        {
            _commands.Add(new ViewSetScale(newScale, ease, duration));
        }

        public static Animation Translate(Point newOffset, float ease, TimeSpan duration)
        {
            Animation animation = new Animation();
            animation.AddTranslate(newOffset, ease, duration);
            return animation;
        }

        public void AddTranslate(Point newOffset, float ease, TimeSpan duration)
        {
            _commands.Add(new ViewSetTranslation(newOffset, ease, duration));
        }

        public static Animation Fade(float newTransparency, float ease, TimeSpan duration)
        {
            Animation animation = new Animation();
            animation.AddFade(newTransparency, ease, duration);
            return animation;
        }

        public void AddFade(float newTransparency, float ease, TimeSpan duration)
        {
            _commands.Add(new ViewSetTransparency(newTransparency, ease, duration));
        }

        public static Animation Visibility(bool newVisibility, float ease, TimeSpan duration)
        {
            Animation animation = new Animation();
            animation.AddVisibility(newVisibility, ease, duration);
            return animation;
        }

        public void AddVisibility(bool newVisibility, float ease, TimeSpan duration)
        {
            _commands.Add(new ViewSetVisible(newVisibility, ease, duration));
        }

        internal List<IViewCommand> Commands
        {
            get { return _commands; }
        }
    }
}
