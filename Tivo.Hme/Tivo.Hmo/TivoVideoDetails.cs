using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Cooley.Tivo.Hmo
{
    public sealed class TivoVideoDetails : IEquatable<TivoVideoDetails>
    {
        public TivoVideoDetails(XElement tivoVideoDetails)
        {
            if (tivoVideoDetails == null)
                throw new ArgumentNullException();
            Uri = new Uri((string)tivoVideoDetails.Element(Calypso16.Url));
            ContentType = (string)tivoVideoDetails.Element(Calypso16.ContentType);
            AcceptsParams = (string)tivoVideoDetails.Element(Calypso16.AcceptsParams) == "Yes";
        }

        public Uri Uri { get; private set; }
        public string ContentType { get; private set; }
        public bool AcceptsParams { get; private set; }

        public override int GetHashCode()
        {
            return Uri.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as TivoVideoDetails;
            return Equals(other);
        }

        public bool Equals(TivoVideoDetails other)
        {
            if (object.ReferenceEquals(other, null))
                return false;
            return Uri == other.Uri && ContentType == other.ContentType && AcceptsParams == other.AcceptsParams;
        }

        public static bool operator==(TivoVideoDetails details1, TivoVideoDetails details2)
        {
            if (object.ReferenceEquals(details1, null) && object.ReferenceEquals(details2, null))
                return true;
            if (!object.ReferenceEquals(details1, null))
                return details1.Equals(details2);
            return false;
        }

        public static bool operator!=(TivoVideoDetails details1, TivoVideoDetails details2)
        {
            return !(details1 == details2);
        }

        public override string ToString()
        {
            return Uri.ToString();
        }
    }
}
