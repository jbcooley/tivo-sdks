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
using System.Collections.ObjectModel;

namespace Tivo.Hme
{
    public class ViewCollection : Collection<View>
    {
        private View _owner;
        private TimeSpan _delay = TimeSpan.Zero;

        public ViewCollection(View owner)
        {
            _owner = owner;
        }

        public void Clear(TimeSpan delay)
        {
            _delay = delay;
            Clear();
        }

        public bool Remove(View item, TimeSpan delay)
        {
            _delay = delay;
            return Remove(item);
        }

        public void RemoveAt(int index, TimeSpan delay)
        {
            _delay = delay;
            RemoveAt(index);
        }

        protected override void ClearItems()
        {
            foreach (View view in Items)
            {
                _owner.PostCommand(new Commands.ViewRemove(view.ViewId, 0, _delay));
                view.SetParent(null, false);
            }
            _delay = TimeSpan.Zero;
            base.ClearItems();
        }

        protected override void InsertItem(int index, View item)
        {
            base.InsertItem(index, item);
            // ViewAdd command is a side effect of setting the parent
            item.SetParent(_owner, false);
        }

        protected override void RemoveItem(int index)
        {
            // send ViewRemove command when parent has not changed
            // this is the most likely senario
            // this will not be true if a view is reparented
            if (Items[index].Parent == _owner)
            {
                _owner.PostCommand(new Commands.ViewRemove(Items[index].ViewId, 0, _delay));
                Items[index].SetParent(null, false);
            }
            _delay = TimeSpan.Zero;
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, View item)
        {
            // send ViewRemove command when parent has not changed
            // this is the most likely senario
            // this will not be true if a view is reparented
            if (Items[index].Parent == _owner)
            {
                _owner.PostCommand(new Commands.ViewRemove(Items[index].ViewId));
                Items[index].SetParent(null, false);
            }
            base.SetItem(index, item);
            // ViewAdd command is a side effect of setting the parent
            item.SetParent(_owner, false);
        }
    }
}
