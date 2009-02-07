using System;
using System.Collections.Generic;
using System.Text;

namespace Tivo.Hme.Host.Services
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class UsesHostHttpServicesAttribute : Attribute
    {
    }
}
