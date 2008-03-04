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
    public class ApplicationStateChangedArgs : EventArgs
    {
        private bool _applicationStarting;
        private bool _applicationStopping;

        public bool ApplicationStarting
        {
            get { return _applicationStarting; }
            set { _applicationStarting = value; }
        }

        public bool ApplicationStopping
        {
            get { return _applicationStopping; }
            set { _applicationStopping = value; }
        }
    }

    public enum ApplicationErrorCode
    {
        Unknown = 0,
        BadArgument = 1,
        CommandNotUnderstood = 2,
        ResourceNotFound = 3,
        ViewNotFound = 4,
        OutOfMemory = 5,
        ConsultErrorText = 100
    }

    public class ApplicationErrorArgs : EventArgs
    {
        private ApplicationErrorCode _code;
        private string _text;

        public ApplicationErrorArgs(ApplicationErrorCode code, string text)
        {
            _code = code;
            _text = text;
        }

        public ApplicationErrorCode Code
        {
            get { return _code; }
            set { _code = value; }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
    }
}
