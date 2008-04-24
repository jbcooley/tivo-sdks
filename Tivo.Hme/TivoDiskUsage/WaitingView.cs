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
using System.Drawing;
using Tivo.Hme;

namespace TivoDiskUsage
{
    class WaitingView : TextView
    {
        private static readonly Color ForeColor = Color.FromArgb(173, 191, 209);
        private ColorView _indicator;
        private bool _disposed;
        private bool _waiting = true;

        public WaitingView()
            : base("Loading Data...", new TextStyle("system", FontStyle.Italic | FontStyle.Bold, 40), ForeColor)
        {
            Margin = SafetyViewMargin.TitleMargin;
            _indicator = new ColorView(ForeColor);
            Children.Add(_indicator);
        }

        public void DisplayFailure(string reason)
        {
            _waiting = false;
            Update("Unable to load data. " + reason,
                new TextStyle("system", FontStyle.Italic | FontStyle.Regular, 20),
                ForeColor, TextLayout.HorizontalAlignLeft | TextLayout.TextWrap);
        }

        protected override void OnNewApplication()
        {
            base.OnNewApplication();

            MoveIndicator(null);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _disposed = true;
        }

        protected override void OnBoundsChanged(BoundsChangedArgs e)
        {
            // if bounds change, need to move indicator 
            // using new coordinates.
            MoveIndicator(null);
            base.OnBoundsChanged(e);
        }

        private static readonly TimeSpan indicatorDuration = TimeSpan.FromSeconds(4);

        private void MoveIndicator(object ignore)
        {
            if (!_disposed && _waiting)
            {
                _indicator.Bounds = new Rectangle(-40, Bounds.Height * 2 / 3, 60, 10);
                _indicator.Animate(Animation.Move(new Point(Bounds.Right - 20, _indicator.Bounds.Top), 0, indicatorDuration));
                Application.DelayAction(indicatorDuration, (object)null, MoveIndicator);
            }
        }
    }
}
