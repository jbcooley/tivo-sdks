using System;
using System.IO;
using System.AddIn.Contract;
using System.AddIn.Pipeline;
#if PIPELINE_BUILDER
using PipelineHints;

[assembly: SegmentAssemblyName(PipelineSegment.AddInSideAdapter, "Tivo.Has.AddInSideAdapters")]
[assembly: SegmentAssemblyName(PipelineSegment.AddInView, "Tivo.Has.AddInView")]
[assembly: SegmentAssemblyName(PipelineSegment.HostSideAdapter, "Tivo.Has.HostSideAdapter")]
[assembly: SegmentAssemblyName(PipelineSegment.HostView, "Tivo.Has.HostView")]
#endif

namespace Tivo.Has.Contracts
{
    [AddInContract]
    public interface IHmeApplicationDriverContract : IContract
    {
        IListContract<IHmeApplicationIdentityContract> ApplicationIdentities { get; }
        IHmeConnectionContract CreateHmeConnection(IHmeApplicationIdentityContract identity, IHmeStreamContract inputStream, IHmeStreamContract outputStream);
        void HandleEventsAsync(IHmeConnectionContract connection);
        void RunOneAsync(IHmeConnectionContract connection);
        
#if PIPELINE_BUILDER
        [EventAdd("ApplicationEnded")]
#endif
        void ApplicationEndedEventAdd(IApplicationEndedEventHandler handler);

#if PIPELINE_BUILDER
        [EventRemove("ApplicationEnded")]
#endif
        void ApplicationEndedEventRemove(IApplicationEndedEventHandler handler);
    }
    
#if PIPELINE_BUILDER
    [EventHandler]
#endif
    public interface IApplicationEndedEventHandler : IContract
    {
        void Handler(IApplicationEndedEventArgs args);
    }
    
#if PIPELINE_BUILDER
    [EventArgs]
#endif
    public interface IApplicationEndedEventArgs : IContract
    {
    }
}
