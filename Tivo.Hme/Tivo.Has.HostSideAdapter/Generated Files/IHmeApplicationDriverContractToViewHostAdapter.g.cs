//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tivo.Has.HostSideAdapters
{
    
    [System.AddIn.Pipeline.HostAdapterAttribute()]
    public class IHmeApplicationDriverContractToViewHostAdapter : Tivo.Has.IHmeApplicationDriver
    {
        private Tivo.Has.Contracts.IHmeApplicationDriverContract _contract;
        private System.AddIn.Pipeline.ContractHandle _handle;
        private Tivo.Has.HostSideAdapters.IApplicationEndedEventHandlerViewToContractHostAdapter ApplicationEnded_Handler;
        private static System.Reflection.MethodInfo s_ApplicationEndedEventAddFire;
		public event System.EventHandler<Tivo.Has.ApplicationEndedEventArgs>ApplicationEnded{
			add{
				if (_ApplicationEnded == null)
				{
					_contract.ApplicationEndedEventAdd(ApplicationEnded_Handler);
				}
				_ApplicationEnded += value;
				}
			remove{
					_ApplicationEnded -= value;
				if (_ApplicationEnded == null)
				{
					_contract.ApplicationEndedEventRemove(ApplicationEnded_Handler);
				}
				}
		}
        static IHmeApplicationDriverContractToViewHostAdapter()
        {
            s_ApplicationEndedEventAddFire = typeof(IHmeApplicationDriverContractToViewHostAdapter).GetMethod("Fire_ApplicationEnded", ((System.Reflection.BindingFlags)(36)));
        }
        public IHmeApplicationDriverContractToViewHostAdapter(Tivo.Has.Contracts.IHmeApplicationDriverContract contract)
        {
            _contract = contract;
            _handle = new System.AddIn.Pipeline.ContractHandle(contract);
            ApplicationEnded_Handler = new Tivo.Has.HostSideAdapters.IApplicationEndedEventHandlerViewToContractHostAdapter(this, s_ApplicationEndedEventAddFire);
        }
        private event System.EventHandler<Tivo.Has.ApplicationEndedEventArgs> _ApplicationEnded;
        public IHmeConnection CreateHmeConnection(IHmeApplicationIdentity identity, System.IO.Stream inputStream, System.IO.Stream outputStream)
        {
            return Tivo.Has.HostSideAdapters.IHmeConnectionHostAdapter.ContractToViewAdapter(_contract.CreateHmeConnection(Tivo.Has.HostSideAdapters.IHmeApplicationIdentityHostAdapter.ViewToContractAdapter(identity), inputStream, outputStream));
        }
        public void HandleEventsAsync(IHmeConnection connection)
        {
            _contract.HandleEventsAsync(Tivo.Has.HostSideAdapters.IHmeConnectionHostAdapter.ViewToContractAdapter(connection));
        }
        public void RunOneAsync(IHmeConnection connection)
        {
            _contract.RunOneAsync(Tivo.Has.HostSideAdapters.IHmeConnectionHostAdapter.ViewToContractAdapter(connection));
        }
        internal virtual void Fire_ApplicationEnded(Tivo.Has.ApplicationEndedEventArgs args)
        {
            if ((_ApplicationEnded == null))
            {
            }
            else
            {
                _ApplicationEnded.Invoke(this, args);
            }
        }
        internal Tivo.Has.Contracts.IHmeApplicationDriverContract GetSourceContract()
        {
            return _contract;
        }
    }
}

