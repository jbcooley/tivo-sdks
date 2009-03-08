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
    /// A <see cref="View"/> that can display an image.
    /// </summary>
    public class ImageView : View
    {
        private ImageResource _imageResource;
        private ImageLayout _imageLayout;

        /// <summary>
        /// Creates an <see cref="ImageView"/> that displays an image.
        /// </summary>
        /// <param name="imageResource">An image retrieved from <see cref="Application.GetImageResource"/>.</param>
        public ImageView(ImageResource imageResource)
            : this(imageResource, 0)
        {
        }

        /// <summary>
        /// Creates an <see cref="ImageView"/> that displays an image.
        /// </summary>
        /// <param name="imageResource">An image retrieved from <see cref="Application.GetImageResource"/>.</param>
        /// <param name="imageLayout">A set of <see cref="ImageLayout"/> flags that controls the layout of the image.</param>
        public ImageView(ImageResource imageResource, ImageLayout imageLayout)
        {
            Update(imageResource, imageLayout);
        }

        /// <summary>
        /// The image resources that is being displayed.
        /// </summary>
        public ImageResource ImageResource
        {
            get { return _imageResource; }
            set { Update(value); }
        }

        /// <summary>
        /// Gets the layout of the image.  Use one of
        /// the <see cref="ImageView.Update"/> methods to change the <see cref="ImageLayout"/>.
        /// </summary>
        public ImageLayout Layout
        {
            get { return _imageLayout; }
        }

        /// <summary>
        /// Sets the ImageResource property to the given value.
        /// </summary>
        /// <param name="imageResource">The image resources that is to be displayed.</param>
        public void Update(ImageResource imageResource)
        {
            Update(imageResource, _imageLayout);
        }

        /// <summary>
        /// Updates the image to be displayed with a new layout.
        /// </summary>
        /// <param name="imageResource">The image resources that is to be displayed.</param>
        /// <param name="layout">The layout of the image.</param>
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

        /// <summary>
        /// Overridden to display the image once the application starts.
        /// </summary>
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
