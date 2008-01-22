using System;
using System.Collections.Generic;
using System.Drawing;

namespace Tivo.Hme.Samples
{
    class Animate : HmeApplicationHandler
    {
        private static Random rand = new Random();

        private View _content;
        private Sprite[] _sprites = new Sprite[16];

        public override void OnApplicationStart(HmeApplicationStartArgs e)
        {
            e.Application.Root = new View();
            _content = new View();
            _content.Margin = SafetyViewMargin.TitleMargin;
            e.Application.Root.Children.Add(_content);

            for (int i = 0; i < _sprites.Length; ++i)
            {
                _sprites[i] = new Sprite(i, e.Application,
                    rand.Next(_content.Bounds.Width),
                    rand.Next(_content.Bounds.Height),
                    8 + rand.Next(64), 8 + rand.Next(64));
                _content.Children.Add(_sprites[i]);
            }
        }

        public override void OnApplicationEnd()
        {
        }

        private class Sprite : ColorView
        {
            private int _index;
            private Application _application;

            public Sprite(int index, Application application, int x, int y, int width, int height)
                : base(Color.FromArgb(rand.Next(0x80, 0x100), rand.Next(0xFF), rand.Next(0xFF), rand.Next(0xFF)))
            {
                _index = index;
                _application = application;
                Bounds = new Rectangle(x, y, width, height);
                Animate();
            }

            public void Animate()
            {
                TimeSpan delay = TimeSpan.FromMilliseconds(250 + rand.Next(5000));

                if (Parent != null)
                {
                    Animate(Animation.Move(new Point(rand.Next(Parent.Bounds.Width), rand.Next(Parent.Bounds.Height)),
                        0, delay));
                }
                else
                {
                    Animate(Animation.Move(new Point(rand.Next(640), rand.Next(480)),
                        0, delay));
                }

                _application.DelayAction<object>(delay, null, delegate(object obj)
                {
                    Animate();
                });
            }
        }
    }
}
