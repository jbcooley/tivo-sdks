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
    
    public class IHmeApplicationIdentityContractToViewHostAdapter : Tivo.Has.IHmeApplicationIdentity
    {
        private Tivo.Has.Contracts.IHmeApplicationIdentityContract _contract;
        private System.AddIn.Pipeline.ContractHandle _handle;
        static IHmeApplicationIdentityContractToViewHostAdapter()
        {
        }
        public IHmeApplicationIdentityContractToViewHostAdapter(Tivo.Has.Contracts.IHmeApplicationIdentityContract contract)
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
        }
        public byte[] Icon
        {
            get
            {
                return _contract.Icon;
            }
        }
        public System.Uri EndPoint
        {
            get
            {
                return _contract.EndPoint;
            }
        }
        internal Tivo.Has.Contracts.IHmeApplicationIdentityContract GetSourceContract()
        {
            return _contract;
        }
    }
}

