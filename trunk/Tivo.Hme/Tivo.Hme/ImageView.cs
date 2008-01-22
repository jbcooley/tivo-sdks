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
    public class ImageView : View
    {
        private ImageResource _imageResource;
        private ImageLayout _imageLayout;

        public ImageView(ImageResource imageResource)
            : this(imageResource, 0)
        {
        }

        public ImageView(ImageResource imageResource, ImageLayout imageLayout)
        {
            Update(imageResource, imageLayout);
        }

        public ImageResource ImageResource
        {
            get { return _imageResource; }
            set { Update(value); }
        }

        public ImageLayout Layout
        {
            get { return _imageLayout; }
        }

        public void Update(ImageResource imageResource)
        {
            Update(imageResource, _imageLayout);
        }

        public void Update(ImageResource imageResource, ImageLayout layout)
        {
            _imageResource = imageResource;
            _imageLayout = layout;
            // handle null images so that it is easier to write photo viewers
            if (Application != null && _imageResource != null)
            {
                ResourceId = Application.GetResourceId(new Resource(ImageResource.Name, ResourceType.Image));
                PostCommand(new Commands.ViewSetResource(ViewId, ResourceId, (long)Layout));
            }
        }

        protected override void OnNewApplication()
        {
            // handle null images so that it is easier to write photo viewers
            if (_imageResource != null)
            {
                ResourceId = Application.GetResourceId(new Resource(ImageResource.Name, ResourceType.Image));
                PostCommand(new Commands.ViewSetResource(ViewId, ResourceId, (long)Layout));
            }
            base.OnNewApplication();
        }
    }
}
