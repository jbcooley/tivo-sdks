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
    
    public class HasApplicationConfigurationArrayAddInAdapter
    {
        public static Tivo.Has.HasApplicationConfiguration[] ContractToViewAdapter(Tivo.Has.Contracts.HasApplicationConfiguration[] contract)
        {
            Tivo.Has.HasApplicationConfiguration[] result = new Tivo.Has.HasApplicationConfiguration[contract.Length];
            for (int i = 0; (i < contract.Length); i = (i + 1))
            {
                result[i] = Tivo.Has.AddInSideAdapters.HasApplicationConfigurationAddInAdapter.ContractToViewAdapter(contract[i]);
            }
            return result;
        }
        public static Tivo.Has.Contracts.HasApplicationConfiguration[] ViewToContractAdapter(Tivo.Has.HasApplicationConfiguration[] view)
        {
            Tivo.Has.Contracts.HasApplicationConfiguration[] result = new Tivo.Has.Contracts.HasApplicationConfiguration[view.Length];
            for (int i = 0; (i < view.Length); i = (i + 1))
            {
                result[i] = Tivo.Has.AddInSideAdapters.HasApplicationConfigurationAddInAdapter.ViewToContractAdapter(view[i]);
            }
            return result;
        }
    }
}
