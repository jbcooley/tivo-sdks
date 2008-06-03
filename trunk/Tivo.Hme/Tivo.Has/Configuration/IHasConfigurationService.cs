using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Tivo.Has.Configuration
{
    // NOTE: If you change the interface name "IService1" here, you must also update the reference to "IService1" in App.config.
    [ServiceContract]
    public interface IHasConfigurationService
    {
        [OperationContract]
        string[] GetApplicationDirectories();
        [OperationContract]
        void AddApplicationDirectory(string directory);
        [OperationContract]
        void RemoveApplicationDirectory(string directory);
        [OperationContract]
        void StopAllApplications();
        [OperationContract]
        void StopApplication(string applicationName);
        [OperationContract]
        void StopApplications(string[] applicationNames);
        [OperationContract]
        void StartApplication(string applicationName);
        [OperationContract]
        void StartApplications(string[] applicationNames);
        [OperationContract]
        string[] GetApplicationNames(string directory);
    }
}
