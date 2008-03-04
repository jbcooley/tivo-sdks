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

namespace Tivo.Hme
{
    // use long since hme type is vint
    // this and TextLayout come from resource layout
    /// <summary>
    /// Settings to be combined in specifying how an image should be placed in a view.
    /// </summary>
    [Flags]
    public enum ImageLayout : long
    {
        /// <summary>
        /// Align the image with the left border of the view.
        /// </summary>
        HorizontalAlignLeft = 0x0001,
        /// <summary>
        /// Center the image horizontally between the borders of the view.
        /// </summary>
        HorizontalAlignCenter = 0x0002,
        /// <summary>
        /// Align the image with the right border of the view.
        /// </summary>
        HorizontalAlignRight = 0x0004,
        /// <summary>
        /// Align the image with the top border of the view.
        /// </summary>
        VerticalAlignTop = 0x0010,
        /// <summary>
        /// Center the image vertically between the borders of the view.
        /// </summary>
        VerticalAlignCenter = 0x0020,
        /// <summary>
        /// Align the image with the bottom border of the view.
        /// </summary>
        VerticalAlignBottom = 0x0040,
        /// <summary>
        /// Size image to fit the width of the view.  Height is adjusted as needed.
        /// </summary>
        HorizontalFit = 0x1000,
        /// <summary>
        /// Size image to fit the height of the view.  Width is adjusted as needed.
        /// </summary>
        VerticalFit = 0x2000,
        /// <summary>
        /// Size image to fit width or height, which ever will show the entire image without exceeding the bounds.
        /// </summary>
        BestFit = 0x4000
    }
}