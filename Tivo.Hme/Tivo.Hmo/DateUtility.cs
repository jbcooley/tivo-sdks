using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tivo.Hmo
{
    static class DateUtility
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTimeOffset ConvertHexEpochSeconds(string hexSeconds)
        {
            return Epoch + TimeSpan.FromSeconds(Convert.ToUInt32(hexSeconds, 16));
        }
    }
}
