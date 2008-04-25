using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tivo.Hme;

namespace Tivo.Has.AddIn
{
    class HmeApplicationIdentity : IHmeApplicationIdentity
    {
        private Type _hmeApplicationHandler = Type.GetType(Properties.Settings.Default.ApplicationType);

        public HmeApplicationIdentity()
        {
            object[] attributes = _hmeApplicationHandler.GetCustomAttributes(typeof(ApplicationIconAttribute), true);
            if (attributes.Length != 0)
            {
                Icon = ((ApplicationIconAttribute)attributes[0]).Icon;
            }
            // TODO: else set Icon = default icon
        }

        #region IHmeApplicationIdentity Members

        public string Name
        {
            get { return Properties.Settings.Default.ApplicationName; }
        }

        public byte[] Icon { get; private set; }

        public Uri EndPoint
        {
            get { return new Uri(Properties.Settings.Default.EndPoint); }
        }

        #endregion
    }
}
