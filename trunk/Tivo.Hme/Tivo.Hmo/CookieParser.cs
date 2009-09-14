//
// System.Net.HttpWebResponse
//
// Authors:
// 	Lawrence Pit (loz@cable.a2000.nl)
// 	Gonzalo Paniagua Javier (gonzalo@ximian.com)
//      Daniel Nauck    (dna(at)mono-project(dot)de)
//
// (c) 2002 Lawrence Pit
// (c) 2003 Ximian, Inc. (http://www.ximian.com)
// (c) 2008 Daniel Nauck
//
// stripped only necessary functions from System.Net.HttpWebResponse in mono
// 2009 Josh Cooley

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
using System.Collections;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Net;

namespace Tivo.Hmo
{
    class CookieParser
    {
        public static Cookie ParseCookie(string header, string host)
        {
            string name, val;
            Cookie cookie = null;
            CookieParser parser = new CookieParser(header);

            while (parser.GetNextNameValue(out name, out val))
            {
                if ((name == null || name == "") && cookie == null)
                    continue;

                if (cookie == null)
                {
                    cookie = new Cookie(name, val);
                    continue;
                }

                name = name.ToUpper();
                switch (name)
                {
                    case "COMMENT":
                        if (cookie.Comment == null)
                            cookie.Comment = val;
                        break;
                    case "COMMENTURL":
                        if (cookie.CommentUri == null)
                            cookie.CommentUri = new Uri(val);
                        break;
                    case "DISCARD":
                        cookie.Discard = true;
                        break;
                    case "DOMAIN":
                        if (cookie.Domain == "")
                            cookie.Domain = val;
                        break;
                    case "HTTPONLY":
                        cookie.HttpOnly = true;
                        break;
                    case "MAX-AGE": // RFC Style Set-Cookie2
                        if (cookie.Expires == DateTime.MinValue)
                        {
                            try
                            {
                                cookie.Expires = cookie.TimeStamp.AddSeconds(UInt32.Parse(val));
                            }
                            catch { }
                        }
                        break;
                    case "EXPIRES": // Netscape Style Set-Cookie
                        if (cookie.Expires != DateTime.MinValue)
                            break;

                        cookie.Expires = TryParseCookieExpires(val);
                        break;
                    case "PATH":
                        cookie.Path = val;
                        break;
                    case "PORT":
                        if (cookie.Port == null)
                            cookie.Port = val;
                        break;
                    case "SECURE":
                        cookie.Secure = true;
                        break;
                    case "VERSION":
                        try
                        {
                            cookie.Version = (int)UInt32.Parse(val);
                        }
                        catch { }
                        break;
                }
            }

            if (cookie.Domain == "")
                cookie.Domain = host;

            return cookie;
        }

        private static DateTime TryParseCookieExpires(string value)
        {
            if (value == null || value.Length == 0)
                return DateTime.MinValue;

            try
            {
                return DateTime.Parse(value).ToUniversalTime();
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        string header;
        int pos;
        int length;

        public CookieParser(string header)
            : this(header, 0)
        {
        }

        public CookieParser(string header, int position)
        {
            this.header = header;
            this.pos = position;
            this.length = header.Length;
        }

        public bool GetNextNameValue(out string name, out string val)
        {
            name = null;
            val = null;

            if (pos >= length)
                return false;

            name = GetCookieName();
            if (pos < header.Length && header[pos] == '=')
            {
                pos++;
                val = GetCookieValue();
            }

            if (pos < length && header[pos] == ';')
                pos++;

            return true;
        }

        string GetCookieName()
        {
            int k = pos;
            while (k < length && Char.IsWhiteSpace(header[k]))
                k++;

            int begin = k;
            while (k < length && header[k] != ';' && header[k] != '=')
                k++;

            pos = k;
            return header.Substring(begin, k - begin).Trim();
        }

        string GetCookieValue()
        {
            if (pos >= length)
                return null;

            int k = pos;
            while (k < length && Char.IsWhiteSpace(header[k]))
                k++;

            int begin;
            if (header[k] == '"')
            {
                int j;
                begin = ++k;

                while (k < length && header[k] != '"')
                    k++;

                for (j = k; j < length && header[j] != ';'; j++)
                    ;
                pos = j;
            }
            else
            {
                begin = k;
                while (k < length && header[k] != ';')
                    k++;
                pos = k;
            }

            return header.Substring(begin, k - begin).Trim();
        }
    }
}
