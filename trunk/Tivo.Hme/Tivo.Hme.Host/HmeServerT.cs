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
    }
}
