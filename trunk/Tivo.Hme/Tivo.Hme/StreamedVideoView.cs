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
    public enum VideoStart
    {
        AutoPlay,
        Pause
    }

    public class StreamedVideoView : View, IHmeResource
    {
        private Resource _resource;

        public StreamedVideoView()
        {
        }

        public StreamedVideoView(Uri uri, string contentType, VideoStart state)
            : this()
        {
            Update(uri, contentType, state);
        }

        public void Play(Uri uri, string contentType)
        {
            Update(uri, contentType, VideoStart.AutoPlay);
        }

        public void Load(Uri uri, string contentType)
        {
            Update(uri, contentType, VideoStart.Pause);
        }

        public void Pause()
        {
            Application.PostCommand(new Commands.ResourceSetSpeed(ResourceId, 0));
        }

        public void Play()
        {
            Application.PostCommand(new Commands.ResourceSetSpeed(ResourceId, 1));
        }

        public void Seek(TimeSpan position)
        {
            Application.PostCommand(new Commands.ResourceSetPosition(ResourceId, position));
        }

        public void Forward(float speed)
        {
            Application.PostCommand(new Commands.ResourceSetSpeed(ResourceId, speed));
        }

        public void Reverse(float speed)
        {
            Application.PostCommand(new Commands.ResourceSetSpeed(ResourceId, -speed));
        }

        #region IHmeResource Members

        public string Name
        {
            get { return _resource.Name; }
        }

        void IHmeResource.Close()
        {
            ReleaseResource();
        }

        #endregion

        public void Stop()
        {
            ((IHmeResource)this).Close();
        }

        #region IEquatable<IHmeResource> Members

        public bool Equals(IHmeResource other)
        {
            return ReferenceEquals(this, other);
        }

        #endregion

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            return ResourceId.GetHashCode();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ReleaseResource();
            }
            base.Dispose(disposing);
        }

        private void ReleaseResource()
        {
            if (ResourceId != 0)
            {
                Application.UnregisterHmeResource(ResourceId);
                Application.PostCommand(new Commands.ResourceClose(ResourceId));
                Application.ReleaseResourceId(ResourceId);
                ResourceId = 0;
            }
        }

        protected override void OnNewApplication()
        {
            Create();
            base.OnNewApplication();
        }

        private void Update(Uri uri, string contentType, VideoStart state)
        {
            if (ResourceId != 0 && Application != null)
            {
                ReleaseResource();
            }
            _resource = new Resource(uri, contentType, state);
            if (Application != null)
            {
                Create();
            }
        }

        private void Create()
        {
            // don't create when no resource exists
            // resources can't exist without a name
            if (!string.IsNullOrEmpty(_resource.Name))
            {
                ResourceId = Application.GetResourceId(_resource);
                PostCommand(new Commands.ViewSetResource(ViewId, ResourceId, 0));
                Application.RegisterHmeResource(ResourceId, this);
            }
        }
    }
}
