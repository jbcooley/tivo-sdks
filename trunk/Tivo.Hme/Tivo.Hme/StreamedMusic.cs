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
    /// Initial status of streamed audio
    /// </summary>
    public enum MusicStart
    {
        /// <summary>
        /// Start playing stream immediately
        /// </summary>
        AutoPlay,
        /// <summary>
        /// Buffer audio stream and wait for Play to be called.
        /// </summary>
        Pause
    }

    /// <summary>
    /// An mp3 audio stream.
    /// </summary>
    public sealed class StreamedMusic : IHmeResource
    {
        private string _name;
        private Application _application;
        private long _resourceId;
        private View _streamHost = new View();

        internal StreamedMusic(Application application, string name, long resourceId)
        {
            application.RegisterHmeResource(resourceId, this);
            _name = name;
            _application = application;
            _resourceId = resourceId;

            _streamHost.PostCommand(new Commands.ViewSetResource(_streamHost.ViewId, _resourceId, 0));
            _application.Root.Children.Add(_streamHost);
        }

        /// <summary>
        /// Pause audio at the currently played location
        /// </summary>
        public void Pause()
        {
            _application.PostCommand(new Commands.ResourceSetSpeed(_resourceId, 0));
        }

        /// <summary>
        /// Begin or continue playing audio.
        /// </summary>
        public void Play()
        {
            _application.PostCommand(new Commands.ResourceSetSpeed(_resourceId, 1));
        }

        /// <summary>
        /// Set starting point of playing or paused stream.
        /// </summary>
        /// <param name="position">Time offset into the mp3 stream.</param>
        public void Seek(TimeSpan position)
        {
            _application.PostCommand(new Commands.ResourceSetPosition(_resourceId, position));
        }

        /// <summary>
        /// Set forward speed, often used to fast forward through a stream.
        /// </summary>
        /// <param name="speed">Speed to move forward in a stream.</param>
        /// <remarks>
        /// The typical fast forward speeds used are 3, 15, and 30.
        /// </remarks>
        public void Forward(float speed)
        {
            _application.PostCommand(new Commands.ResourceSetSpeed(_resourceId, speed));
        }

        /// <summary>
        /// Set the reverse speed, used in rewinding through a stream.
        /// </summary>
        /// <param name="speed">Speed to move backward in a stream.</param>
        /// <remarks>
        /// The typical reverse speeds used are 3, 15, and 30.
        /// </remarks>
        public void Reverse(float speed)
        {
            _application.PostCommand(new Commands.ResourceSetSpeed(_resourceId, -speed));
        }

        #region IHmeResource Members

        /// <summary>
        /// Name that uniquely identifies this stream.  This value may be the same
        /// for multiple streams over multiple instance of the application when the
        /// audio source is the same.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Release client side resources used in playing the stream.
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Release client side resources used in playing the stream.
        /// </summary>
        public void Dispose()
        {
            _application.UnregisterHmeResource(_resourceId);
            _application.PostCommand(new Commands.ResourceClose(_resourceId));
            _application.ReleaseResourceId(_resourceId);
            _application.Root.Children.Remove(_streamHost);
        }

        #endregion

        #region IEquatable<IHmeResource> Members

        /// <summary>
        /// Tests equality between two resources.
        /// </summary>
        /// <param name="other">Another resource</param>
        /// <returns>true if the two streams represent the same source audio; false otherwise</returns>
        public bool Equals(IHmeResource other)
        {
            return Equals((object)other);
        }

        #endregion

        /// <summary>
        /// Tests equality between two resources.
        /// </summary>
        /// <param name="obj">Another resource</param>
        /// <returns>true if the two streams represent the same source audio; false otherwise</returns>
        public override bool Equals(object obj)
        {
            StreamedMusic streamedResource = obj as StreamedMusic;
            if (streamedResource != null)
                return _resourceId == streamedResource._resourceId;
            else
                return false;
        }

        /// <summary>
        /// hash value for use in hash tables.  Does not uniquely identify object.
        /// </summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode()
        {
            return _resourceId.GetHashCode();
        }
    }
}
