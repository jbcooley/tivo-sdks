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
    
    public class IHmeAsyncCallbackHostAdapter
    {
        internal static Tivo.Has.IHmeAsyncCallback ContractToViewAdapter(Tivo.Has.Contracts.IHmeAsyncCallbackContract contract)
        {
            if (((System.Runtime.Remoting.RemotingServices.IsObjectOutOfAppDomain(contract) != true) 
                        && contract.GetType().Equals(typeof(IHmeAsyncCallbackViewToContractHostAdapter))))
            {
                return ((IHmeAsyncCallbackViewToContractHostAdapter)(contract)).GetSourceView();
            }
            else
            {
                return new IHmeAsyncCallbackContractToViewHostAdapter(contract);
            }
        }
        internal static Tivo.Has.Contracts.IHmeAsyncCallbackContract ViewToContractAdapter(Tivo.Has.IHmeAsyncCallback view)
        {
            if (view.GetType().Equals(typeof(IHmeAsyncCallbackContractToViewHostAdapter)))
            {
                return ((IHmeAsyncCallbackContractToViewHostAdapter)(view)).GetSourceContract();
            }
            else
            {
                return new IHmeAsyncCallbackViewToContractHostAdapter(view);
            }
        }
    }
}

