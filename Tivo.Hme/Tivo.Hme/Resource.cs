// Copyright (c) 2008 Josh Cooley

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Tivo.Hme
{
    internal enum ResourceType
    {
        Image,
        Sound,
        TrueTypeFont
    }

    /// <summary>
    /// Summary description for Resource
    /// </summary>
    internal struct Resource : IEquatable<Resource>
    {
        // this provide equality and a hashcode
        // must be prepended with resource type to prevent collisions
        private string _hashString;
        private string _name;
        private Commands.IResourceCommand _addResourceCommand;

        public Resource(TextStyle textStyle)
        {
            _name = textStyle.ToString();
            _hashString = "F:" + _name;
            _addResourceCommand = new Commands.ResourceAddFont(textStyle);
        }

        public Resource(Color color)
        {
            _name = GetColorString(color);
            _hashString = "C:" + _name;
            _addResourceCommand = new Commands.ResourceAddColor(color);
        }

        public Resource(string text, TextStyle style, Color color)
        {
            _hashString = "T:" + text + ";" + style.ToString() + ";" + GetColorString(color);
            _name = _hashString;
            _addResourceCommand = new Commands.ResourceAddText(text, style, color);
        }

        public Resource(float ease, TimeSpan duration)
        {
            _hashString = "A:" + ease.ToString() + ";" + duration.ToString();
            _name = _hashString;
            _addResourceCommand = new Commands.ResourceAddAnimation(ease, duration);
        }

        public Resource(Uri uri, string contentType, bool autoPlay)
        {
            _name = uri.OriginalString;
            _hashString = "M:" + uri.OriginalString;
            _addResourceCommand = new Commands.ResourceAddStream(uri, contentType, autoPlay);
        }

        public Resource(string resourceName, ResourceType resourceType)
        {
            _name = resourceName;
            _hashString = "N:" + resourceName + "RT:" + resourceType.ToString();
            switch (resourceType)
            {
                case ResourceType.Image:
                    _addResourceCommand = new Commands.ResourceAddImage(resourceName);
                    break;
                case ResourceType.Sound:
                    _addResourceCommand = new Commands.ResourceAddSound(resourceName);
                    break;
                case ResourceType.TrueTypeFont:
                    _addResourceCommand = new Commands.ResourceAddTrueTypeFont(resourceName);
                    break;
                default:
                    _addResourceCommand = null;
                    break;
            }
        }

        public Resource(string resourceName, Uri uri, ImageFormat imageFormat)
        {
            _name = resourceName;
            _hashString = "N:" + resourceName + "RT:" + ResourceType.Image;
            string contentType = string.Empty;
            if (imageFormat == ImageFormat.Bmp)
                contentType = "image/bmp";
            else if (imageFormat == ImageFormat.Gif)
                contentType = "image/gif";
            else if (imageFormat == ImageFormat.Jpeg)
                contentType = "image/jpeg";
            else if (imageFormat == ImageFormat.Png)
                contentType = "image/png";
            _addResourceCommand = new Commands.ResourceAddStream(uri, contentType, false);
        }

        public Resource(Uri uri, string contentType, MusicStart state)
        {
            _name = uri.OriginalString;
            _hashString = "M:" + uri.OriginalString;
            _addResourceCommand = new Commands.ResourceAddStream(uri, contentType, state == MusicStart.AutoPlay);
        }

        public Resource(Uri uri, string contentType, VideoStart state)
        {
            _name = uri.OriginalString;
            _hashString = "V:" + uri.OriginalString;
            _addResourceCommand = new Commands.ResourceAddStream(uri, contentType, state == VideoStart.AutoPlay);
        }

        public Commands.IResourceCommand AddResourceCommand
        {
            get { return _addResourceCommand; }
        }

        public override bool Equals(object obj)
        {
            if (obj is Resource)
                return Equals((Resource)obj);
            return false;
        }

        public override int GetHashCode()
        {
            return _hashString.GetHashCode();
        }

        #region IEquatable<Resource> Members

        public bool Equals(Resource other)
        {
            return _hashString == other._hashString;
        }

        #endregion

        internal string Name
        {
            get { return _name; }
        }

        internal bool IsResourceType(ResourceType resourceType)
        {
            return _hashString.EndsWith("RT:" + resourceType);
        }

        internal bool IsMusic
        {
            get { return _hashString.StartsWith("M:"); }
        }

        private static string GetColorString(Color color)
        {
            return string.Format("{0:X}", color.ToArgb());
        }
    }
}
