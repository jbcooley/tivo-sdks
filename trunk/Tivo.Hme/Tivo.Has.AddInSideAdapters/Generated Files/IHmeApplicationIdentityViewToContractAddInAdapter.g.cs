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
    
    public class IHmeApplicationIdentityViewToContractAddInAdapter : System.AddIn.Pipeline.ContractBase, Tivo.Has.Contracts.IHmeApplicationIdentityContract
    {
        private Tivo.Has.IHmeApplicationIdentity _view;
        public IHmeApplicationIdentityViewToContractAddInAdapter(Tivo.Has.IHmeApplicationIdentity view)
        {
            _view = view;
        }
        public string Name
        {
            get
            {
                return _view.Name;
            }
        }
        public byte[] Icon
        {
            get
            {
                return _view.Icon;
            }
        }
        public System.Uri EndPoint
        {
            get
            {
                return _view.EndPoint;
            }
        }
        internal Tivo.Has.IHmeApplicationIdentity GetSourceView()
        {
            return _view;
        }
    }
}

