//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tivo.Has.HostSideAdapters
{
    
    public class IApplicationEndedEventHandlerViewToContractHostAdapter : System.AddIn.Pipeline.ContractBase, Tivo.Has.Contracts.IApplicationEndedEventHandler
    {
        private object _view;
        private System.Reflection.MethodInfo _event;
        public IApplicationEndedEventHandlerViewToContractHostAdapter(object view, System.Reflection.MethodInfo eventProp)
        {
            _view = view;
            _event = eventProp;
        }
        public void Handler(Tivo.Has.Contracts.IApplicationEndedEventArgs args)
        {
            Tivo.Has.HostSideAdapters.ApplicationEndedEventArgsContractToViewHostAdapter adaptedArgs;
            adaptedArgs = new Tivo.Has.HostSideAdapters.ApplicationEndedEventArgsContractToViewHostAdapter(args);
            object[] argsArray = new object[1];
            argsArray[0] = adaptedArgs;
            _event.Invoke(_view, argsArray);
        }
        internal object GetSourceView()
        {
            return _view;
        }
    }
}

