using System;
using System.Collections.Generic;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;

namespace Tivo.Hme.Samples
{
    class Pictures : HmeApplicationHandler
    {
        private const int ShowTime = 4000;
        private static readonly TimeSpan FadeTime = TimeSpan.FromMilliseconds(1250);
        private static List<string> imageNames = new List<string>();
        private Application _application;
        private ImageView _foreground;
        private ImageView _background;
        private Timer _timer;
        private int _currentImageIndex = 0;

        static Pictures()
        {
            string imagePath = Properties.Settings.Default.ImagePath;
            foreach (string imageFileName in System.IO.Directory.GetFiles(imagePath, "*.jpg", System.IO.SearchOption.AllDirectories))
            {
                string imageName = Guid.NewGuid().ToString();
                using (Image image = GetScaledImage(imageFileName, 640, 480))
                {
                    Application.Images.Add(imageName, image, ImageFormat.Jpeg);
                }
                imageNames.Add(imageName);
            }
        }

        private static Image GetScaledImage(string imageFileName, int maxWidth, int maxHeight)
        {
            Image original = Image.FromFile(imageFileName);
            // no scaling if it fits in requested size
            if (original.Width <= maxWidth && original.Height <= maxHeight)
                return original;

            double scale = Math.Max((double)original.Width / maxWidth, (double)original.Height / maxHeight);
            Bitmap scaledBitmap = new Bitmap(original, (int)(original.Width * scale), (int)(original.Height * scale));
            original.Dispose();
            return scaledBitmap;
        }

        public override void OnApplicationStart(HmeApplicationStartArgs e)
        {
            _application = e.Application;
            View root = new View();
            root.Bounds = new Rectangle(0, 0, 640, 480);

            _foreground = new ImageView(null, ImageLayout.BestFit);
            _foreground.Bounds = root.Bounds;
            _background = new ImageView(null, ImageLayout.BestFit);
            _background.Bounds = root.Bounds;

            // add the background first because it is behind the foreground
            root.Children.Add(_background);
            root.Children.Add(_foreground);
            _application.Root = root;

            _timer = new Timer(DisplayNextImage, null, 0, ShowTime);
        }

        public override void OnApplicationEnd()
        {
            _timer.Dispose();
        }

        public void DisplayNextImage(object state)
        {
            using (SuspendPainting suspend = new SuspendPainting(_application.Root))
            {
                if (_background.ImageResource != null)
                {
                    // release old images
                    _background.ImageResource.Close();
                }
                _background.ImageResource = _foreground.ImageResource;

                _background.Transparency = 0;
                _foreground.Transparency = 1;

                _foreground.ImageResource = _application.GetImageResource(imageNames[_currentImageIndex]);

                _foreground.Animate(Animation.Fade(0, 0, FadeTime));
                _background.Animate(Animation.Fade(1, 0, FadeTime));
            }
            _currentImageIndex = (_currentImageIndex + 1) % imageNames.Count;
        }
    }
}
