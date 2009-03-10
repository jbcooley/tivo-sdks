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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using System.Web;
using System.IO;
using System.Collections.Specialized;
using System.Runtime.Remoting.Lifetime;

namespace Tivo.Hme.Host.Services
{
    class SimpleAspNetHost : MarshalByRefObject
    {
        /// <summary>
        /// We will use this field to check if the object has been destroyed
        /// by the timeout.
        /// </summary>
        public bool StillAlive = true;
        /// <summary>
        /// The timeout for the ASP.Net runtime after which it is automatically unloaded when idle
        /// to release resources. Note this can't be externally set because the lease is set
        /// during object construction. All you can do is change this property value here statically
        /// </summary>
        public static int IdleTimeoutMinutes = 15;
        /// <summary>
        /// the Response status code the server sent. 200 on success, 500 on error, 404 for redirect etc.
        /// </summary>
        public int ResponseStatusCode = 200;
        /// <summary>
        /// An error message if bError is set. Only works for the ProcessRequest method
        /// </summary>
        public string ErrorMessage = "";
        public bool Error = false;

        public void ProcessRequest(HttpRequestData requestData, HttpResponseWrapper response)
        {
            try
            {
                var wr = new HmeHostWorkerRequest(requestData, response);
                HttpRuntime.ProcessRequest(wr);
            }
            catch (IOException)
            {
                // ignore cases where the client closes the connection
            }
            catch (Exception ex)
            {
                ResponseStatusCode = 500;
                ErrorMessage = ex.Message;
                Error = true;
            }
        }

        /// <summary>
        /// Overrides the default Lease setting to allow the runtime to not
        /// expire after 5 minutes. 
        /// </summary>
        /// <returns></returns>
        public override object InitializeLifetimeService()
        {
            // return null; // never expire

            ILease lease = (ILease)base.InitializeLifetimeService();

            // *** Set the initial lease which determines how long the remote ref sticks around
            // *** before .Net automatically releases it. Although our code has the logic to
            // *** to automatically restart it's better to keep it loaded
            if (lease.CurrentState == LeaseState.Initial)
            {
                lease.InitialLeaseTime = TimeSpan.FromMinutes(SimpleAspNetHost.IdleTimeoutMinutes);
                lease.RenewOnCallTime = TimeSpan.FromMinutes(SimpleAspNetHost.IdleTimeoutMinutes);
                lease.SponsorshipTimeout = TimeSpan.FromMinutes(5);
            }

            return lease;
        }
    }
}
