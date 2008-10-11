using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Tivo.Hmo
{
    public static class Calypso16
    {
        private static readonly XNamespace _namespace = "http://www.tivo.com/developer/calypso-protocol-1.6/";

        public static XNamespace XNamespace
        {
            get { return _namespace; }
        }

        public static XName Details
        {
            get { return XNamespace + "Details"; }
        }

        public static XName SourceFormat
        {
            get { return XNamespace + "SourceFormat"; }
        }

        public static XName Title
        {
            get { return XNamespace + "Title"; }
        }

        public static XName Links
        {
            get { return XNamespace + "Links"; }
        }

        public static XName Content
        {
            get { return XNamespace + "Content"; }
        }

        public static XName Url
        {
            get { return XNamespace + "Url"; }
        }

        public static XName ContentType
        {
            get { return XNamespace + "ContentType"; }
        }

        public static XName Item
        {
            get { return XNamespace + "Item"; }
        }

        public static XName LastChangeDate
        {
            get { return XNamespace + "LastChangeDate"; }
        }

        public static XName SourceSize
        {
            get { return XNamespace + "SourceSize"; }
        }

        public static XName Duration
        {
            get { return XNamespace + "Duration"; }
        }

        public static XName CaptureDate
        {
            get { return XNamespace + "CaptureDate"; }
        }

        public static XName CustomIcon
        {
            get { return XNamespace + "CustomIcon"; }
        }

        public static XName AcceptsParams
        {
            get { return XNamespace + "AcceptsParams"; }
        }
    }
}
