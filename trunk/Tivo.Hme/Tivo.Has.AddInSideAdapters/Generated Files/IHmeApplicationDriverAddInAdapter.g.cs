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
    
    public class IHmeApplicationDriverAddInAdapter
    {
        internal static Tivo.Has.IHmeApplicationDriver ContractToViewAdapter(Tivo.Has.Contracts.IHmeApplicationDriverContract contract)
        {
            if (((System.Runtime.Remoting.RemotingServices.IsObjectOutOfAppDomain(contract) != true) 
                        && contract.GetType().Equals(typeof(IHmeApplicationDriverViewToContractAddInAdapter))))
            {
                return ((IHmeApplicationDriverViewToContractAddInAdapter)(contract)).GetSourceView();
            }
            else
            {
                return new IHmeApplicationDriverContractToViewAddInAdapter(contract);
            }
        }
        internal static Tivo.Has.Contracts.IHmeApplicationDriverContract ViewToContractAdapter(Tivo.Has.IHmeApplicationDriver view)
        {
            if (view.GetType().Equals(typeof(IHmeApplicationDriverContractToViewAddInAdapter)))
            {
                return ((IHmeApplicationDriverContractToViewAddInAdapter)(view)).GetSourceContract();
            }
            else
            {
                return new IHmeApplicationDriverViewToContractAddInAdapter(view);
            }
        }
    }
}

