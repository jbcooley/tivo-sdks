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
    
    public class IHmeConnectionAddInAdapter
    {
        internal static Tivo.Has.IHmeConnection ContractToViewAdapter(Tivo.Has.Contracts.IHmeConnectionContract contract)
        {
            if (((System.Runtime.Remoting.RemotingServices.IsObjectOutOfAppDomain(contract) != true) 
                        && contract.GetType().Equals(typeof(IHmeConnectionViewToContractAddInAdapter))))
            {
                return ((IHmeConnectionViewToContractAddInAdapter)(contract)).GetSourceView();
            }
            else
            {
                return new IHmeConnectionContractToViewAddInAdapter(contract);
            }
        }
        internal static Tivo.Has.Contracts.IHmeConnectionContract ViewToContractAdapter(Tivo.Has.IHmeConnection view)
        {
            if (view.GetType().Equals(typeof(IHmeConnectionContractToViewAddInAdapter)))
            {
                return ((IHmeConnectionContractToViewAddInAdapter)(view)).GetSourceContract();
            }
            else
            {
                return new IHmeConnectionViewToContractAddInAdapter(view);
            }
        }
    }
}

