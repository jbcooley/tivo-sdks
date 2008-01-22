using System;
using System.Collections.Generic;
using System.Text;

namespace Tivo.Hme.Samples
{
    class FontInfo :HmeApplicationHandler
    {
        private FontView _fontView;

        public override void OnApplicationStart(HmeApplicationStartArgs e)
        {
            e.Application.Root = new View();
            _fontView = new FontView();
            e.Application.TextStyleCreated += _fontView.application_TextStyleCreated;
            e.Application.Root.Children.Add(_fontView);
        }

        public override void OnApplicationEnd()
        {
        }

        private class FontView : TextView
        {
            public FontView()
                : base("", new TextStyle("default", System.Drawing.FontStyle.Bold, 36), System.Drawing.Color.White, TextLayout.TextWrap)
            {
                Margin = SafetyViewMargin.TitleMargin;
            }

            public void application_TextStyleCreated(object sender, TextStyleCreatedArgs e)
            {
                Update("Font Info:  default.ttf 36 Bold\n" +
                    "height:     " + e.Info.Height + "\n" +
                    "ascent:     " + e.Info.Ascent + "\n" +
                    "descent:    " + e.Info.Descent + "\n" +
                    "linegap:    " + e.Info.LineGap + "\n" +
                    "l advance:  " + e.Info.GlyphInfo['l'].AdvanceWidth + "\n" +
                    "M advance:  " + e.Info.GlyphInfo['M'].AdvanceWidth);

                // resize the view vertically to exactly fit the text
                int newHeight = (int)(e.Info.Height * 8);
                int newY = 240 - newHeight / 2;
                Bounds = new System.Drawing.Rectangle(0, newY, 640, newHeight);

                // create a header view that is sized to the exact top area above the font info
                ColorView header = new ColorView(System.Drawing.Color.Gray);
                header.Bounds = new System.Drawing.Rectangle(0, 0, 640, newY);
                Parent.Children.Add(header);
                TextView headerText = new TextView("Header", Style, System.Drawing.Color.Blue);
                headerText.Bounds = new System.Drawing.Rectangle(0, 0, 640, newY);
                header.Children.Add(headerText);

                // create a footer view that is sized to the exact area below the font info
                ColorView footer = new ColorView(System.Drawing.Color.Gray);
                footer.Bounds = new System.Drawing.Rectangle(0, newY + newHeight, 640, newY);
                Parent.Children.Add(footer);

                // create a text resource and put in a view that is sized-to-fit the width
                string someText = "Program Your TV!\u00ae";
                System.Drawing.SizeF someTextSize = e.Info.MeasureText(someText);
                int someTextWidth = (int)Math.Ceiling(someTextSize.Width);

                // set the BG color behind the text
                ColorView textBackground = new ColorView(System.Drawing.Color.Red);
                textBackground.Bounds = new System.Drawing.Rectangle(0, 0, someTextWidth, newY);
                footer.Children.Add(textBackground);

                // create the view that contains the text
                TextView footerText = new TextView(someText, Style, System.Drawing.Color.Cyan);
                footerText.Bounds = textBackground.Bounds;
                footer.Children.Add(footerText);

                // position the footer to the right of the text
                TextView footerText2 = new TextView("Footer", Style, System.Drawing.Color.Blue);
                footerText2.Bounds = new System.Drawing.Rectangle(someTextWidth, 0, 640 - someTextWidth, newY);
                footer.Children.Add(footerText2);
            }
        }
    }
}
