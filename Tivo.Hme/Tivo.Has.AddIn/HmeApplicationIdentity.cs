using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tivo.Hme;
using Tivo.Hme.Host;
using Tivo.Hme.Host.Services;

namespace Tivo.Has.AddIn
{
    [Serializable]
    class HmeApplicationIdentity : IHmeApplicationIdentity
    {
        private Type _hmeApplicationHandler;
        private Dictionary<Application, HmeApplicationHandler> _applications = new Dictionary<Application, HmeApplicationHandler>();
        private System.ComponentModel.Design.ServiceContainer _services = new System.ComponentModel.Design.ServiceContainer();

        public HmeApplicationIdentity(int index)
        {
            _hmeApplicationHandler = Type.GetType(Properties.Settings.Default.ApplicationType[index]);
            Name = Properties.Settings.Default.ApplicationName[index];
            EndPoint = new Uri(Properties.Settings.Default.EndPoint[index]);
            object[] attributes = _hmeApplicationHandler.GetCustomAttributes(typeof(ApplicationIconAttribute), true);
            if (attributes.Length != 0)
            {
                Icon = ((ApplicationIconAttribute)attributes[0]).Icon;
            }
            attributes = _hmeApplicationHandler.GetCustomAttributes(typeof(UsesHostHttpServicesAttribute), true);
            UsesHostHttpServices = attributes.Length != 0 && attributes[0] is UsesHostHttpServicesAttribute;
            // TODO: else set Icon = default icon
            // TODO: allow WebPath to be configured
            WebPath = AppDomain.CurrentDomain.BaseDirectory;
        }

        public HmeApplicationHandler CreateApplication(HmeConnection connection, string baseUri)
        {
            HmeApplicationHandler application = (HmeApplicationHandler)Activator.CreateInstance(_hmeApplicationHandler);
            // store copy associated to application so it can be used in closed event
            _applications.Add(connection.Application, application);
            connection.Application.Closed += new EventHandler<EventArgs>(Application_Closed);

            application.BaseUri = new Uri(baseUri);
            // start the application
            HmeApplicationStartArgs startArgs = new HmeApplicationStartArgs();
            startArgs.Application = connection.Application;
            startArgs.HostServices = _services;
            application.OnApplicationStart(startArgs);
            return application;
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

        #region IHmeApplicationIdentity Members

        public string Name { get; private set; }

        public byte[] Icon { get; private set; }

        public Uri EndPoint { get; private set; }

        public bool UsesHostHttpServices { get; private set; }

        #endregion

        public string WebPath { get; private set; }
    }
}
