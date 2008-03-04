using System;
using System.Collections.Generic;
using System.Text;

namespace Tivo.Hme.Samples
{

    public abstract class BaseList<T> : View, IList<T>
    {
        private List<T> _items = new List<T>();
        private List<View> _rows = new List<View>();

        // geometry
        private int _rowHeight;
        private int _numberOfVisibleRows;

        // selector view that sits behind the rows
        private View _selector;

        // the current "top" row displayed within the list. As the list scrolls down
        // top will move down as well.
        private int _top = 0;

        // the currently selected row
        private int _selected = -1;

        // when elements are added in the middle of the visible screen, the "dirty"
        // index keeps track of which rows must be moved up.
        private int _dirty = -1;

        public BaseList(int rowHeight)
        {
            _rowHeight = rowHeight;
            CanFocus = true;
        }

        protected override void OnNewApplication()
        {
            _numberOfVisibleRows = Bounds.Height / _rowHeight;
            base.OnNewApplication();

            _selector = CreateSelector(Bounds.Width, _rowHeight);
            _selector.Visible = false;
        }

        protected abstract View CreateSelector(int width, int height);
        protected abstract View CreateRow(int index);

        public int SelectedIndex
        {
            get { return _selected; }
            set { Select(value, true); }
        }

        public EventHandler<EventArgs> SelectionChanged;

