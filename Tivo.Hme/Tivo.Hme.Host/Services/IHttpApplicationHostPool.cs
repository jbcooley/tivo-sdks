// Copyright (c) 2009 Josh Cooley

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
using System.Linq;
using System.Text;

namespace Tivo.Hme.Host.Services
{
    public interface IHttpApplicationHostPool
    {
        IHttpApplicationHost GetHost(string appPath);
        void Release(string appPath);
    }

    class HttpApplicationHostPool : IHttpApplicationHostPool
    {
        private Dictionary<string, IHttpApplicationHost> _applicationHostPool = new Dictionary<string, IHttpApplicationHost>();

        #region IHttpApplicationHostPool Members

        public IHttpApplicationHost GetHost(string appPath)
        {
            IHttpApplicationHost host;
            if (!_applicationHostPool.TryGetValue(appPath, out host))
            {
                host = new HttpApplicationHost(appPath);
                _applicationHostPool.Add(appPath, host);
            }
            return host;
        }

        public void Release(string appPath)
        {
            _applicationHostPool.Remove(appPath);
        }

        #endregion
    }
}
