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
    /// <summary>
    /// Provides data for the <see cref="Application.KeyDown"/>, <see cref="Application.KeyPress"/>, and <see cref="Application.KeyUp"/> events.
    /// </summary>
    public class KeyEventArgs : EventArgs
    {
        private KeyCode _keyCode;
        private long _rawCode;
        private bool _handled;

        /// <summary>
        /// Creates a KeyEventArgs.
        /// </summary>
        /// <param name="keyCode">The key code.</param>
        /// <param name="rawCode">The raw code for the key.</param>
        public KeyEventArgs(KeyCode keyCode, long rawCode)
        {
            _keyCode = keyCode;
            _rawCode = rawCode;
        }

        /// <summary>
        /// The <see cref="KeyCode"/> value.
        /// </summary>
        public KeyCode KeyCode
        {
            get { return _keyCode; }
        }

        /// <summary>
        /// The raw numerical key value.
        /// </summary>
        public long RawCode
        {
            get { return _rawCode; }
        }

        /// <summary>
        /// Handler should set this to true if the event has been handled.  If left to false will bubble up.
        /// </summary>
        public bool Handled
        {
            get { return _handled; }
            set { _handled = value; }
        }
    }
}
