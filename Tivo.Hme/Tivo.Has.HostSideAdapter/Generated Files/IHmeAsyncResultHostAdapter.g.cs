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
    
    public class IHmeAsyncResultHostAdapter
    {
        internal static Tivo.Has.IHmeAsyncResult ContractToViewAdapter(Tivo.Has.Contracts.IHmeAsyncResultContract contract)
        {
            if (((System.Runtime.Remoting.RemotingServices.IsObjectOutOfAppDomain(contract) != true) 
                        && contract.GetType().Equals(typeof(IHmeAsyncResultViewToContractHostAdapter))))
            {
                return ((IHmeAsyncResultViewToContractHostAdapter)(contract)).GetSourceView();
            }
            else
            {
                return new IHmeAsyncResultContractToViewHostAdapter(contract);
            }
        }
        internal static Tivo.Has.Contracts.IHmeAsyncResultContract ViewToContractAdapter(Tivo.Has.IHmeAsyncResult view)
        {
            if (view.GetType().Equals(typeof(IHmeAsyncResultContractToViewHostAdapter)))
            {
                return ((IHmeAsyncResultContractToViewHostAdapter)(view)).GetSourceContract();
            }
            else
            {
                return new IHmeAsyncResultViewToContractHostAdapter(view);
            }
        }
    }
}

