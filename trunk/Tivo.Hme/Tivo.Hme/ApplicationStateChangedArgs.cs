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
    /// Provides data for the <see cref="Application.ApplicationStateChanged"/> event.
    /// </summary>
    public class ApplicationStateChangedArgs : EventArgs
    {
        private bool _applicationStarting;
        private bool _applicationStopping;

        /// <summary>
        /// true if the application is now starting; false otherwise
        /// </summary>
        public bool ApplicationStarting
        {
            get { return _applicationStarting; }
            set { _applicationStarting = value; }
        }

        /// <summary>
        /// true if the application is now stopping; false otherwise
        /// </summary>
        public bool ApplicationStopping
        {
            get { return _applicationStopping; }
            set { _applicationStopping = value; }
        }
    }

    /// <summary>
    /// Specifies the type of error encountered by the application
    /// </summary>
    public enum ApplicationErrorCode
    {
        /// <summary>
        /// Indicates the error is not known
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// An argument passed to a command was invalid.
        /// </summary>
        BadArgument = 1,
        /// <summary>
        /// Command is either unsupported or in a bad format
        /// </summary>
        CommandNotUnderstood = 2,
        /// <summary>
        /// The resource specified is not found.
        /// </summary>
        ResourceNotFound = 3,
        /// <summary>
        /// The view specified is not found.
        /// </summary>
        ViewNotFound = 4,
        /// <summary>
        /// The tivo device is out of hme application memory.
        /// </summary>
        OutOfMemory = 5,
        /// <summary>
        /// Generic error with information contained in <see cref="ApplicationErrorArgs.Text"/>.
        /// </summary>
        ConsultErrorText = 100
    }

    /// <summary>
    /// Provides data for the <see cref="Application.ApplicationErrorOccurred"/> event.
    /// </summary>
    public class ApplicationErrorArgs : EventArgs
    {
        private ApplicationErrorCode _code;
        private string _text;

        /// <summary>
        /// Creates an ApplicationErrorArgs.
        /// </summary>
        /// <param name="code">The code for the error.</param>
        /// <param name="text">A textual representation of the error.</param>
        public ApplicationErrorArgs(ApplicationErrorCode code, string text)
        {
            _code = code;
            _text = text;
        }

        /// <summary>
        /// An <see cref="ApplicationErrorCode"/> value that represents the error.
        /// </summary>
        public ApplicationErrorCode Code
        {
            get { return _code; }
            set { _code = value; }
        }

        /// <summary>
        /// A textual representation of the error.
        /// </summary>
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
    }
}
