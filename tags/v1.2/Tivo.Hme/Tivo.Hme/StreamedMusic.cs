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
    public enum MusicStart
    {
        AutoPlay,
        Pause
    }

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

        public void Pause()
        {
            _application.PostCommand(new Commands.ResourceSetSpeed(_resourceId, 0));
        }

        public void Play()
        {
            _application.PostCommand(new Commands.ResourceSetSpeed(_resourceId, 1));
        }

        public void Seek(TimeSpan position)
        {
            _application.PostCommand(new Commands.ResourceSetPosition(_resourceId, position));
        }

        public void Forward(float speed)
        {
            _application.PostCommand(new Commands.ResourceSetSpeed(_resourceId, speed));
        }

        public void Reverse(float speed)
        {
            _application.PostCommand(new Commands.ResourceSetSpeed(_resourceId, -speed));
        }

        #region IHmeResource Members

        public string Name
        {
            get { return _name; }
        }

        public void Close()
        {
            Dispose();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _application.UnregisterHmeResource(_resourceId);
            _application.PostCommand(new Commands.ResourceClose(_resourceId));
            _application.ReleaseResourceId(_resourceId);
            _application.Root.Children.Remove(_streamHost);
        }

        #endregion

        #region IEquatable<IHmeResource> Members

        public bool Equals(IHmeResource other)
        {
            return Equals((object)other);
        }

        #endregion

        public override bool Equals(object obj)
        {
            StreamedMusic streamedResource = obj as StreamedMusic;
            if (streamedResource != null)
                return _resourceId == streamedResource._resourceId;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return _resourceId.GetHashCode();
        }
    }
}
