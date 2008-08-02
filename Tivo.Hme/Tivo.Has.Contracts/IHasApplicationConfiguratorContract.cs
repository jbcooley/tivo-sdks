using System;
using System.Collections.Generic;
using System.Text;
using System.AddIn.Contract;

namespace Tivo.Has.Contracts
{
    public interface IHasApplicationConfiguratorContract : IContract
    {
        string Name { get; set; }
        // list accessable assemblies - this is base directory and all directories in relative search path
        IListContract<string> GetAccessableAssemblies();
        // list applications in an assembly - return assembly qualified type name
        IListContract<string> GetApplications(string assemblyPath);
        // allow adding application identities (both free form key in and find and select)
        //void AddApplication(string name, Uri endPoint, string assemblyQualifiedTypeName);
        // a list api works better for adding and removing applications as well as listing existing applications
        IListContract<HasApplicationConfiguration> ApplicationConfigurations { get; }
        void CommitChanges();
        void RollbackChanges();
        // TODO: do we need start and stop control here or just in the main app?
    }

    [Serializable]
    public struct HasApplicationConfiguration
    {
        public HasApplicationConfiguration(string name, Uri endPoint, string assemblyQualifiedTypeName)
            : this()
        {
            Name = name;
            EndPoint = endPoint;
            AssemblyQualifiedTypeName = assemblyQualifiedTypeName;
        }
        public string Name { get; set; }
        public Uri EndPoint { get; set; }
        public string AssemblyQualifiedTypeName { get; set; }
    }
}
