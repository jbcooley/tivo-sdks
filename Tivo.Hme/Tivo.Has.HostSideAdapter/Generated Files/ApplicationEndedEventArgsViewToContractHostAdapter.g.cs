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
    
    public class ApplicationEndedEventArgsViewToContractHostAdapter : System.AddIn.Pipeline.ContractBase, Tivo.Has.Contracts.IApplicationEndedEventArgs
    {
        private Tivo.Has.ApplicationEndedEventArgs _view;
        public ApplicationEndedEventArgsViewToContractHostAdapter(Tivo.Has.ApplicationEndedEventArgs view)
        {
            _view = view;
        }
        internal Tivo.Has.ApplicationEndedEventArgs GetSourceView()
        {
            return _view;
        }
    }
}

