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
using System.Web;

namespace Tivo.Hme.Host.Services
{
    public class Mp3Handler : IHttpHandler
    {
        #region IHttpHandler Members

        public virtual bool IsReusable
        {
            get { return true; }
        }

        public virtual void ProcessRequest(HttpContext context)
        {
            // TODO: read query portion of RequestUri for seek if supporting trick play.
            var seekQueryString = context.Request.QueryString["Seek"];
            if (!string.IsNullOrEmpty(seekQueryString))
            {
                // TODO: set position in stream to support trick play
                // The value of the "Seek" parameter is the time in the file to seek to, in milliseconds.
            }
            context.Response.ContentType = "audio/mpeg3";
            string mp3Path = context.Request.MapPath(context.Request.Path);
            string contentLength = new System.IO.FileInfo(mp3Path).Length.ToString();
            // TODO: support X-TiVo-Accurate-Duration by including the duration in milliseconds
            context.Response.AppendHeader("Content-Length", contentLength);
            context.Response.WriteFile(mp3Path);
        }

        #endregion
    }
}
