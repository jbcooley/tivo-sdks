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
using System.Collections.ObjectModel;
using System.Drawing;
using System.ComponentModel;

namespace Tivo.Hme
{
    public class View : IDisposable, INotifyPropertyChanged
    {
        private const int RootId = 2;
        private long _id;
        private View _parent;
        private ViewCollection _children;
        private Rectangle _bounds;
        private Margin _margin;
        private bool _usingMargin;
        private Point _offset;
        private SizeF _scale;
        private bool _visible = true;
        private float _transparency;
        private long _resourceId;
        private bool _canFocus;

        private List<Commands.IHmeCommand> _queuedCommands;
        private Application _application;

        public View()
        {
            _children = new ViewCollection(this);
        }

        internal void MakeRoot(Application application)
        {
            // TODO: make this work to for margins
            // when the child is parented to a view
            // that is then made a root view
            ViewId = RootId;
            _bounds = new Rectangle(0, 0,
                (int)application.CurrentResolution.Horizontal, (int)application.CurrentResolution.Vertical);
            Application = application;
            ProcessNewApplication();
            PostCommand(new Commands.ViewSetVisible(ViewId, true));
        }

        public event EventHandler<KeyEventArgs> KeyDown;
        public event EventHandler<KeyEventArgs> KeyPress;
        public event EventHandler<KeyEventArgs> KeyUp;

        public long ViewId
        {
            get { return _id; }
            internal set { _id = value; }
        }

        public View Parent
        {
            get { return _parent; }
            set
            {
                SetParent(value, true);
            }
        }

        internal void SetParent(View newParent, bool fromPropertySetter)
        {
            bool changed = _parent != newParent;
            View oldParent = _parent;
            if (changed && _parent != null)
                _parent.PropertyChanged -= new PropertyChangedEventHandler(_parent_PropertyChanged);
            _parent = newParent;
            if (_parent != null && Application != _parent.Application)
            {
                Application = _parent.Application;
                ProcessNewApplication();
            }
            if (changed)
            {
                if (_parent != null)
                {
                    _parent.PropertyChanged += new PropertyChangedEventHandler(_parent_PropertyChanged);
                }
                if (fromPropertySetter)
                {
                    if (oldParent != null)
                        oldParent.Children.Remove(this);
                    if (_parent != null)
                        _parent.Children.Add(this);
                }
                OnPropertyChanged(new PropertyChangedEventArgs("Parent"));
            }
        }

        public ViewCollection Children
        {
            get { return _children; }
        }

        public Rectangle Bounds
        {
            get { return _bounds; }
            set
            {
                value = SetBounds(value, false);
            }
        }

        public Point Location
        {
            get { return _bounds.Location; }
            set
            {
                _usingMargin = false;
                bool changed = _bounds.Location != value;
                _bounds.Location = value;
                PostCommand(new Commands.ViewSetBounds(this));
                if (changed)
                    OnPropertyChanged(new PropertyChangedEventArgs("Location"));
            }
        }

        public Size Size
        {
            get { return _bounds.Size; }
            set
            {
                _usingMargin = false;
                bool changed = _bounds.Size != value;
                _bounds.Size = value;
                PostCommand(new Commands.ViewSetBounds(this));
                if (changed)
                    OnPropertyChanged(new PropertyChangedEventArgs("Size"));
            }
        }

        public Margin Margin
        {
            get { return _margin; }
            set
            {
                _usingMargin = true;
                bool changed = _margin != value;
                _margin = value;
                if (_parent != null && changed)
                {
                    AdjustBoundsForMargin();
                }
                if (changed)
                    OnPropertyChanged(new PropertyChangedEventArgs("Margin"));
            }
        }

        private void AdjustBoundsForMargin()
        {
            Rectangle newBounds = _parent.Bounds;
            newBounds.Size += new Size(-(_margin.Left + _margin.Right), -(_margin.Top + _margin.Bottom));
            if (newBounds.Size.Width >= 0 && newBounds.Size.Height >= 0)
            {
                newBounds.Offset(Margin.Left, Margin.Top);
                SetBounds(newBounds, true);
            }
            else
            {
                SetBounds(Rectangle.Empty, true);
            }
        }

        public Point Offset
        {
            get { return _offset; }
            set
            {
                bool changed = _offset != value;
                _offset = value;
                PostCommand(new Commands.ViewSetTranslation(this));
                if (changed)
                    OnPropertyChanged(new PropertyChangedEventArgs("Offset"));
            }
        }

        public SizeF Scale
        {
            get { return _scale; }
            set
            {
                bool changed = _scale != value;
                _scale = value;
                PostCommand(new Commands.ViewSetScale(this));
                if (changed)
                    OnPropertyChanged(new PropertyChangedEventArgs("Scale"));
            }
        }

        public bool Visible
        {
            get { return _visible; }
            set
            {
                bool changed = _visible != value;
                _visible = value;
                PostCommand(new Commands.ViewSetVisible(this));
                if (changed)
                    OnPropertyChanged(new PropertyChangedEventArgs("Visible"));
            }
        }

        public float Transparency
        {
            get { return _transparency; }
            set
            {
                bool changed = _transparency != value;
                _transparency = value;
                PostCommand(new Commands.ViewSetTransparency(this));
                if (changed)
                    OnPropertyChanged(new PropertyChangedEventArgs("Transparency"));
            }
        }

        public bool CanFocus
        {
            get { return _canFocus; }
            set
            {
                bool changed = _canFocus != value;
                _canFocus = value;
                if (changed)
                    OnPropertyChanged(new PropertyChangedEventArgs("CanFocus"));
            }
        }

