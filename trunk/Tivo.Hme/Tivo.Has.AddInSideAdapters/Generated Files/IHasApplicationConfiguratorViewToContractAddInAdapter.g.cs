//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tivo.Has.AddInSideAdapters
{
    
    public class IHasApplicationConfiguratorViewToContractAddInAdapter : System.AddIn.Pipeline.ContractBase, Tivo.Has.Contracts.IHasApplicationConfiguratorContract
    {
        private Tivo.Has.IHasApplicationConfigurator _view;
        public IHasApplicationConfiguratorViewToContractAddInAdapter(Tivo.Has.IHasApplicationConfigurator view)
        {
            _view = view;
        }
        public string Name
        {
            get
            {
                return _view.Name;
            }
            set
            {
                _view.Name = value;
            }
        }
        public System.AddIn.Contract.IListContract<Tivo.Has.Contracts.HasApplicationConfiguration> ApplicationConfigurations
        {
            get
            {
                return System.AddIn.Pipeline.CollectionAdapters.ToIListContract<Tivo.Has.HasApplicationConfiguration, Tivo.Has.Contracts.HasApplicationConfiguration>(_view.ApplicationConfigurations, Tivo.Has.AddInSideAdapters.HasApplicationConfigurationAddInAdapter.ViewToContractAdapter, Tivo.Has.AddInSideAdapters.HasApplicationConfigurationAddInAdapter.ContractToViewAdapter);
            }
        }
        public virtual System.AddIn.Contract.IListContract<string> GetAccessableAssemblies()
        {
            return System.AddIn.Pipeline.CollectionAdapters.ToIListContract<string>(_view.GetAccessableAssemblies());
        }
        public virtual System.AddIn.Contract.IListContract<string> GetApplications(string assemblyPath)
        {
            return System.AddIn.Pipeline.CollectionAdapters.ToIListContract<string>(_view.GetApplications(assemblyPath));
        }
        public virtual void CommitChanges()
        {
            _view.CommitChanges();
        }
        public virtual void RollbackChanges()
        {
            _view.RollbackChanges();
        }
        internal Tivo.Has.IHasApplicationConfigurator GetSourceView()
        {
            return _view;
        }
    }
}

