using System;
using System.Collections.Generic;
using System.Text;

namespace Tivo.Hme.Host
{
    /// <summary>
    /// Provides access to the names of the connection events used in syncronization.
    /// </summary>
    public interface IHmeConnectionSyncronizationInfo
    {
        /// <summary>
        /// The name of the event that is set when HME events are received.
        /// </summary>
        string EventReceivedName { get; }
        /// <summary>
        /// The name of the event that is set when HME commands are ready to be sent.
        /// </summary>
        string CommandReceivedName { get; }
    }
}
