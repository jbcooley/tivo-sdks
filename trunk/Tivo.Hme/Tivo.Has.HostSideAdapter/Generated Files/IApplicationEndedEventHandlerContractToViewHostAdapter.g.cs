//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tivo.Has.HostSideAdapters
{
    
    public class IApplicationEndedEventHandlerContractToViewHostAdapter
    {
        private Tivo.Has.Contracts.IApplicationEndedEventHandler _contract;
        private System.AddIn.Pipeline.ContractHandle _handle;
        public IApplicationEndedEventHandlerContractToViewHostAdapter(Tivo.Has.Contracts.IApplicationEndedEventHandler contract)
        {
            _contract = contract;
            _handle = new System.AddIn.Pipeline.ContractHandle(contract);
        }
        public void Handler(object sender, Tivo.Has.ApplicationEndedEventArgs args)
        {
            _contract.Handler(Tivo.Has.HostSideAdapters.ApplicationEndedEventArgsHostAdapter.ViewToContractAdapter(args));
        }
        internal Tivo.Has.Contracts.IApplicationEndedEventHandler GetSourceContract()
        {
            return _contract;
        }
    }
}

