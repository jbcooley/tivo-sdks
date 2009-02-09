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

        public HttpApplicationHost(string appPath)
        {
            _aspHost = (SimpleAspNetHost)ApplicationHost.CreateApplicationHost(
            typeof(SimpleAspNetHost), "/", appPath);
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
            _aspHost.ProcessRequest(requestData, new HttpResponseWrapper(context.Response));
        }

        #endregion
    }

}
