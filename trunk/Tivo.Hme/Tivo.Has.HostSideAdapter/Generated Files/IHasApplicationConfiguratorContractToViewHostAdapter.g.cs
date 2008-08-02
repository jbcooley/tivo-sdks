//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3031
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tivo.Has.HostSideAdapters
{
    
    public class IHasApplicationConfiguratorContractToViewHostAdapter : Tivo.Has.IHasApplicationConfigurator
    {
        private Tivo.Has.Contracts.IHasApplicationConfiguratorContract _contract;
        private System.AddIn.Pipeline.ContractHandle _handle;
        static IHasApplicationConfiguratorContractToViewHostAdapter()
        {
        }
        public IHasApplicationConfiguratorContractToViewHostAdapter(Tivo.Has.Contracts.IHasApplicationConfiguratorContract contract)
        {
            _contract = contract;
            _handle = new System.AddIn.Pipeline.ContractHandle(contract);
        }
        public string Name
        {
            get
            {
                return _contract.Name;
            }
            set
            {
                _contract.Name = value;
            }
        }
        public System.Collections.Generic.IList<Tivo.Has.HasApplicationConfiguration> ApplicationConfigurations
        {
            get
            {
                return System.AddIn.Pipeline.CollectionAdapters.ToIList<Tivo.Has.Contracts.HasApplicationConfiguration, Tivo.Has.HasApplicationConfiguration>(_contract.ApplicationConfigurations, Tivo.Has.HostSideAdapters.HasApplicationConfigurationHostAdapter.ContractToViewAdapter, Tivo.Has.HostSideAdapters.HasApplicationConfigurationHostAdapter.ViewToContractAdapter);
            }
        }
        public System.Collections.Generic.IList<string> GetAccessableAssemblies()
        {
            return System.AddIn.Pipeline.CollectionAdapters.ToIList<string>(_contract.GetAccessableAssemblies());
        }
        public System.Collections.Generic.IList<string> GetApplications(string assemblyPath)
        {
            return System.AddIn.Pipeline.CollectionAdapters.ToIList<string>(_contract.GetApplications(assemblyPath));
        }
        public void CommitChanges()
        {
            _contract.CommitChanges();
        }
        public void RollbackChanges()
        {
            _contract.RollbackChanges();
        }
        internal Tivo.Has.Contracts.IHasApplicationConfiguratorContract GetSourceContract()
        {
            return _contract;
        }
    }
}

