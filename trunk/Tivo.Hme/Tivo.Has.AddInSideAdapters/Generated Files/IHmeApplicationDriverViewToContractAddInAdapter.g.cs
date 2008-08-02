//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3031
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tivo.Has.AddInSideAdapters
{
    
    [System.AddIn.Pipeline.AddInAdapterAttribute()]
    public class IHmeApplicationDriverViewToContractAddInAdapter : System.AddIn.Pipeline.ContractBase, Tivo.Has.Contracts.IHmeApplicationDriverContract
    {
        private Tivo.Has.IHmeApplicationDriver _view;
        private System.Collections.Generic.Dictionary<Tivo.Has.Contracts.IApplicationEndedEventHandler, System.EventHandler<Tivo.Has.ApplicationEndedEventArgs>> ApplicationEnded_handlers;
        public IHmeApplicationDriverViewToContractAddInAdapter(Tivo.Has.IHmeApplicationDriver view)
        {
            _view = view;
            ApplicationEnded_handlers = new System.Collections.Generic.Dictionary<Tivo.Has.Contracts.IApplicationEndedEventHandler, System.EventHandler<Tivo.Has.ApplicationEndedEventArgs>>();
        }
        public System.AddIn.Contract.IListContract<Tivo.Has.Contracts.IHmeApplicationIdentityContract> ApplicationIdentities
        {
            get
            {
                return System.AddIn.Pipeline.CollectionAdapters.ToIListContract<Tivo.Has.IHmeApplicationIdentity, Tivo.Has.Contracts.IHmeApplicationIdentityContract>(_view.ApplicationIdentities, Tivo.Has.AddInSideAdapters.IHmeApplicationIdentityAddInAdapter.ViewToContractAdapter, Tivo.Has.AddInSideAdapters.IHmeApplicationIdentityAddInAdapter.ContractToViewAdapter);
            }
        }
        public virtual Tivo.Has.Contracts.IHasApplicationConfiguratorContract GetApplicationConfiguration()
        {
            return Tivo.Has.AddInSideAdapters.IHasApplicationConfiguratorAddInAdapter.ViewToContractAdapter(_view.GetApplicationConfiguration());
        }
        public virtual Tivo.Has.Contracts.IHmeConnectionContract CreateHmeConnection(Tivo.Has.Contracts.IHmeApplicationIdentityContract identity, Tivo.Has.Contracts.IHmeStreamContract inputStream, Tivo.Has.Contracts.IHmeStreamContract outputStream)
        {
            return Tivo.Has.AddInSideAdapters.IHmeConnectionAddInAdapter.ViewToContractAdapter(_view.CreateHmeConnection(Tivo.Has.AddInSideAdapters.IHmeApplicationIdentityAddInAdapter.ContractToViewAdapter(identity), Tivo.Has.AddInSideAdapters.IHmeStreamAddInAdapter.ContractToViewAdapter(inputStream), Tivo.Has.AddInSideAdapters.IHmeStreamAddInAdapter.ContractToViewAdapter(outputStream)));
        }
        public virtual void HandleEventsAsync(Tivo.Has.Contracts.IHmeConnectionContract connection)
        {
            _view.HandleEventsAsync(Tivo.Has.AddInSideAdapters.IHmeConnectionAddInAdapter.ContractToViewAdapter(connection));
        }
        public virtual void RunOneAsync(Tivo.Has.Contracts.IHmeConnectionContract connection)
        {
            _view.RunOneAsync(Tivo.Has.AddInSideAdapters.IHmeConnectionAddInAdapter.ContractToViewAdapter(connection));
        }
        public virtual void ApplicationEndedEventAdd(Tivo.Has.Contracts.IApplicationEndedEventHandler handler)
        {
            System.EventHandler<Tivo.Has.ApplicationEndedEventArgs> adaptedHandler = new System.EventHandler<Tivo.Has.ApplicationEndedEventArgs>(new Tivo.Has.AddInSideAdapters.IApplicationEndedEventHandlerContractToViewAddInAdapter(handler).Handler);
            _view.ApplicationEnded += adaptedHandler;
            ApplicationEnded_handlers[handler] = adaptedHandler;
        }
        public virtual void ApplicationEndedEventRemove(Tivo.Has.Contracts.IApplicationEndedEventHandler handler)
        {
            System.EventHandler<Tivo.Has.ApplicationEndedEventArgs> adaptedHandler;
            if (ApplicationEnded_handlers.TryGetValue(handler, out adaptedHandler))
            {
                ApplicationEnded_handlers.Remove(handler);
                _view.ApplicationEnded -= adaptedHandler;
            }
        }
        internal Tivo.Has.IHmeApplicationDriver GetSourceView()
        {
            return _view;
        }
    }
}

