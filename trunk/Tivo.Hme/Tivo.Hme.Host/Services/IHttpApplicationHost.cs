// Copyright (c) 2009 Josh Cooley
// Copyright (c) 2009 David Sempek

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
using System.Web.Hosting;
using Tivo.Hme.Host.Http;

namespace Tivo.Hme.Host.Services
{
    public interface IHttpApplicationHost
    {
        void ProcessRequest(Uri baseUri, HttpListenerContext context);
    }

    class HttpApplicationHost : IHttpApplicationHost
    {
        private SimpleAspNetHost _aspHost;

        protected string AppPath { get; set; }

        public HttpApplicationHost(string appPath)
        {
            AppPath = appPath;
            Start();
        }

        private bool Start()
        {
            try
            {
                _aspHost = (SimpleAspNetHost)ApplicationHost.CreateApplicationHost(
                typeof(SimpleAspNetHost), "/", AppPath);
            }
            catch(Exception ex)
            {
                ServerLog.Write(ex);
                return false;
            }
            return true;
        }

        private bool CheckHostOk()
        {
            try
            {
                // If the _aspHost hasn't been destroyed this is always true
                // otherwise it throws an exception
                if (_aspHost.StillAlive) { }
            }
            catch (Exception ex)
            {
                ServerLog.Write(ex);
                return false;
            }
            return true;
        }

        #region IHttpApplicationHost Members

        public void ProcessRequest(Uri baseUri, HttpListenerContext context)
        {
            var requestData = new HttpRequestData
            {
                HttpVerb = context.Request.HttpMethod,
                HttpVersion = context.Request.ProtocolVersion.ToString(),
                RequestUrl = context.Request.Url,
                RemoteEndPoint = context.Request.RemoteEndPoint,
                VirtualDirectory = baseUri.AbsolutePath,
                RelativePagePath = context.Request.Url.LocalPath.Substring(baseUri.AbsolutePath.Length)
            };
            try
            {
                if (!CheckHostOk())
                {
                    Start();
                }
            }
            catch(Exception ex)
            {
                // We can't recover from this
                ServerLog.Write(ex);
                throw;
            }
            _aspHost.ProcessRequest(requestData, new HttpResponseWrapper(context.Response));
        }

        #endregion
    }

}
