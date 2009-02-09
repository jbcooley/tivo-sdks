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
