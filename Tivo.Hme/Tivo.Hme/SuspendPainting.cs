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
    /// Helper class for calling <see cref="View.SuspendPainting"/> and <see cref="View.ResumePainting"/> within the scope of a using statement.
    /// </summary>
    /// <example>
    /// <code>
    /// using (new SuspendPainting(view))
    /// {
    ///     // painting suspended in this block
    /// }
    /// </code>
    /// </example>
    public class SuspendPainting : IDisposable
    {
        private View _view;

        /// <summary>
        /// Creates a SuspendPainting object that suspends painting for a view.
        /// </summary>
        /// <param name="view">A view to suspend painting.</param>
        public SuspendPainting(View view)
        {
            _view = view;
            _view.SuspendPainting();
        }

        #region IDisposable Members

        /// <summary>
        /// Resumes painting the view.
        /// </summary>
        public void Dispose()
        {
            _view.ResumePainting();
        }

        #endregion
    }
}
