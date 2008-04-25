using System;
using System.IO;
using System.AddIn.Contract;
using System.AddIn.Pipeline;
using PipelineHints;

[assembly: SegmentAssemblyName(PipelineSegment.AddInSideAdapter, "Tivo.Has.AddInSideAdapters")]
[assembly: SegmentAssemblyName(PipelineSegment.AddInView, "Tivo.Has.AddInView")]
[assembly: SegmentAssemblyName(PipelineSegment.HostSideAdapter, "Tivo.Has.HostSideAdapter")]
[assembly: SegmentAssemblyName(PipelineSegment.HostView, "Tivo.Has.HostView")]

namespace Tivo.Has.Contracts
{
    [AddInContract]
    public interface IHmeApplicationDriverContract : IContract
    {
        IHmeConnectionContract CreateHmeConnection(IHmeApplicationIdentityContract identity, Stream inputStream, Stream outputStream);
        void HandleEventsAsync(IHmeConnectionContract connection);
        void RunOneAsync(IHmeConnectionContract connection);

        [EventAdd("ApplicationEnded")]
        void ApplicationEndedEventAdd(IApplicationEndedEventHandler handler);
        [EventRemove("ApplicationEnded")]
        void ApplicationEndedEventRemove(IApplicationEndedEventHandler handler);
    }

    [EventHandler]
    public interface IApplicationEndedEventHandler : IContract
    {
        void Handler(IApplicationEndedEventArgs args);
    }

    [EventArgs]
    public interface IApplicationEndedEventArgs : IContract
    {
    }
}
