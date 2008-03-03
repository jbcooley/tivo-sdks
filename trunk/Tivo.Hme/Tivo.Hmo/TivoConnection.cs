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

namespace Tivo.Hmo
{
    public class TivoConnection : IDisposable
    {
        private string _hmoServer;
        private string _mediaAccessKey;

        public TivoConnection(string hmoServer, string mediaAccessKey)
        {
            _hmoServer = hmoServer;
            _mediaAccessKey = mediaAccessKey;
            // optional -- 4.4.2 QueryServer
        }

        private bool _disposed;

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

        public IAsyncResult BeginQueryContainer(string container, bool recurse, AsyncCallback asyncCallback, object asyncState)
        {
            WebClient client;
            Uri uri;
            PrepareQueryContainer(container, recurse, out client, out uri);
            client.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);
            WebClientAsyncResult asyncResult = new WebClientAsyncResult(asyncCallback, asyncState);
            ServicePointManager.ServerCertificateValidationCallback = TrustAllCertificatePolicy.TrustAllCertificateCallback;
            client.OpenReadAsync(uri, asyncResult);
            return asyncResult;
        }

        void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            // return object
            WebClientAsyncResult asyncResult = (WebClientAsyncResult)e.UserState;
            XmlSerializer serializer = new XmlSerializer(typeof(TiVoContainer));
            asyncResult.Result = (TiVoContainer)serializer.Deserialize(e.Result);
            asyncResult.AsyncCallback(asyncResult);
            // TODO: what to do with errors and cancelled?
        }

        public TiVoContainer EndQueryContainer(IAsyncResult asyncResult)
        {
            WebClientAsyncResult webClientAsyncResult = asyncResult as WebClientAsyncResult;
            if (webClientAsyncResult == null)
                throw new ArgumentException("IAsyncResult did not come from BeginQueryContainer", "asyncResult");
            if (webClientAsyncResult.Error != null)
                throw webClientAsyncResult.Error;
            return (TiVoContainer)webClientAsyncResult.Result;
        }

        public TiVoContainer QueryContainer(string container, bool recurse)
            // sortorder, randomseed, randomstart, itemcount, anchoritem, anchoroffset,
            // filter - mime type filter
        {
            WebClient client;
            Uri uri;
            PrepareQueryContainer(container, recurse, out client, out uri);
            XmlSerializer serializer = new XmlSerializer(typeof(TiVoContainer));

            ServicePointManager.ServerCertificateValidationCallback = TrustAllCertificatePolicy.TrustAllCertificateCallback;
            return (TiVoContainer)serializer.Deserialize(client.OpenRead(uri));
        }

        private void PrepareQueryContainer(string container, bool recurse, out WebClient client, out Uri uri)
        {
            client = new WebClient();
            //ServicePointManager.ServerCertificateValidationCallback = TrustAllCertificatePolicy.TrustAllCertificateCallback;
            client.Credentials = new NetworkCredential("tivo", _mediaAccessKey);
            client.QueryString.Add("Command", "QueryContainer");
            client.QueryString.Add("Container", container);
            client.QueryString.Add("Recurse", recurse ? "Yes" : "No");
            uri = new Uri("https://" + _hmoServer + "/TiVoConnect");
        }

        public void QueryItem()
        {
        }

        public void QueryFormats()
        {
        }

        public void RequestDocument()
        {
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
