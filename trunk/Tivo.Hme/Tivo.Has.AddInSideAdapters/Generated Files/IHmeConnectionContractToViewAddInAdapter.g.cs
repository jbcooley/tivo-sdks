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
    
    public class IHmeConnectionContractToViewAddInAdapter : Tivo.Has.IHmeConnection
    {
        private Tivo.Has.Contracts.IHmeConnectionContract _contract;
        private System.AddIn.Pipeline.ContractHandle _handle;
        static IHmeConnectionContractToViewAddInAdapter()
        {
        }
        public IHmeConnectionContractToViewAddInAdapter(Tivo.Has.Contracts.IHmeConnectionContract contract)
        {
            _contract = contract;
            _handle = new System.AddIn.Pipeline.ContractHandle(contract);
        }
        public string CommandReceivedName
        {
            get
            {
                return _contract.CommandReceivedName;
            }
        }
        public string EventReceivedName
        {
            get
            {
                return _contract.EventReceivedName;
            }
        }
        internal Tivo.Has.Contracts.IHmeConnectionContract GetSourceContract()
        {
            return _contract;
        }
    }
}

