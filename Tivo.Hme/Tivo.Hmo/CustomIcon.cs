using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Tivo.Hmo
{
    public sealed class CustomIcon : IEquatable<CustomIcon>
    {
        internal CustomIcon(XElement customIcon)
        {
            Uri = new Uri((string)customIcon.Element(Calypso16.Url));
            ContentType = (string)customIcon.Element(Calypso16.ContentType);
            AcceptsParams = (string)customIcon.Element(Calypso16.AcceptsParams) == "Yes";
        }

        public Uri Uri { get; private set; }
        public string ContentType { get; private set; }
        public bool AcceptsParams { get; private set; }

        public bool Equals(CustomIcon other)
        {
            return Uri == other.Uri && ContentType == other.ContentType && AcceptsParams == other.AcceptsParams;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(CustomIcon))
                return false;
            return Equals((CustomIcon)obj);
        }

        public override int GetHashCode()
        {
            return Uri.GetHashCode();
        }

        public override string ToString()
        {
            return Uri.ToString();
        }
    }
}
