//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tivo.Has.AddInSideAdapters
{
    
    [System.AddIn.Pipeline.AddInAdapterAttribute()]
    public class IHmeConnectionViewToContractAddInAdapter : System.AddIn.Pipeline.ContractBase, Tivo.Has.Contracts.IHmeConnectionContract
    {
        private Tivo.Has.IHmeConnection _view;
        public IHmeConnectionViewToContractAddInAdapter(Tivo.Has.IHmeConnection view)
        {
            _view = view;
        }
        public System.Threading.WaitHandle CommandReceived
        {
            get
            {
                return _view.CommandReceived;
            }
        }
        public System.Threading.WaitHandle EventReceived
        {
            get
            {
                return _view.EventReceived;
            }
        }
        internal Tivo.Has.IHmeConnection GetSourceView()
        {
            return _view;
        }
    }
}

