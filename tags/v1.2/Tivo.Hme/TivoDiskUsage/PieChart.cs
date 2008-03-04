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

namespace TivoDiskUsage
{
    class PieChart
    {
        private List<Slice> _data = new List<Slice>();
        private ulong _total;

        public void AddSlice(string label, ulong value, Color color)
        {
            _data.Add(new Slice { Label = label, Value = value, Color = color });
            _total += value;
        }

        public void DrawChart(Graphics g, Rectangle rect)
        {
            // shrink by one to get the chart to completely fit
            rect.Width -= 1;
            rect.Height -= 1;
            float startAngle = 0;
            float nextAngle = 0;
            foreach(Slice slice in _data)
            {
                nextAngle = startAngle + 360F * slice.Value / _total;
                using (SolidBrush brush = new SolidBrush(slice.Color))
                {
                    g.FillPie(brush, rect, startAngle, nextAngle - startAngle);
                }
                startAngle = nextAngle;
            }
            startAngle = 0;
            foreach (Slice slice in _data)
            {
                nextAngle = startAngle + 360F * slice.Value / _total;
                g.DrawPie(Pens.Black, rect, startAngle, nextAngle - startAngle);
                startAngle = nextAngle;
            }
        }

        private class Slice
        {
            public string Label { get; set; }
            public ulong Value { get; set; }
            public Color Color { get; set; }
        }
    }
}
