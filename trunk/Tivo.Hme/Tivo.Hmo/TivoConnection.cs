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
            _webClient = new WebClient();
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
    }
}
