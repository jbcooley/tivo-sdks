using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Threading;

namespace Tivo.Has
{
    public partial class HmeApplicationService : ServiceBase
    {
        //private ServiceHost _configurationServiceHost = new ServiceHost(typeof(Configuration.HasConfigurationService));
        private HmeServersController _controller = new HmeServersController();

        public HmeApplicationService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ThreadPool.QueueUserWorkItem(StartApplications);
            // this opens the configuration host
            // with a thread to respond to requests
//            _configurationServiceHost.Open();
        }

        protected override void OnStop()
        {
            lock (_controller)
            {
                _controller.StopAllServers();
                _controller.Clear();
            }
//            _configurationServiceHost.Close();
        }

        public void StartApplications(object unused)
        {
            if (Properties.Settings.Default.ApplicationDirectories == null)
                Properties.Settings.Default.ApplicationDirectories = new System.Collections.Specialized.StringCollection();

            string[] applicationDirectories = Properties.Settings.Default.ApplicationDirectories.Cast<string>().ToArray();
            lock (_controller)
            {
                _controller.LoadApplications(server => server.Start(), applicationDirectories);
            }
        }
    }
}
