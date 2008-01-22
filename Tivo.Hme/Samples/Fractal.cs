using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Tivo.Hme.Samples
{
    class Fractal : HmeApplicationHandler
    {
        Mandel _mandel;

        public override void OnApplicationStart(HmeApplicationStartArgs e)
        {
            View root = new View();
            _mandel = new Mandel();
            _mandel.Margin = SafetyViewMargin.TitleMargin;
            root.Children.Add(_mandel);
            e.Application.Root = root;
            e.Application.KeyPress += new EventHandler<KeyEventArgs>(application_KeyPress);
        }

        public override void OnApplicationEnd()
        {
        }

        void application_KeyPress(object sender, KeyEventArgs e)
        {
            Application app = (Application)sender;
            switch (e.KeyCode)
            {
                case KeyCode.ThumbsUp:
                case KeyCode.ChannelUp:
                case KeyCode.Forward:
                    _mandel.ScaleFractal(0.5);
                    break;

                case KeyCode.ThumbsDown:
                case KeyCode.ChannelDown:
                case KeyCode.Reverse:
                    _mandel.ScaleFractal(2);
                    break;

                case KeyCode.Up:
                    _mandel.MoveFractal(0, -0.1);
                    break;
                case KeyCode.Down:
                    _mandel.MoveFractal(0, 0.1);
                    break;
                case KeyCode.Left:
                    _mandel.MoveFractal(-0.1, 0);
                    break;
                case KeyCode.Right:
                    _mandel.MoveFractal(0.1, 0);
                    break;
            }
        }

        private class Mandel : ImageView
        {
            private const int Iterations = 100;
            private Bitmap _mandelImage;
            private double _mandelX;
            private double _mandelY;
            private double _mandelWidth;
            private double _mandelHeight;
            private Color[] _palette;
            private string _imageName = Guid.NewGuid().ToString();

            public Mandel()
                : base(null)
            {
                _palette = new Color[Iterations + 1];
                for (int i = 0; i < Iterations; ++i)
                {
                    int distance = (i % 10) * 25;
                    int r = distance;
                    int g = 0;
                    int b = distance;

                    _palette[i] = Color.FromArgb(r & 0xFF, g, b & 0xFF);
                }
                _palette[Iterations] = _palette[0];
            }

            protected override void OnNewApplication()
            {
                base.OnNewApplication();
                SetFractalBounds(-2, -1.5, 2.5, 2.75);
            }

            public void MoveFractal(double deltaX, double deltaY)
            {
                Application.GetSound("updown").Play();
                SetFractalBounds(_mandelX + (deltaX * _mandelWidth), _mandelY + (deltaY * _mandelHeight), _mandelWidth, _mandelHeight);
            }

            public void ScaleFractal(double scaleFactor)
            {
                Application.GetSound((scaleFactor < 1) ? "left" : "right").Play();
                SetFractalBounds(_mandelX + (_mandelWidth / 2.0) * (1.0 - scaleFactor),
                              _mandelY + (_mandelHeight / 2.0) * (1.0 - scaleFactor),
                              _mandelWidth * scaleFactor, _mandelHeight * scaleFactor);
            }

            public void SetFractalBounds(double mandelX, double mandelY, double mandelWidth, double mandelHeight)
            {
                // update the fractal
                Update(mandelX, mandelY, mandelWidth, mandelHeight);

                // update the mandel image
                if (ImageResource != null)
                    ImageResource.Close();
                if (Application.Images.Contains(_imageName))
                    Application.Images.Remove(_imageName);
                Application.Images.Add(_imageName, _mandelImage, ImageFormat.Jpeg);
                ImageResource = Application.GetImageResource(_imageName);
            }

            private void Update(double mandelX, double mandelY, double mandelWidth, double mandelHeight)
            {
                _mandelX = mandelX;
                _mandelY = mandelY;
                _mandelWidth = mandelWidth;
                _mandelHeight = mandelHeight;

                if (_mandelImage == null)
                    _mandelImage = new Bitmap(Bounds.Width, Bounds.Height, PixelFormat.Format24bppRgb);

                byte[] bytes = new byte[_mandelImage.Width * _mandelImage.Height * 3];
                // compute the image
                int boundsHeight = Bounds.Height;
                int boundsWidth = Bounds.Width;
                for (int y = 0; y < boundsHeight; ++y)
                {
                    double pY = mandelY + mandelHeight * ((double)y / (double)boundsHeight);
                    for (int x = 0; x < boundsWidth; ++x)
                    {
                        double iX = 0;
                        double iY = 0;
                        double pX = mandelX + mandelWidth * ((double)x / (double)boundsWidth);

                        int loop;
                        for (loop = 0; loop < Iterations; ++loop)
                        {
                            double nX = iX * iX - iY * iY + pX;
                            iY = 2.0 * iX * iY + pY;
                            iX = nX;
                            if ((iX * iX + iY * iY) > 4)
                            {
                                break;
                            }
                        }
                        int index = (x + y * _mandelImage.Width) * 3;
                        bytes[index] = _palette[loop].R;
                        bytes[index + 1] = _palette[loop].G;
                        bytes[index + 2] = _palette[loop].B;
                    }
                }
                BitmapData bmpData = _mandelImage.LockBits(new Rectangle(0, 0, boundsWidth, boundsHeight), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                System.Runtime.InteropServices.Marshal.Copy(bytes, 0, bmpData.Scan0, bytes.Length);
                _mandelImage.UnlockBits(bmpData);
            }
        }
    }
}
