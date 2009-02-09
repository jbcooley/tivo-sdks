using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Tivo.Has
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            //ServiceBase[] ServicesToRun;
            //ServicesToRun = new ServiceBase[] 
            //{ 
            //    new HmeApplicationService() 
            //};
            //ServiceBase.Run(ServicesToRun);
            var service = new HmeApplicationService();
            service.StartApplications(null);
            Console.ReadLine();
        }
    }
}
