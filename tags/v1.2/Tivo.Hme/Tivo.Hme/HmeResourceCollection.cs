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
    /// <summary>
    /// Used internally by the framework
    /// </summary>
    public interface IHmeResource : IDisposable, IEquatable<IHmeResource>
    {
        /// <summary>
        /// A unique name usually used as the key in an <see cref="HmeResourceCollection"/>.
        /// </summary>
        string Name { get;}
        /// <summary>
        /// Releases the resource allocated to the application.
        /// </summary>
        void Close();
    }

    /// <summary>
    /// Represents a name, value collection used to store raw resources
    /// </summary>
    public class HmeResourceCollection
    {
        private Dictionary<string, object> _resources = new Dictionary<string, object>();

        /// <summary>
        /// The number of resources stored
        /// </summary>
        public int Count
        {
            get { return _resources.Count; }
        }

        /// <summary>
        /// Adds the file as a resource.
        /// </summary>
        /// <param name="name">A unique name for the resource.</param>
        /// <param name="filePath">A valid path to a file.</param>
        public void Add(string name, string filePath)
        {
            _resources[name] = filePath;
        }

        /// <summary>
        /// Adds the byte[] as a resource.
        /// </summary>
        /// <param name="name">A unique name for the resource.</param>
        /// <param name="bytes">The raw bytes for a resource.</param>
        public void Add(string name, byte[] bytes)
        {
            _resources[name] = bytes;
        }

        /// <summary>
        /// Checks for the existence of a named resource.
        /// </summary>
        /// <param name="name">The name of a resource.</param>
        /// <returns>true if the collection contains the named resource; false otherwise.</returns>
        public bool Contains(string name)
        {
            return _resources.ContainsKey(name);
        }

        /// <summary>
        /// Removes a named resource.
        /// </summary>
        /// <param name="name">The name of a resource.</param>
        /// <returns>true if the collection contained the named resource before removal; false otherwise.</returns>
        public bool Remove(string name)
        {
            return _resources.Remove(name);
        }

        /// <summary>
        /// Removes all resource from the collection.
        /// </summary>
        public void Clear()
        {
            _resources.Clear();
        }

        /// <summary>
        /// Retrieves a raw byte[] representation of the resource, translating a file into bytes if need be.
        /// </summary>
        /// <param name="name">The name of a resource.</param>
        /// <returns>The raw bytes for a resource.</returns>
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

        /// <summary>
        /// Retrieves a file path for a resource if it exists in that form.
        /// </summary>
        /// <param name="name">The name of a resource.</param>
        /// <param name="filePath">A valid file path if the resource contained one.</param>
        /// <returns>true if a file path exists for the named resource; false otherwise.</returns>
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

    /// <summary>
    /// Represents a name, value collection used to store raw image resources
    /// </summary>
    public class ImageResourceCollection : HmeResourceCollection
    {
        /// <summary>
        /// Add an image using System.Drawing.Image.
        /// </summary>
        /// <param name="name">A unique name for the image.</param>
        /// <param name="image">The image to be stored in the collection.</param>
        /// <param name="format">The format to be used in storing the image.</param>
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
