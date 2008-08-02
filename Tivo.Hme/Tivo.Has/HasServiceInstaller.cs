using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Install;
using System.ServiceProcess;
using System.ComponentModel;



namespace Tivo.Has
{
    [RunInstaller(true)]
    public class HasServiceInstaller : Installer
    {
        private ServiceInstaller _serviceInstaller = new ServiceInstaller();
        private ServiceProcessInstaller _processInstaller = new ServiceProcessInstaller();

        public HasServiceInstaller()
        {
            _processInstaller.Account = ServiceAccount.LocalService;
            _serviceInstaller.StartType = ServiceStartMode.Automatic;
            _serviceInstaller.ServiceName = "HME Application Service";

            Installers.Add(_serviceInstaller);
            Installers.Add(_processInstaller);
        }
    }
}
