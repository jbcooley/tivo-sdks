using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Net;

namespace Tivo.Hmo
{
    public class TivoVideo : TivoItem
    {
        private TivoVideo(XElement element)
            : base(element)
        {
            if (!ContentTypes.IsVideo(ContentType))
                throw new FormatException(Properties.Resources.ArgumentNotVideo);
        }

        public static explicit operator TivoVideo(XElement element)
        {
            return new TivoVideo(element);
        }

        public static explicit operator TivoVideo(XDocument document)
        {
            return new TivoVideo(document.Root);
        }

        /// <summary>
        /// Size of video in bytes
        /// </summary>
        public long Length
        {
            get { return GetLength(Element); }
        }

        public TimeSpan Duration
        {
            get { return GetDuration(Element); }
        }

        public DateTimeOffset Captured
        {
            get { return GetCaptured(Element); }
        }

        public CustomIcon CustomIcon
        {
            get
            {
                return GetCustomIcon(Element);
            }
        }

        //public object GetDetails(TivoConnection connection)
        //{
        //    return null;
        //}

        protected static long GetLength(XElement tivoItem)
        {
            return (long)tivoItem.Element(Calypso16.Details).Element(Calypso16.SourceSize);
        }

        protected static TimeSpan GetDuration(XElement tivoItem)
        {
            return TimeSpan.FromMilliseconds((int)tivoItem.Element(Calypso16.Details).Element(Calypso16.Duration));
        }

        protected static DateTimeOffset GetCaptured(XElement tivoItem)
        {
            return DateUtility.ConvertHexEpochSeconds((string)tivoItem.Element(Calypso16.Details).Element(Calypso16.CaptureDate));
        }

        protected static CustomIcon GetCustomIcon(XElement tivoItem)
        {
            var customIconElement = tivoItem.Element(Calypso16.Links).Element(Calypso16.CustomIcon);
            if (customIconElement == null)
                return null;
            return new CustomIcon(customIconElement);
        }
    }
}
