using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Tivo.Hmo
{
    public class TivoContainer : TivoItem
    {
        private TivoContainer(XElement element)
            : base(element)
        {
            if (!ContentTypes.IsContainer(ContentType))
                throw new FormatException(Properties.Resources.ArgumentNotContainer);
        }

        public static explicit operator TivoContainer(XElement element)
        {
            return new TivoContainer(element);
        }

        public static explicit operator TivoContainer(XDocument document)
        {
            return new TivoContainer(document.Root);
        }

        public DateTimeOffset LastChanged
        {
            get { return GetLastChanged(Element); }
        }

        public int TotalItems
        {
            get { return (int)Element.Element(Calypso16.Details).Element(Calypso16.TotalItems); }
        }

        public int ItemStart
        {
            get { return (int)Element.Element(Calypso16.ItemStart); }
        }

        public int ItemCount
        {
            get { return (int)Element.Element(Calypso16.ItemCount); }
        }

        public TivoItemCollection TivoItems
        {
            get { return new TivoItemCollection(Element); }
        }

        protected static DateTimeOffset GetLastChanged(XElement tivoItem)
        {
            return DateUtility.ConvertHexEpochSeconds((string)tivoItem.Element(Calypso16.Details).Element(Calypso16.LastChangeDate));
        }
    }
}
