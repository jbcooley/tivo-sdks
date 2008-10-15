using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Tivo.Hme.Samples
{
    class Transition : HmeApplicationHandler
    {
        int _depth = 0;
        int _entryColor = -1;
        int _returnColor = -1;
        int _curColor = 0;

        private View _root;
        private TextView _titleView;
        private TextView _depthView;
        private TextView _entryView;
        private TextView _returnView;
        private TextView _colorView;
        private ColorView _hilightView;
        private View[] _colorViews;
        private TextView _hintsView;
        private TextView _errorView;

        private static readonly Color[] colors = { Color.Red, Color.Yellow, Color.Green, Color.Blue };

        private static readonly TextStyle DefaultRegular30 = new TextStyle("default", FontStyle.Regular, 30);
        private static readonly TextStyle DefaultRegular20 = new TextStyle("default", FontStyle.Regular, 20);
        private static readonly TextStyle DefaultRegular14 = new TextStyle("default", FontStyle.Regular, 14);
        private static readonly TextStyle DefaultRegular18 = new TextStyle("default", FontStyle.Regular, 18);

        public override void OnApplicationStart(HmeApplicationStartArgs e)
        {
            e.Application.ApplicationParametersReceived += new EventHandler<ApplicationParametersReceivedArgs>(Application_ApplicationParametersReceived);
            e.Application.KeyPress += new EventHandler<KeyEventArgs>(Application_KeyPress);
            e.Application.ApplicationErrorOccurred += new EventHandler<ApplicationErrorArgs>(Application_ApplicationErrorOccurred);

            _root = e.Application.Root;

            int x = SafetyViewMargin.HorizontalActionMargin;
            int w = _root.Bounds.Width - 2 * x;

            _titleView = new TextView("HME Transition Test", DefaultRegular30, Color.White);
            _titleView.Bounds = new Rectangle(x, SafetyViewMargin.VerticalActionMargin, w, 40);
            _root.Children.Add(_titleView);

            _depthView = new TextView(string.Empty, DefaultRegular20, Color.White);
            _depthView.Bounds = new Rectangle(x, 70, w, 40);
            _root.Children.Add(_depthView);

            _entryView = new TextView(string.Empty, DefaultRegular20, Color.White);
            _entryView.Bounds = new Rectangle(x, 100, w / 2 - 10, 40);
            _root.Children.Add(_entryView);

            _returnView = new TextView(string.Empty, DefaultRegular20, Color.White);
            _returnView.Bounds = new Rectangle(x + w / 2 + 20, 100, w / 2 - 10, 40);
            _root.Children.Add(_returnView);

            _colorView = new TextView(string.Empty, DefaultRegular20, Color.White);
            _colorView.Bounds = new Rectangle(x, 130, w, 40);
            _root.Children.Add(_colorView);

            _hilightView = new ColorView(Color.White);
            _hilightView.Bounds = new Rectangle(0, 0, 0, 0);
            _root.Children.Add(_hilightView);
            UpdateHilight(e.Application);

            _colorViews = new View[colors.Length];
            for (int i = 0; i < colors.Length; ++i)
            {
                x = SafetyViewMargin.HorizontalActionMargin + 80;
                w = _root.Bounds.Width - 2 * x;
                int y = 180 + i * 50;
                int h = 40;
                _colorViews[i] = new ColorView(colors[i]);
                _colorViews[i].Bounds = new Rectangle(x, y, w, h);
                _root.Children.Add(_colorViews[i]);
            }

            _hintsView = new TextView("Move up and down to select a color.  " +
                "Move left to go back, right to go forward.",
                DefaultRegular14, Color.White);
            _hintsView.Bounds = new Rectangle(x, 400, w, 40);
            _root.Children.Add(_hintsView);

            _errorView = new TextView(string.Empty, DefaultRegular18, Color.Red);
            _errorView.Bounds = new Rectangle(x, 440, w, 40);
            _root.Children.Add(_errorView);
        }

        public override void OnApplicationEnd()
        {
        }

        void Application_ApplicationParametersReceived(object sender, ApplicationParametersReceivedArgs e)
        {
            string value;
            if (e.Parameters.TryGetStringValue("entry", 0, out value))
                _entryColor = Convert.ToInt32(value);
            if (e.Parameters.TryGetStringValue("return", 0, out value))
                _returnColor = Convert.ToInt32(value);
            if (e.Parameters.TryGetStringValue("depth", 0, out value))
                _depth = Convert.ToInt32(value);

            // updateInits
            _depthView.Text = string.Format("Current depth is {0}.", _depth);
            if (_entryColor < 0)
                _entryView.Update("No entry color.", _entryView.Style, Color.Gray);
            else
                _entryView.Update(colors[_entryColor].ToString(), _entryView.Style, colors[_entryColor]);
            if (_returnColor < 0)
                _returnView.Update("No return color", _returnView.Style, Color.Gray);
            else
                _returnView.Update(colors[_returnColor].ToString(), _returnView.Style, colors[_returnColor]);

            if (e.SavedData.Length != 0)
            {
                _curColor = (int)e.SavedData[0];
                UpdateHilight((Application)sender);
            }
        }

        void Application_KeyPress(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case KeyCode.Up:
                    _curColor = (_curColor + colors.Length - 1) % colors.Length;
                    UpdateHilight((Application)sender);
                    e.Handled = true;
                    break;
                case KeyCode.Down:
                    _curColor = (_curColor + colors.Length + 1) % colors.Length;
                    UpdateHilight((Application)sender);
                    e.Handled = true;
                    break;
                case KeyCode.Right:
                    byte[] savedData = new byte[1];
                    savedData[0] = (byte)_curColor;
                    TivoTree forwardParameters = new TivoTree();
                    forwardParameters.Add("entry", _curColor.ToString());
                    forwardParameters.Add("depth", _depth.ToString());
                    ((Application)sender).TransitionForward(BaseUri, forwardParameters, savedData);
                    e.Handled = true;
                    break;
                case KeyCode.Left:
                    TivoTree backParameters = new TivoTree();
                    backParameters.Add("return", _curColor.ToString());
                    backParameters.Add("depth", (_depth - 1).ToString());
                    ((Application)sender).TransitionBack(backParameters);
                    e.Handled = true;
                    break;
            }
        }

        void Application_ApplicationErrorOccurred(object sender, ApplicationErrorArgs e)
        {
            _errorView.Text = e.Text;
        }

        private void UpdateHilight(Application app)
        {
            int x = SafetyViewMargin.HorizontalActionMargin;
            int w = app.Root.Bounds.Width - 2 * x;
            int y = 180 + _curColor * 50 - 5;
            int h = 50;
            _hilightView.Animate(Animation.SetBounds(new Rectangle(x, y, w, h), 0, TimeSpan.FromMilliseconds(250)));
            _colorView.Update("The currently selected color is " + colors[_curColor], _colorView.Style, colors[_curColor]);
        }
    }
}
