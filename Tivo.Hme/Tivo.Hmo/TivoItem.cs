using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Tivo.Hmo
{
    public class TivoItem
    {
        protected TivoItem(XElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            if (element.Element(Calypso16.Details) == null)
                throw new ArgumentException();

            Element = element;
        }                                                                                       

        protected XElement Element { get; private set; }

        public string ContentType
        {
            get { return GetContentType(Element); }
        }

        public string SourceFormat
        {
            get { return GetSourceFormat(Element); }
        }

        public string Name
        {
            get { return GetName(Element); }
        }

        protected internal string ContentUrl
        {
            get { return GetContentUrl(Element); }
        }

        protected static string GetSourceFormat(XElement tivoItem)
        {
            return (string)tivoItem.Element(Calypso16.Details).Element(Calypso16.SourceFormat);
        }

        protected static string GetName(XElement tivoItem)
        {
            return (string)tivoItem.Element(Calypso16.Details).Element(Calypso16.Title);
        }

        protected internal static string GetContentUrl(XElement tivoItem)
        {
            return (string)tivoItem.Element(Calypso16.Links).Element(Calypso16.Content).Element(Calypso16.Url);
        }

        internal static string GetContentType(XElement tivoItem)
        {
            return (string)tivoItem.Element(Calypso16.Details).Element(Calypso16.ContentType);
        }
    }
}
