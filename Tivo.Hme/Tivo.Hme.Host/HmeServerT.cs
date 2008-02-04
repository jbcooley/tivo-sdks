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

namespace Tivo.Hme.Host
{
    public class HmeServer<ApplicationT> : HmeServer where ApplicationT : HmeApplicationHandler, new()
    {
        private Dictionary<Application, ApplicationT> _applications = new Dictionary<Application, ApplicationT>();

        public HmeServer(string name, Uri applicationPrefix, HmeServerOptions options)
            : base(name, applicationPrefix, options)
        {
            ApplicationConnected += new EventHandler<HmeApplicationConnectedEventArgs>(HmeServer_ApplicationConnected);
        }

        public HmeServer(string name, Uri applicationPrefix)
            : base(name, applicationPrefix)
        {
            ApplicationConnected += new EventHandler<HmeApplicationConnectedEventArgs>(HmeServer_ApplicationConnected);
        }

        void HmeServer_ApplicationConnected(object sender, HmeApplicationConnectedEventArgs e)
        {
            ApplicationT applicationT = new ApplicationT();
            // store copy associated to application so it can be used in closed event
            _applications.Add(e.Application, applicationT);
            e.Application.Closed += new EventHandler<EventArgs>(Application_Closed);
            // set the base uri for the application
            applicationT.BaseUri = e.BaseUri;
            // start the application
            HmeApplicationStartArgs startArgs = new HmeApplicationStartArgs();
            startArgs.Application = e.Application;
            applicationT.OnApplicationStart(startArgs);
        }

        void Application_Closed(object sender, EventArgs e)
        {
            Application application = sender as Application;
            if (application != null)
            {
                _applications[application].OnApplicationEnd();
                _applications.Remove(application);
            }
        }

        protected override void NonApplicationRequestRecieved(NonApplicationRequestReceivedArgs e)
        {
            if (e.HttpRequest.RequestUri.OriginalString.EndsWith("/icon.png", StringComparison.OrdinalIgnoreCase))
            {
                object[] attributes = typeof(ApplicationT).GetCustomAttributes(typeof(ApplicationIconAttribute), true);
                if (attributes.Length != 0)
                {
                    e.HttpResponse = new ApplicationIconHttpResponse(((ApplicationIconAttribute)attributes[0]).Icon);
                }
            }
            base.NonApplicationRequestRecieved(e);
        }
    }
}
