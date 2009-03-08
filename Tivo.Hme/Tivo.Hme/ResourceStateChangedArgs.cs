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
    /// <summary>
    /// Provides data for the <see cref="Application.ResourceStateChanged"/> event.
    /// </summary>
    public class ResourceStateChangedArgs : EventArgs
    {
        private IHmeResource _resource;
        private ResourceStatus _status;
        private Dictionary<string, string> _resourceInfo;

        /// <summary>
        /// The key for the <see cref="ResourceInfo"/> dictionary containing speed data.
        /// </summary>
        public const string Speed = "speed";
        /// <summary>
        /// The key for the <see cref="ResourceInfo"/> dictionary containing position data.
        /// </summary>
        public const string Position = "pos";

        /// <summary>
        /// Creates a <see cref="ResourceStateChangedArgs"/>.
        /// </summary>
        /// <param name="resource">The resource that changed state.</param>
        /// <param name="status">The status of the resource.</param>
        /// <param name="info">A collection of name/value pairs defining additional resource information.</param>
        public ResourceStateChangedArgs(IHmeResource resource, ResourceStatus status, Dictionary<string, string> info)
        {
            _resource = resource;
            _status = status;
            _resourceInfo = info;
        }

        /// <summary>
        /// The resource that is changing state.
        /// </summary>
        public IHmeResource Resource
        {
            get { return _resource; }
        }

        /// <summary>
        /// <see cref="ResourceStatus"/> value for the resource.
        /// </summary>
        public ResourceStatus Status
        {
            get { return _status; }
        }

        /// <summary>
        /// A collection of name/value pairs defining additional resource information.
        /// </summary>
        public Dictionary<string, string> ResourceInfo
        {
            get { return _resourceInfo; }
        }
    }

    /// <summary>
    /// Specifies the type of resource error encountered by the application
    /// </summary>
    public enum ResourceErrorCode
    {
        /// <summary>
        /// Indicates the error is not known
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// A data format error.
        /// </summary>
        BadDataFormat = 1,
        /// <summary>
        /// A bad magic number given the content type.
        /// </summary>
        BadMagicNumber = 2,
        /// <summary>
        /// The version of media is not supported
        /// </summary>
        MediaVersionNotSupported = 3,
        /// <summary>
        /// Connection was lost unexpectedly.
        /// </summary>
        ConnectionLost = 4,
        /// <summary>
        /// Connection timed-out.
        /// </summary>
        ConnectionTimeout = 5,
        /// <summary>
        /// Connection failed.
        /// </summary>
        ConnectionFailed = 6,
        /// <summary>
        /// Specified host name was not found.
        /// </summary>
        HostNotFound = 7,
        /// <summary>
        /// The version of media is incompatible.
        /// </summary>
        IncompatibleMediaVersion = 8,
        /// <summary>
        /// Media type is not supported.
        /// </summary>
        MediaTypeNotSupported = 9,
        /// <summary>
        /// A bad argument was specified.
        /// </summary>
        BadArgument = 20,
        /// <summary>
        /// The state is invalid.
        /// </summary>
        BadState = 21
    }

    /// <summary>
    /// Provides data for the <see cref="Application.ResourceErrorOccurred"/> event.
    /// </summary>
    public class ResourceErrorArgs : EventArgs
    {
        private IHmeResource _resource;
        private ResourceErrorCode _code;
        private string _text;

        /// <summary>
        /// Creates a <see cref="ResourceErrorArgs"/>.
        /// </summary>
        /// <param name="resource">The resource with an error.</param>
        /// <param name="code">The code for the error.</param>
        /// <param name="text">A textual representation of the error.</param>
        public ResourceErrorArgs(IHmeResource resource, ResourceErrorCode code, string text)
        {
            _resource = resource;
            _code = code;
            _text = text;
        }

        /// <summary>
        /// The resource with an error.
        /// </summary>
        public IHmeResource Resource
        {
            get { return _resource; }
        }

        /// <summary>
        /// An <see cref="ResourceErrorCode"/> value that represents the error.
        /// </summary>
        public ResourceErrorCode Code
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
