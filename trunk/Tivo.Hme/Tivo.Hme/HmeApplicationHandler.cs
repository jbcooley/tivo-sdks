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

namespace Tivo.Hme
{
    public class HmeApplicationStartArgs
    {
        public Application Application { get; set; }
        public IServiceProvider HostServices { get; set; }
    }

    public abstract class HmeApplicationHandler
    {
        private Uri _baseUri;

        protected HmeApplicationHandler()
        {
        }

        public Uri BaseUri
        {
            get { return _baseUri; }
            set { _baseUri = value; }
        }
	

        public abstract void OnApplicationStart(HmeApplicationStartArgs e);
        public abstract void OnApplicationEnd();
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ApplicationIconAttribute : Attribute
    {
        public ApplicationIconAttribute(Type resourceSource, string resourceName)
        {
            System.Resources.ResourceManager resMan = new System.Resources.ResourceManager(resourceSource);
            Icon = resMan.GetObject(resourceName) as byte[];
            if (Icon == null)
            {
                StatusLog.Write(System.Diagnostics.TraceEventType.Error,
                    string.Format("Unable to load resource {0} from resourceSet {1}.",
                    resourceName, resMan.BaseName));
            }
        }

        public byte[] Icon { get; private set; }
    }
}
