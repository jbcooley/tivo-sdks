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
    
    public class HasApplicationConfigurationAddInAdapter
    {
        public static Tivo.Has.HasApplicationConfiguration ContractToViewAdapter(Tivo.Has.Contracts.HasApplicationConfiguration contract)
        {
            return new Tivo.Has.HasApplicationConfiguration(contract.Name, contract.EndPoint, contract.AssemblyQualifiedTypeName);
        }
        public static Tivo.Has.Contracts.HasApplicationConfiguration ViewToContractAdapter(Tivo.Has.HasApplicationConfiguration view)
        {
            return new Tivo.Has.Contracts.HasApplicationConfiguration(view.Name, view.EndPoint, view.AssemblyQualifiedTypeName);
        }
    }
}
