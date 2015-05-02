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
using System.Net;
using System.Xml.Serialization;
using System.Net.Security;

namespace Tivo.Hmo
{
    public enum TivoConnectionState
    {
        Closed,
        Open,
        Downloading
    }

    public class TivoConnection : IDisposable
    {
        private bool _disposed;
        private string _hmoServer;
        private string _mediaAccessKey;
        private WebClient _webClient;

        public TivoConnection(string hmoServer, string mediaAccessKey)
        {
            _hmoServer = hmoServer;
            _mediaAccessKey = mediaAccessKey;
            // optional -- 4.4.2 QueryServer
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                Close();
                // required -- 4.4.3 ResetServer
                // Currently times out - may be because of active TivoDesktop stuff.
                //WebClient client = new WebClient();
                //client.Credentials = new NetworkCredential("tivo", _mediaAccessKey);
                //client.QueryString.Add("Command", "ResetServer");
                //Uri uri = new Uri("https://" + _hmoServer + "/TiVoConnect");
                //using (System.IO.Stream reader = client.OpenRead(uri)) { }
            }
            _disposed = true;
        }

        public TivoConnectionState State
        {
            get
            {
                if (_webClient == null)
                    return TivoConnectionState.Closed;
                else if (_webClient.IsBusy)
                    return TivoConnectionState.Downloading;
                else
                    return TivoConnectionState.Open;
            }
        }

        internal WebClient WebClient
        {
            get { return _webClient; }
        }

        public string HmoServer
        {
            get { return _hmoServer; }
        }

        public void Open()
        {
            if (_webClient != null)
            {
                throw new InvalidOperationException();
            }
            _webClient = new CookieHandlingWebClient();
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(TrustAllCertificatePolicy.TrustAllCertificateCallback);
            _webClient.Credentials = new NetworkCredential("tivo", _mediaAccessKey);

        }

        public void Close()
        {
            if (_webClient != null)
            {
                _webClient.Dispose();
                _webClient = null;
            }
        }

        public TivoContainerQuery CreateContainerQuery(string container)
        {
            return new TivoContainerQuery(this, container);
        }

        public TivoContainerQuery CreateContainerQuery(TivoContainer container)
        {
            return new TivoContainerQuery(this, new Uri(container.ContentUrl));
        }

        public ContentDownloader GetDownloader(TivoVideo video)
        {
            return new ContentDownloader(this, new Uri(video.ContentUrl));
        }

        public System.Xml.Linq.XDocument GetTivoVideoDetailsDocument(TivoVideoDetails tivoVideoDetails)
        {
            WebClient.QueryString.Clear();
            using (var stream = WebClient.OpenRead(tivoVideoDetails.Uri))
            using (var reader = new System.IO.StreamReader(stream))
            {
                return System.Xml.Linq.XDocument.Load(reader);
            }
        }

        public class TrustAllCertificatePolicy
        {
            public static bool TrustAllCertificateCallback(object sender,
                System.Security.Cryptography.X509Certificates.X509Certificate cert,
                System.Security.Cryptography.X509Certificates.X509Chain chain,
                System.Net.Security.SslPolicyErrors errors)
            {
                return true;
            }
        }

        private class CookieHandlingWebClient : WebClient
        {
            private CookieContainer _container = new CookieContainer();

            internal WebRequest GetBaseWebRequest(Uri address)
            {
                var request = base.GetWebRequest(address);
                var httpRequest = request as HttpWebRequest;
                if (httpRequest != null)
                    httpRequest.CookieContainer = _container;
                return request;
            }

            protected override WebRequest GetWebRequest(Uri address)
            {
                return new RecreatableWebRequest(this, address);
            }

