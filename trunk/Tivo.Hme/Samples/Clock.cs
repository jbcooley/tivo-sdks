using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Tivo.Hme.Samples
{
    class Clock : HmeApplicationHandler
    {
        private TextView[] _timeViews;
        private System.Threading.Timer _timer;
        private int _activeTime = 0;

        public override void OnApplicationStart(HmeApplicationStartArgs e)
        {
            e.Application.Root = new ColorView(Color.White);

            _timeViews = new TextView[2];

            TextStyle font = new TextStyle("default", FontStyle.Bold, 96);
            for (int i = 0; i < _timeViews.Length; ++i)
            {
                _timeViews[i] = new TextView(string.Empty, font, Color.Black, TextLayout.HorizontalAlignCenter);
                _timeViews[i].Bounds = new Rectangle(0, 100, e.Application.Root.Bounds.Width, 280);
                e.Application.Root.Children.Add(_timeViews[i]);
            }

            app = e.Application;
            e.Application.KeyPress += new EventHandler<KeyEventArgs>(Application_KeyPress);

            _timer = new System.Threading.Timer(UpdateTime, null, 0, 1000);
        }

        int res = 0;
        Application app;
        void Application_KeyPress(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == KeyCode.ChannelDown)
            {
                app.CurrentResolution = app.SupportedResolutions[res];
                res = (res + 1) % app.SupportedResolutions.Count;
            }
        }

        public override void OnApplicationEnd()
        {
            _timer.Dispose();
        }

        private void UpdateTime(object state)
        {
            _timeViews[_activeTime].Animate(Animation.Fade(1, 0, TimeSpan.FromSeconds(0.75)));

            _activeTime = _activeTime ^ 1;

            _timeViews[_activeTime].Text = DateTime.Now.ToString("HH:mm:ss");
            _timeViews[_activeTime].Animate(Animation.Fade(0, 0, TimeSpan.FromSeconds(0.75)));
        }
    }
}
