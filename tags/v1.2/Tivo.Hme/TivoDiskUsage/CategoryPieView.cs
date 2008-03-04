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
    class CategoryPieView : ImageView
    {
        private static readonly TextStyle _legendSdStyle = new TextStyle("default", FontStyle.Regular, 20);
        private static readonly TextStyle _legendHdStyle = new TextStyle("default", FontStyle.Regular, 30);
        private TextStyle _legendStyle;
        private PieChart _chart = new PieChart();
        private Dictionary<string, Color> _legend = new Dictionary<string, Color>();
        private string _chartName;
        private ulong _estimatedDiskSize;

        public CategoryPieView(DiskUsageCalculator calculator)
            : base(null, ImageLayout.HorizontalAlignLeft | ImageLayout.VerticalAlignCenter)
        {
            Margin = SafetyViewMargin.TitleMargin;

            foreach (string category in calculator.Categories)
            {
                ulong categoryTotal = calculator[category];
                _chart.AddSlice(category, categoryTotal, GetPieColor(category));
                _legend.Add(string.Format("{0} {1}%", category, categoryTotal * 100 / calculator.MaxSpaceUsed), GetPieColor(category));
            }
            _estimatedDiskSize = calculator.MaxSpaceUsed;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_chartName != null)
                {
                    Application.Images.Remove(_chartName);
                    _chartName = null;
                }
            }
        }

        protected override void OnNewApplication()
        {
            base.OnNewApplication();

            if (Application.CurrentResolution.Horizontal > 640)
                _legendStyle = _legendHdStyle;
            else
                _legendStyle = _legendSdStyle;
            Application.CreateTextStyle(_legendStyle, LegendTextStyleCreated, null);
            ShowChart();
        }

        private void ShowChart()
        {
            // calculate by width since we are reserving space for the legend
            // rather than fitting to the height.  This has the nice side effect
            // of making pie chart look a reasonable size on both HD and SD sets.
            int height = (int)(Application.CurrentResolution.Horizontal * 0.45);
            int width = height;
            using (Bitmap bm = new Bitmap(width, height))
            {
                using (Graphics g = Graphics.FromImage(bm))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    _chart.DrawChart(g, new Rectangle(0, 0, width, height));
                }
                _chartName = GenerateChartName();
                Application.Images.Add(_chartName, bm, System.Drawing.Imaging.ImageFormat.Png);
            }

            Update(Application.GetImageResource(_chartName));
        }

        void LegendTextStyleCreated(object sender, TextStyleCreatedArgs e)
        {
            ShowLegend(e.Info);
        }

        private void ShowLegend(TextStyleInfo info)
        {
            View legendView = new ColorView(Color.AntiqueWhite);
            // round height
            int lineHeight = (int)(info.Height + 0.5);
            // height is enough for each line plus half a line padding plus line for total
            int boxHeight = lineHeight * (_legend.Count + 2);
            float longestLabel = 0;
            foreach (string label in _legend.Keys)
            {
                longestLabel = Math.Max(longestLabel, info.MeasureText(label).Width);
            }
            // width is long enough for the longest line + color palete + padding
            // using Height for width and height of color palete (shrunk a little for margin)
            int boxWidth = (int)(longestLabel + 0.5 + 2 * lineHeight);
            legendView.Bounds = new Rectangle(Bounds.Width - boxWidth, 0, boxWidth, boxHeight);
            Children.Add(legendView);

            // leave a half line space at the top (and bottom since the whole thing has an extra line height)
            int topText = lineHeight / 2;
            // small left margin for black border. actual color starts one more pixel in
            int leftColor = lineHeight / 4;
            // enough space for the color.  Subtract on since the black border gives too much margin
            int leftText = lineHeight / 2 + lineHeight - 1;
            // subtract three from color size to get 1 pixel margin between boxes plus a pixel for top and bottom black border
            int colorSize = lineHeight - 3;
            foreach (var legendEntry in _legend)
            {
                ColorView colorBorder = new ColorView(Color.Black);
                colorBorder.Bounds = new Rectangle(leftColor, topText, colorSize + 2, colorSize + 2);
                ColorView legendColor = new ColorView(legendEntry.Value);
                legendColor.Bounds = new Rectangle(1, 1, colorSize, colorSize);
                colorBorder.Children.Add(legendColor);
                legendView.Children.Add(colorBorder);
                TextView legendText = new TextView(legendEntry.Key, _legendStyle, Color.Black, TextLayout.HorizontalAlignLeft);
                legendText.Bounds = new Rectangle(leftText, topText, boxWidth-leftText, lineHeight);
                legendView.Children.Add(legendText);
                topText += lineHeight;
            }
            TextView diskSizeText = new TextView("Estimated Disk Size " + GetPrettySize(_estimatedDiskSize),
                new TextStyle(_legendStyle.Name, _legendStyle.Style, _legendStyle.Weight * 3 / 4),
                Color.Black, TextLayout.HorizontalAlignCenter);
            diskSizeText.Bounds = new Rectangle(0, topText + lineHeight / 4, boxWidth, lineHeight);
            legendView.Children.Add(diskSizeText);
            topText += lineHeight;
        }

        private static string GetPrettySize(ulong size)
        {
            if (size < 1000)
                return size.ToString() + " bytes";
            else if (size < 1000000)
            {
                double unitSize = size / 1000.0;
                return string.Format(GetFormatString(unitSize) + " KB", unitSize);
            }
            else if (size < 1000000000)
            {
                double unitSize = size / 1000000.0;
                return string.Format(GetFormatString(unitSize) + " MB", unitSize);
            }
            else
            {
                double unitSize = size / 1000000000.0;
                // even though terabyte disks are a possibility, don't try to show that.
                return string.Format(GetFormatString(unitSize) + " GB", unitSize);
            }
        }

        private static string GetFormatString(double unitSize)
        {
            if (unitSize < 10)
                return "{0:F2}";
            if (unitSize < 100)
                return "{0:F1}";
            return "{0:F0}";
        }

        private static long chartNumber = 0;
        private static string GenerateChartName()
        {
            return "DiskUsagePieChart" + System.Threading.Interlocked.Increment(ref chartNumber);
        }

        private static Color GetPieColor(string categoryName)
        {
            switch (categoryName)
            {
                case DiskUsageCalculator.InProgress:
                    return Color.FromArgb(0x66, 0xFF, 0x66); // light green
                case DiskUsageCalculator.SaveUntilIDelete:
                    return Color.FromArgb(0x00, 0x7A, 0x00); // dark green
                case DiskUsageCalculator.Current:
                    return Color.FromArgb(0x99, 0x6B, 0x00); // dark orange
                case DiskUsageCalculator.ExpiresSoon:
                    return Color.FromArgb(0xFF, 0xD1, 0x66); // light orange
                case DiskUsageCalculator.Expired:
                    return Color.FromArgb(0xFF, 0x00, 0x00); // red
                case DiskUsageCalculator.Suggestion:
                    return Color.FromArgb(0xFF, 0xC0, 0xC0); // pink
                case DiskUsageCalculator.Free:
                    return Color.FromArgb(0x19, 0x19, 0xB3); // dark blue
                default:
                    return Color.Black;
            }
        }
    }
}