            protected override WebResponse GetWebResponse(WebRequest request)
            {
                var credentials = request.Credentials;
                if (request.RequestUri.Scheme == "http")
                    request.Credentials = null;
                string setCookieHeader = null;
                string host;
                try
                {
                    return base.GetWebResponse(request);
                }
                catch (WebException e)
                {
                    if (e.Response == null || (((HttpWebResponse)e.Response).StatusCode != HttpStatusCode.Unauthorized))
                        throw;
                    if (!(request is RecreatableWebRequest))
                        throw;
                    setCookieHeader = e.Response.Headers[HttpResponseHeader.SetCookie];
                    host = ((HttpWebResponse)e.Response).ResponseUri.Host;
                    ((RecreatableWebRequest)request).Recreate();
                    request.Credentials = credentials;
                }
                if (setCookieHeader != null)
                    _container.Add(CookieParser.ParseCookie(setCookieHeader, host));
                return base.GetWebResponse(request);
            }
        }

        private class RecreatableWebRequest : WebRequest
        {
            private CookieHandlingWebClient _webClient;
            private Uri _address;

            internal RecreatableWebRequest(CookieHandlingWebClient webClient, Uri address)
            {
                _webClient = webClient;
                _address = address;
                Recreate();
            }

            public void Recreate()
            {
                InnerRequest = _webClient.GetBaseWebRequest(_address);
            }

            public WebRequest InnerRequest { get; private set; }

            public override void Abort()
            {
                InnerRequest.Abort();
            }

            public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
            {
                return InnerRequest.BeginGetRequestStream(callback, state);
            }

            public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
            {
                return InnerRequest.BeginGetResponse(callback, state);
            }

            public override System.Net.Cache.RequestCachePolicy CachePolicy
            {
                get
                {
                    return InnerRequest.CachePolicy;
                }
                set
                {
                    InnerRequest.CachePolicy = value;
                }
            }

            public override string ConnectionGroupName
            {
                get
                {
                    return InnerRequest.ConnectionGroupName;
                }
                set
                {
                    InnerRequest.ConnectionGroupName = value;
                }
            }

            public override long ContentLength
            {
                get
                {
                    return InnerRequest.ContentLength;
                }
                set
                {
                    InnerRequest.ContentLength = value;
                }
            }

            public override string ContentType
            {
                get
                {
                    return InnerRequest.ContentType;
                }
                set
                {
                    InnerRequest.ContentType = value;
                }
            }

            public override ICredentials Credentials
            {
                get
                {
                    return InnerRequest.Credentials;
                }
                set
                {
                    InnerRequest.Credentials = value;
                }
            }

            public override System.IO.Stream EndGetRequestStream(IAsyncResult asyncResult)
            {
                return InnerRequest.EndGetRequestStream(asyncResult);
            }

            public override WebResponse EndGetResponse(IAsyncResult asyncResult)
            {
                return InnerRequest.EndGetResponse(asyncResult);
            }

            // can't call protected method on InnerRequest.  This shouldn't
            // cause any problems unless the request is serialized.
            //protected override void GetObjectData(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
            //{
            //    InnerRequest.GetObjectData(serializationInfo, streamingContext);
            //}

            public override System.IO.Stream GetRequestStream()
            {
                return InnerRequest.GetRequestStream();
            }

            public override WebResponse GetResponse()
            {
                return InnerRequest.GetResponse();
            }

            public override WebHeaderCollection Headers
            {
                get
                {
                    return InnerRequest.Headers;
                }
                set
                {
                    InnerRequest.Headers = value;
                }
            }

            public override string Method
            {
                get
                {
                    return InnerRequest.Method;
                }
                set
                {
                    InnerRequest.Method = value;
                }
            }

            public override bool PreAuthenticate
            {
                get
                {
                    return InnerRequest.PreAuthenticate;
                }
                set
                {
                    InnerRequest.PreAuthenticate = value;
                }
            }

            public override IWebProxy Proxy
            {
                get
                {
                    return InnerRequest.Proxy;
                }
                set
                {
                    InnerRequest.Proxy = value;
                }
            }

            public override Uri RequestUri
            {
                get
                {
                    return InnerRequest.RequestUri;
                }
            }

            public override int Timeout
            {
                get
                {
                    return InnerRequest.Timeout;
                }
                set
                {
                    InnerRequest.Timeout = value;
                }
            }

            public override bool UseDefaultCredentials
            {
                get
                {
                    return InnerRequest.UseDefaultCredentials;
                }
                set
                {
                    InnerRequest.UseDefaultCredentials = value;
                }
            }
        }
    }
}
