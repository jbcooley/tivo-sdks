using System;
using System.Collections.Generic;
using System.Text;

namespace Tivo.Hme.Host.Services
{
    /// <summary>
    /// Indicates an application needs the host to handle http requests
    /// beyond the standard hme protocol defined urls.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class UsesHostHttpServicesAttribute : Attribute
    {
    }
}
