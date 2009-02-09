//
// System.Net.Cookie.cs
//
// Authors:
// 	Lawrence Pit (loz@cable.a2000.nl)
//	Gonzalo Paniagua Javier (gonzalo@ximian.com)
//      Daniel Nauck    (dna(at)mono-project(dot)de)
//
// stripped only necessary functions from System.Net.Cookie in mono
// 2008 Josh Cooley
//
// (c) Copyright 2004 Novell, Inc. (http://www.ximian.com)
//

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Tivo.Hme.Host.Http
{
    static class CookieHelper
    {
        static string tspecials = "()<>@,;:\\\"/[]?={} \t";   // from RFC 2965, 2068

        internal static string ToClientString(Cookie cookie)
        {
            if (cookie.Name.Length == 0)
                return String.Empty;

            StringBuilder result = new StringBuilder(64);

            if (cookie.Version > 0)
                result.Append("Version=").Append(cookie.Version).Append(";");

            result.Append(cookie.Name).Append("=").Append(cookie.Value);

            if (cookie.Path != null && cookie.Path.Length != 0)
                result.Append(";Path=").Append(QuotedString(cookie, cookie.Path));

            if (cookie.Domain != null && cookie.Domain.Length != 0)
                result.Append(";Domain=").Append(QuotedString(cookie, cookie.Domain));

            if (cookie.Port != null && cookie.Port.Length != 0)
                result.Append(";Port=").Append(cookie.Port);

            return result.ToString();
        }

        // See par 3.6 of RFC 2616
        static string QuotedString(Cookie cookie, string value)
        {
            if (cookie.Version == 0 || IsToken(value))
                return value;
            else
                return "\"" + value.Replace("\"", "\\\"") + "\"";
        }

        static bool IsToken(string value)
        {
            int len = value.Length;
            for (int i = 0; i < len; i++)
            {
                char c = value[i];
                if (c < 0x20 || c >= 0x7f || tspecials.IndexOf(c) != -1)
                    return false;
            }
            return true;
        }
    }
}
