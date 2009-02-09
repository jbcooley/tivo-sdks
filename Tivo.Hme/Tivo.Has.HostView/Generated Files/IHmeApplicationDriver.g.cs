//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tivo.Has
{
    
    public interface IHmeApplicationDriver
    {
        System.Collections.Generic.IList<IHmeApplicationIdentity> ApplicationIdentities
        {
            get;
        }
        event System.EventHandler<ApplicationEndedEventArgs> ApplicationEnded;
        IHasApplicationConfigurator GetApplicationConfiguration();
        IHmeConnection CreateHmeConnection(IHmeApplicationIdentity identity, string baseUri, IHmeStream inputStream, IHmeStream outputStream);
        string GetWebPath(IHmeApplicationIdentity identity);
        void HandleEventsAsync(IHmeConnection connection);
        void RunOneAsync(IHmeConnection connection);
    }
}

