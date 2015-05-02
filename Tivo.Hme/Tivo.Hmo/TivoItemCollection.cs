using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Tivo.Hmo
{
    public struct TivoItemCollection : IList<TivoItem>, IReadOnlyList<TivoItem>
    {
        private XElement _itemsContainer;

        internal TivoItemCollection(XElement itemsContainer)
        {
            _itemsContainer = itemsContainer;
        }

        public TivoItem this[int index]
        {
            get
            {
                XElement tivoItem = _itemsContainer.Elements(Calypso16.Item).ElementAt(index);
                return ConvertToTivoItem(tivoItem);
            }
        }

        #region IList<TivoItem> Members

        public int IndexOf(TivoItem item)
        {
            int index = -1;
            _itemsContainer.Elements(Calypso16.Item)
                .Where((e, i) => { index = i; return item.ContentUrl == TivoItem.GetContentUrl(e); })
                .FirstOrDefault();
            return index;
        }

        void IList<TivoItem>.Insert(int index, TivoItem item)
        {
            throw new NotSupportedException();
        }

        void IList<TivoItem>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        TivoItem IList<TivoItem>.this[int index]
        {
            get { return this[index]; }
            set { throw new NotSupportedException(); }
        }

        #endregion

        #region ICollection<TivoItem> Members

        void ICollection<TivoItem>.Add(TivoItem item)
        {
            throw new NotSupportedException();
        }

        void ICollection<TivoItem>.Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(TivoItem item)
        {
            return _itemsContainer.Elements(Calypso16.Item)
                .Where(e => item.ContentUrl == TivoItem.GetContentUrl(e))
                .Any();
        }

        public void CopyTo(TivoItem[] array, int arrayIndex)
        {
            int currentIndex = arrayIndex;
            foreach (var item in this)
            {
                array[currentIndex] = item;
            }
        }

        public int Count
        {
            get { return _itemsContainer.Elements(Calypso16.Item).Count(); }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(TivoItem item)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IEnumerable<TivoItem> Members

        public IEnumerator<TivoItem> GetEnumerator()
        {
            foreach (var tivoItem in _itemsContainer.Elements(Calypso16.Item))
            {
                yield return ConvertToTivoItem(tivoItem);
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        private static TivoItem ConvertToTivoItem(XElement tivoItem)
        {
            string contentType = TivoItem.GetContentType(tivoItem);
            if (ContentTypes.IsContainer(contentType))
                return (TivoContainer)tivoItem;
            else if (ContentTypes.IsVideo(contentType))
                return (TivoVideo)tivoItem;
            else
                return new UnknownTivoItem(tivoItem);
        }
    }
}
