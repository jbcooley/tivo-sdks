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

namespace Tivo.Hme
{
    public class ResourceStateChangedArgs : EventArgs
    {
        private IHmeResource _resource;
        private ResourceStatus _status;
        private Dictionary<string, string> _resourceInfo;

        public const string Speed = "speed";
        public const string Position = "pos";

        public ResourceStateChangedArgs(IHmeResource resource, ResourceStatus status, Dictionary<string, string> info)
        {
            _resource = resource;
            _status = status;
            _resourceInfo = info;
        }

        public IHmeResource Resource
        {
            get { return _resource; }
        }

        public ResourceStatus Status
        {
            get { return _status; }
        }

        public Dictionary<string, string> ResourceInfo
        {
            get { return _resourceInfo; }
        }
    }

    public enum ResourceErrorCode
    {
        Unknown = 0,
        BadDataFormat = 1,
        BadMagicNumber = 2,
        MediaVersionNotSupported = 3,
        ConnectionLost = 4,
        ConnectionTimeout = 5,
        ConnectionFailed = 6,
        HostNotFound = 7,
        IncompatibleMediaVersion = 8,
        MediaTypeNotSupported = 9,
        BadArgument = 20,
        BadState = 21
    }

    public class ResourceErrorArgs : EventArgs
    {
        private IHmeResource _resource;
        private ResourceErrorCode _code;
        private string _text;

        public ResourceErrorArgs(IHmeResource resource, ResourceErrorCode code, string text)
        {
            _resource = resource;
            _code = code;
            _text = text;
        }

        public IHmeResource Resource
        {
            get { return _resource; }
        }
         

        public ResourceErrorCode Code
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
