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
    
    public class IApplicationEndedEventHandlerContractToViewAddInAdapter
    {
        private Tivo.Has.Contracts.IApplicationEndedEventHandler _contract;
        private System.AddIn.Pipeline.ContractHandle _handle;
        public IApplicationEndedEventHandlerContractToViewAddInAdapter(Tivo.Has.Contracts.IApplicationEndedEventHandler contract)
        {
            _contract = contract;
            _handle = new System.AddIn.Pipeline.ContractHandle(contract);
        }
        public void Handler(object sender, Tivo.Has.ApplicationEndedEventArgs args)
        {
            _contract.Handler(Tivo.Has.AddInSideAdapters.ApplicationEndedEventArgsAddInAdapter.ViewToContractAdapter(args));
        }
        internal Tivo.Has.Contracts.IApplicationEndedEventHandler GetSourceContract()
        {
            return _contract;
        }
    }
}
