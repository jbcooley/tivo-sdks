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
    
    public class IHmeConnectionViewToContractAddInAdapter : System.AddIn.Pipeline.ContractBase, Tivo.Has.Contracts.IHmeConnectionContract
    {
        private Tivo.Has.IHmeConnection _view;
        public IHmeConnectionViewToContractAddInAdapter(Tivo.Has.IHmeConnection view)
        {
            _view = view;
        }
        public string CommandReceivedName
        {
            get
            {
                return _view.CommandReceivedName;
            }
        }
        public string EventReceivedName
        {
            get
            {
                return _view.EventReceivedName;
            }
        }
        internal Tivo.Has.IHmeConnection GetSourceView()
        {
            return _view;
        }
    }
}

