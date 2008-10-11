using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Tivo.Hmo
{
    internal class UnknownTivoItem : TivoItem
    {
        internal UnknownTivoItem(XElement element)
            : base(element)
        {
        }
    }
}