        protected void OnSelectionChanged()
        {
            EventHandler<EventArgs> handler = SelectionChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected int RowHeight
        {
            get { return _rowHeight; }
        }

        private void Select(int newSelected, bool animate)
        {
            if (Count == 0)
            {
                _selected = -1;
            }
            else if (newSelected < 0)
            {
                _selected = 0;
            }
            else if (newSelected >= Count)
            {
                _selected = Count - 1;
            }
            else
            {
                _selected = newSelected;
            }
            Update(animate);
            OnSelectionChanged();
        }

        protected override void OnKeyPress(KeyEventArgs e)
        {
            int delta = 0;
            switch (e.KeyCode)
            {
                case KeyCode.Up:
                    delta = -1;
                    break;
                case KeyCode.Down:
                    delta = 1;
                    break;
                case KeyCode.ChannelUp:
                    delta = -1 * _numberOfVisibleRows;
                    break;
                case KeyCode.ChannelDown:
                    delta = _numberOfVisibleRows;
                    break;
            }
            if (delta != 0)
            {
                if (SelectedIndex == -1)
                {
                    Application.GetSound("bonk").Play();
                }
                else
                {
                    Application.GetSound("updown").Play();

                    bool animate = true;
                    int max = Count - 1;
                    int newSelected = SelectedIndex + delta;
                    if (newSelected < 0)
                    {
                        // scrolled above the first row in the list. Either jump to
                        // the first row or wrap around to the LAST row.
                        if (SelectedIndex != 0)
                        {
                            newSelected = 0;
                        }
                        else
                        {
                            animate = false;
                            newSelected = max;
                        }
                    }
                    else if (newSelected > max)
                    {
                        // scrolled below the last row in the list. Either jump to
                        // the last row or wrap around to the FIRST row.
                        if (SelectedIndex != max)
                        {
                            newSelected = max;
                        }
                        else
                        {
                            animate = false;
                            newSelected = 0;
                        }
                    }
                    else
                    {
                        // If the user hit channel up/down, move the top of the list
                        // too.
                        if (e.KeyCode == KeyCode.ChannelUp ||
                            e.KeyCode == KeyCode.ChannelDown)
                        {
                            _top += delta;
                        }
                    }
                    Select(newSelected, animate);
                }
                e.Handled = true;
            }
            base.OnKeyPress(e);
        }

        private void Update()
        {
            Update(false);
        }

        private static readonly TimeSpan animTime = TimeSpan.FromMilliseconds(200);
        private void Update(bool animate)
        {
            using (SuspendPainting suspend = new SuspendPainting(this))
            {
                if (_selected == -1 && Count != 0)
                {
                    _selected = 0;
                    OnSelectionChanged();
                }

                //
                // 1. update "top" so that selected is still visible
                //
                _top = Math.Max(Math.Min(_top, Count - _numberOfVisibleRows), 0);

                int max = Math.Min(_top + _numberOfVisibleRows, Count) - 1;
                if (_selected < _top)
                {
                    _top = Math.Max(0, _selected);
                }
                else if (_selected > max)
                {
                    int end = Math.Min(_selected + 1, Count);
                    _top = Math.Max(end - _numberOfVisibleRows, 0);
                }
                max = Math.Min(_top + _numberOfVisibleRows, Count) - 1;

                //
                // 2. populate rows and put them at the correct height
                //

                for (int i = _top; i <= max; ++i)
                {
                    View v = _rows[i];
                    if (v == null)
                    {
                        v = CreateRow(i);
                        _rows[i] = v;
                    }
                    else
                    {
                        v.Location = new System.Drawing.Point(v.Location.X, i * _rowHeight);
                    }
                }

                //
                // 3. fix "dirty" rows
                //

                if (_dirty != -1)
                {
                    max = Math.Min(_dirty + _numberOfVisibleRows, Count) - 1;
                    for (int i = _dirty; i <= max; ++i)
                    {
                        View v = _rows[i];
                        if (v != null)
                        {
                            v.Location = new System.Drawing.Point(v.Location.X, i * _rowHeight);
                        }
                    }
                    _dirty = -1;
                }

                //
                // 4. translate to the right location
                //

                this.Animate(Animation.Translate(new System.Drawing.Point(0, -_top * _rowHeight), 0, animTime));

                //
                // 5. move the selector
                //

                if (_selected >= 0)
                {
                    _selector.Visible = true;
                    _selector.Animate(Animation.Move(new System.Drawing.Point(0, _selected * _rowHeight), 0, animTime));
                }
                else
                {
                    _selector.Visible = false;
                }
            }
        }

        #region IList<T> Members

        public int IndexOf(T item)
        {
            int index = _items.IndexOf(item);
            Update();
            return index;
        }

        public void Insert(int index, T item)
        {
            Insert0(index, item);
            Update();
        }

        public void RemoveAt(int index)
        {
            RemoveAt0(index);
            Update();
        }

        public T this[int index]
        {
            get { return _items[index]; }
            set
            {
                Children.Remove(_rows[index]);
                _rows[index].Dispose();
                _rows[index] = null;
                _items[index] = value;
                Update();
            }
        }

        #endregion

        #region ICollection<T> Members

        public void Add(T item)
        {
            Insert0(Count, item);
            Update();
        }

        public void Clear()
        {
            foreach (View row in _rows)
            {
                Children.Remove(row);
                row.Dispose();
            }
            _rows.Clear();
            _items.Clear();

            _selected = -1;
            _top = 0;
            _dirty = -1;

            Update();
            OnSelectionChanged();
        }

        public bool Contains(T item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index != -1)
                RemoveAt(index);
            return index != -1;
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        #endregion

        public void AddRange(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                Add(item);
            }
        }

        // add0/remove0 are low level helprs that update selected/top/dirty but
        // don't call update()

        private void Insert0(int index, T item)
        {
            bool selectionChanged = false;
            if (index <= _selected)
            {
                ++_selected;
                selectionChanged = true;
            }
            if (index <= _top)
            {
                ++_top;
            }
            // update the  dirty index
            if (_dirty == -1)
            {
                _dirty = index;
            }
            else if (index <= _dirty)
            {
                ++_dirty;
            }

            _items.Insert(index, item);
            _rows.Insert(index, null);
            if (selectionChanged)
                OnSelectionChanged();
        }

        void RemoveAt0(int index)
        {
            bool selectionChanged = false;
            if (index < _selected)
            {
                --_selected;
                selectionChanged = true;
            }
            if (index < _top)
            {
                --_top;
            }

            Children.Remove(_rows[index]);
            _rows[index].Dispose();
            _rows.RemoveAt(index);
            _items.RemoveAt(index);
            if (selectionChanged)
                OnSelectionChanged();
        }
    }

}
