using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Tivo.Hme.Samples
{
    class Effects : HmeApplicationHandler
    {
        // Each view is used for demonstrating a different property
        View _bounds, _translate, _transparency, _scale, _visible;

        View _content;         // Container for everything inset
        TextView _easeText;        // Displays the current ease value
        TextView _timeText;        // Displays current animation animTime
        float _ease = 0;             // The current ease value
        TimeSpan _animTime = TimeSpan.FromSeconds(1);  // Animation time
        bool _closing = false;

        public override void OnApplicationStart(HmeApplicationStartArgs e)
        {
            e.Application.Root = new ColorView(Color.White);

            _content = new View();
            _content.Margin = SafetyViewMargin.TitleMargin;
            e.Application.Root.Children.Add(_content);

            TextStyle font = new TextStyle("default", System.Drawing.FontStyle.Regular, 17);

            _easeText = new TextView("Ease (use left/right) : " + _ease, font, Color.Black, TextLayout.HorizontalAlignLeft);
            _easeText.Bounds = new Rectangle(300, 0, 190, 20);
            _content.Children.Add(_easeText);
            _timeText = new TextView("Time (use up/down) : " + _animTime.TotalSeconds, font, Color.Black, TextLayout.HorizontalAlignLeft);
            _timeText.Bounds = new Rectangle(300, 20, 190, 20);
            _content.Children.Add(_timeText);

            _transparency = new Square(0, 0, 90, 90, "Transparency", Color.Magenta);
            _visible = new Square(100, 0, 90, 90, "Visible", Color.Magenta);
            _bounds = new Square(300, 100, 90, 90, "Bounds", Color.Magenta);
            _translate = new View();
            _translate.Bounds = new Rectangle(0, 100, 290, 90);
            _translate.Children.Add(new Square(-300, 0, 600, 90, Color.Green));
            _translate.Children.Add(new Square(0, 0, 90, 90, "Translate", Color.Magenta));
            _scale = new Square(0, 200, 90, 90, "Scale", Color.Magenta);
            _content.Children.Add(_transparency);
            _content.Children.Add(_visible);
            _content.Children.Add(_bounds);
            _content.Children.Add(_translate);
            _content.Children.Add(_scale);

            e.Application.KeyPress += new EventHandler<KeyEventArgs>(application_KeyPress);

            System.Threading.Thread animate = new System.Threading.Thread(Animate);
            animate.Start();
        }

        public override void OnApplicationEnd()
        {
            _closing = true;
        }

        private static readonly TimeSpan _250ms = TimeSpan.FromMilliseconds(250);
        void application_KeyPress(object sender, KeyEventArgs e)
        {
            bool settingsChanged = false;

            switch (e.KeyCode)
            {
                case KeyCode.Up:
                    if (_animTime.TotalMilliseconds + 250 > 9750)
                        _animTime = TimeSpan.FromMilliseconds(9750);
                    else
                        _animTime = _animTime.Add(_250ms);
                    settingsChanged = true;
                    break;
                case KeyCode.Down:
                    if (_animTime.TotalMilliseconds - 250 < 1000)
                        _animTime = TimeSpan.FromMilliseconds(1000);
                    else
                        _animTime = _animTime.Subtract(_250ms);
                    settingsChanged = true;
                    break;
                case KeyCode.Left:
                    if (_ease - 0.1 < -1)
                        _ease = -1;
                    else
                        _ease -= 0.1F;
                    settingsChanged = true;
                    break;
                case KeyCode.Right:
                    if (_ease + 0.1 > 1)
                        _ease = 1;
                    else
                        _ease += 0.1F;
                    settingsChanged = true;
                    break;
            }

            if (settingsChanged)
            {
                ShowSettings(Color.Red);
            }
        }

        private void Animate()
        {
            bool parity = false;
            while (!_closing)
            {
                parity = !parity;

                _transparency.Animate(Animation.Fade(parity ? 1 : 0, _ease, _animTime));
                // TODO: double check this
                _visible.Animate(Animation.Visibility(!parity, _ease, _animTime));
                _translate.Animate(Animation.Translate(new Point(parity ? 200 : 0, 0), _ease, _animTime));
                _scale.Animate(Animation.Scale(new SizeF(parity ? 1.5F : 1.0F, parity ? 1.5F : 1.0F), _ease, _animTime));

                if (parity)
                {
                    Square remove = new Square(200, 0, 90, 90, "Remove", Color.Magenta);
                    _content.Children.Add(remove);
                    _content.Children.Remove(remove, _animTime);
                    _bounds.Animate(Animation.SetBounds(new Rectangle(300, 150, 190, 190), _ease, _animTime));
                }
                else
                {
                    _bounds.Animate(Animation.SetBounds(new Rectangle(300, 100, 90, 90), _ease, _animTime));
                }

                ShowSettings(Color.Black);

                System.Threading.Thread.Sleep(_animTime);
            }
        }

        private void ShowSettings(Color color)
        {
            _easeText.Update("Ease (use left/right) : " + _ease, _easeText.Style, color);
            _timeText.Update("Time (use up/down) : " + _animTime.TotalSeconds, _timeText.Style, color);
        }

        private class Square : ColorView
        {
            public Square(int x, int y, int width, int height, Color backgroundColor)
                : base(backgroundColor)
            {
                Bounds = new Rectangle(x, y, width, height);
            }

            public Square(int x, int y, int width, int height, String title, Color backgroundColor)
                : this(x, y, width, height, backgroundColor)
            {
                TextView label = new TextView(title, new TextStyle("default", FontStyle.Regular, 17), Color.White);
                label.Bounds = new Rectangle(0, 0, width, height);
                Children.Add(label);
            }
        }
    }
}
