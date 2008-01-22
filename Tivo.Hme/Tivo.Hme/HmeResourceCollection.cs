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
using System.Collections.Generic;
using System.Text;

namespace Tivo.Hme
{
    public interface IHmeResource : IDisposable, IEquatable<IHmeResource>
    {
        string Name { get;}
        void Close();
    }

    public class HmeResourceCollection
    {
        private Dictionary<string, object> _resources = new Dictionary<string, object>();

        public int Count
        {
            get { return _resources.Count; }
        }

        public void Add(string name, string filePath)
        {
            _resources[name] = filePath;
        }

        public void Add(string name, byte[] bytes)
        {
            _resources[name] = bytes;
        }

        public bool Contains(string name)
        {
            return _resources.ContainsKey(name);
        }

        public bool Remove(string name)
        {
            return _resources.Remove(name);
        }

        public void Clear()
        {
            _resources.Clear();
        }

        public byte[] GetBytes(string name)
        {
            object value = _resources[name];
            byte[] bytes = value as byte[];
            if (bytes == null)
            {
                bytes = System.IO.File.ReadAllBytes((string)value);
            }
            return bytes;
        }

        public bool TryGetFilePath(string name, out string filePath)
        {
            object value;
            filePath = null;
            if (_resources.TryGetValue(name, out value))
            {
                filePath = value as string;
                return filePath != null;
            }
            return false;
        }
    }

    public class ImageResourceCollection : HmeResourceCollection
    {
        public void Add(string name, System.Drawing.Image image, System.Drawing.Imaging.ImageFormat format)
        {
            if (!Contains(name))
            {
                using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                {
                    image.Save(stream, format);
                    stream.Flush();
                    Add(name, stream.ToArray());
                }
            }
        }
    }
}
