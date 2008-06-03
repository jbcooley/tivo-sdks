using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Tivo.Has.Configuration
{
    public class HasConfigurationService : IHasConfigurationService
    {
        private HmeServersController _controller;

        /// <summary>
        /// Not used except for Visual Studio integration support
        /// </summary>
        public HasConfigurationService()
        {
            _controller = new HmeServersController();
        }

        internal HasConfigurationService(HmeServersController controller)
        {
            _controller = controller;
        }

        #region IHasConfigurationService Members

        public string[] GetApplicationDirectories()
        {
            return Properties.Settings.Default.ApplicationDirectories.Cast<string>().ToArray();
        }

        public void AddApplicationDirectory(string directory)
        {
            _controller.LoadApplications(directory);
            Properties.Settings.Default.ApplicationDirectories.Add(directory);
            Properties.Settings.Default.Save();
        }

        public void RemoveApplicationDirectory(string directory)
        {
            var servers = from s in _controller.Servers
                          where s.ServerPath == directory
                          select s.HmeServer;

            foreach (var server in servers)
            {
                server.Stop();
            }
            _controller.Remove(server => server.ServerPath == directory);

            Properties.Settings.Default.ApplicationDirectories.Remove(directory);
            Properties.Settings.Default.Save();
        }

        public void StopAllApplications()
        {
            _controller.StopAllServers();
        }

        public void StopApplication(string applicationName)
        {
            StopApplications(new string[] { applicationName });
        }

        public void StopApplications(string[] applicationNames)
        {
            var servers = from s in _controller.Servers
                          where applicationNames.Contains(s.Identity.Name)
                          select s.HmeServer;
            foreach (var server in servers)
            {
                server.Stop();
            }
        }

        public void StartApplication(string applicationName)
        {
            StartApplications(new string[] { applicationName });
        }

        public void StartApplications(string[] applicationNames)
        {
            var servers = from s in _controller.Servers
                          where applicationNames.Contains(s.Identity.Name)
                          select s.HmeServer;
            foreach (var server in servers)
            {
                server.Start();
            }
        }

        public string[] GetApplicationNames(string directory)
        {
            return (from s in _controller.Servers
                    where s.ServerPath == directory
                    select s.Identity.Name).ToArray();
        }

        #endregion
    }
}