        public bool Focused
        {
            get { return Application != null && Application.ActiveView == this; }
        }

        public void SuspendPainting()
        {
            PostCommand(new Commands.ViewSetPainting(ViewId, false));
        }

        public void ResumePainting()
        {
            PostCommand(new Commands.ViewSetPainting(ViewId, true));
        }

        protected internal long ResourceId
        {
            get { return _resourceId; }
            protected set{ _resourceId = value; }
        }

        public void Animate(Animation animation)
        {
            foreach (Commands.IViewCommand viewCommand in animation.Commands)
            {
                viewCommand.UseView(this);
                PostCommand(viewCommand);
            }
        }

        public bool Focus()
        {
            if (CanFocus && Application != null && Parent != null)
            {
                Application.ActiveView = this;
                return true;
            }
            return false;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && Application != null)
            {
                Application.ReleaseResourceId(ResourceId);
                foreach (View child in Children)
                {
                    child.Dispose();
                }
            }
        }

        protected Application Application
        {
            get { return _application; }
            set
            {
                if (_application != value)
                {
                    // unregister old property changed event handler
                    if (IsRoot && _application != null)
                    {
                        // this is primarily to keep root's bounds equal to the resolution
                        _application.PropertyChanged -= new PropertyChangedEventHandler(_application_PropertyChanged);
                    }
                    _application = value;
                    foreach (View child in Children)
                    {
                        child.Application = _application;
                    }
                    // register new property changed event handler
                    if (IsRoot && _application != null)
                    {
                        // this is primarily to keep root's bounds equal to the resolution
                        _application.PropertyChanged += new PropertyChangedEventHandler(_application_PropertyChanged);
                    }
                }
            }
        }

        protected virtual void OnNewApplication()
        {
        }

        internal void PostCommand(Commands.IHmeCommand command)
        {
            if (_application != null)
            {
                _application.PostCommand(command);
            }
            else
            {
                if (_queuedCommands == null)
                {
                    _queuedCommands = new List<Tivo.Hme.Commands.IHmeCommand>();
                }
                _queuedCommands.Add(command);
            }
        }

        internal void PostCommandBatch(IEnumerable<Commands.IHmeCommand> batch)
        {
            if (_application != null)
            {
                _application.PostCommandBatch(batch);
            }
            else
            {
                if (_queuedCommands == null)
                {
                    _queuedCommands = new List<Tivo.Hme.Commands.IHmeCommand>();
                }
                _queuedCommands.AddRange(batch);
            }
        }

        private void ProcessNewApplication()
        {
            if (Application != null)
            {
                if (!IsRoot)
                {
                    ViewId = Application.GetNewViewId();
                    PostCommand(new Commands.ViewAdd(ViewId, _parent, _bounds, _visible));
                }
                if (_usingMargin)
                    AdjustBoundsForMargin();
                if (_queuedCommands != null)
                {
                    _queuedCommands.ForEach(delegate(Commands.IHmeCommand command)
                    {
                        Commands.IViewCommand viewCommand = command as Commands.IViewCommand;
                        if (viewCommand != null && viewCommand.ViewId == 0) viewCommand.UseView(this);
                    });
                    Application.PostCommandBatch(_queuedCommands);
                    _queuedCommands = null;
                }
                OnNewApplication();
                foreach (View child in Children)
                {
                    child.ProcessNewApplication();
                }
            }
        }

        protected internal virtual void OnKeyDown(KeyEventArgs e)
        {
            EventHandler<KeyEventArgs> handler = KeyDown;
            if (handler != null)
                handler(this, e);
            if (!e.Handled && Parent != null)
                Parent.OnKeyDown(e);
        }

        protected internal virtual void OnKeyPress(KeyEventArgs e)
        {
            EventHandler<KeyEventArgs> handler = KeyPress;
            if (handler != null)
                handler(this, e);
            if (!e.Handled && Parent != null)
                Parent.OnKeyPress(e);
        }

        protected internal virtual void OnKeyUp(KeyEventArgs e)
        {
            EventHandler<KeyEventArgs> handler = KeyUp;
            if (handler != null)
                handler(this, e);
            if (!e.Handled && Parent != null)
                Parent.OnKeyUp(e);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, e);
        }

        void _parent_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Bounds":
                case "Location":
                case "Size":
                    if (_parent != null && _usingMargin)
                        AdjustBoundsForMargin();
                    break;
            }
        }

        void _application_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CurrentResolution":
                    if (IsRoot)
                    {
                        Bounds = new Rectangle(0, 0,
                            (int)Application.CurrentResolution.Horizontal, (int)Application.CurrentResolution.Vertical);
                    }
                    break;
            }
        }

        private Rectangle SetBounds(Rectangle value, bool usingMargin)
        {
            usingMargin = false;
            bool changedBounds = _bounds != value;
            bool changedLocation = _bounds.Location != value.Location;
            bool changedSize = _bounds.Size != value.Size;
            _bounds = value;
            PostCommand(new Commands.ViewSetBounds(this));
            if (changedBounds)
                OnPropertyChanged(new PropertyChangedEventArgs("Bounds"));
            if (changedLocation)
                OnPropertyChanged(new PropertyChangedEventArgs("Location"));
            if (changedSize)
                OnPropertyChanged(new PropertyChangedEventArgs("Size"));
            return value;
        }

        private bool IsRoot
        {
            get { return _id == RootId; }
        }
    }
}
