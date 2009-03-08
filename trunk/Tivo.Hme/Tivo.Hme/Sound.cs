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

namespace Tivo.Hme
{
    /// <summary>
    /// Represents a primitive sound resource.  Usually one of the built in tivo effects.
    /// </summary>
    public sealed class Sound : IHmeResource
    {
        private Application _application;
        private string _name;
        private long _resourceId;

        /// <summary>
        /// Creates a sound resource.  Used internally by <see cref="Application.GetSound"/>
        /// </summary>
        /// <param name="application">Application instance associated with sound</param>
        /// <param name="name">The name of the sound resource</param>
        /// <param name="resourceId">resource id.</param>
        public Sound(Application application, string name, long resourceId)
        {
            _application = application;
            _name = name;
            _resourceId = resourceId;
        }

        #region IHmeResource Members

        /// <summary>
        /// Name of the sound resource
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Releases the sound resource.
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Releases the sound resource.
        /// </summary>
        public void Dispose()
        {
            if (_resourceId >= 2048)
                _application.ReleaseResourceId(_resourceId);
        }

        #endregion

        /// <summary>
        /// Play sound at normal speed.
        /// </summary>
        public void Play()
        {
            _application.PostCommand(new Commands.ResourceSetSpeed(_resourceId, 1));
        }

        /// <summary>
        /// Play sound at specified speed where 1 is normal speed.
        /// </summary>
        /// <param name="speed"></param>
        public void Play(float speed)
        {
            _application.PostCommand(new Commands.ResourceSetSpeed(_resourceId, speed));
        }

        #region IEquatable<IHmeResource> Members

        /// <summary>
        /// Test equality between resources.
        /// </summary>
        /// <param name="other">Another resource.</param>
        /// <returns>true if the resources are equivelent; false otherwise.</returns>
        public bool Equals(IHmeResource other)
        {
            return Equals((object)other);
        }

        #endregion

        /// <summary>
        /// Tests equality between resources.
        /// </summary>
        /// <param name="obj">An object that should be a resource.</param>
        /// <returns>true if the objects are equal; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            Sound soundResource = obj as Sound;
            if (soundResource != null)
                return _resourceId == soundResource._resourceId;
            else
                return false;
        }

        /// <summary>
        /// Serves as a hash function.
        /// </summary>
        /// <returns>A hash code for the current resource.</returns>
        public override int GetHashCode()
        {
            return _resourceId.GetHashCode();
        }
    }
}
