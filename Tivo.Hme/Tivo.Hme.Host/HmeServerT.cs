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
using System.Diagnostics;
using Tivo.Hme.Host.Services;

namespace Tivo.Hme.Host
{
    public class HmeServer<ApplicationT> : HmeServer where ApplicationT : HmeApplicationHandler, new()
    {
        private Dictionary<Application, ApplicationT> _applications = new Dictionary<Application, ApplicationT>();
        private bool _usesHostHttpServices;
        // use shared domain with this server
        private static string WebAppPath = AppDomain.CurrentDomain.BaseDirectory;

        public HmeServer(string name, Uri applicationPrefix, HmeServerOptions options)
            : base(name, applicationPrefix, options)
        {
            InitializeServices();
        }

        public HmeServer(string name, Uri applicationPrefix)
            : base(name, applicationPrefix)
        {
            InitializeServices();
        }

        private void InitializeServices()
        {
            object[] attributes = typeof(ApplicationT).GetCustomAttributes(typeof(UsesHostHttpServicesAttribute), true);
            _usesHostHttpServices = attributes.Length != 0 && attributes[0] is UsesHostHttpServicesAttribute;
        }

        protected override void OnApplicationConnected(HmeApplicationConnectedEventArgs args)
        {
            ApplicationT applicationT = new ApplicationT();
            // store copy associated to application so it can be used in closed event
            _applications.Add(args.Application, applicationT);
            args.Application.Closed += new EventHandler<EventArgs>(Application_Closed);
            // set the base uri for the application
            applicationT.BaseUri = args.BaseUri;
            // start the application
            HmeApplicationStartArgs startArgs = new HmeApplicationStartArgs();
            startArgs.Application = args.Application;
            startArgs.HostServices = this;
            applicationT.OnApplicationStart(startArgs);

            base.OnApplicationConnected(args);
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

        protected override void OnNonApplicationRequestReceived(HttpConnectionEventArgs e)
        {
            ServerLog.Write(TraceEventType.Verbose, "Enter HmeServer<T>.OnNonApplicationRequestReceived");
            if (_usesHostHttpServices)
            {
                var host = ((IHttpApplicationHostPool)GetService(typeof(IHttpApplicationHostPool))).GetHost(WebAppPath);
                host.ProcessRequest(ApplicationPrefix, e.Context);
            }
            else
            {
                base.OnNonApplicationRequestReceived(e);
            }
            ServerLog.Write(TraceEventType.Verbose, "Exit HmeServer<T>.OnNonApplicationRequestReceived");
        }

        protected override void OnHmeApplicationIconRequested(HmeApplicationIconRequestedArgs e)
        {
            object[] attributes = typeof(ApplicationT).GetCustomAttributes(typeof(ApplicationIconAttribute), true);
            if (attributes.Length != 0)
            {
                e.Icon = ((ApplicationIconAttribute)attributes[0]).Icon;
                e.ContentType = "image/png";
            }
            base.OnHmeApplicationIconRequested(e);
        }
    }
}
