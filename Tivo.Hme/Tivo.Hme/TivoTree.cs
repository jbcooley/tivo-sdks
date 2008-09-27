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
using System.Text;
using System.Collections.ObjectModel;

namespace Tivo.Hme
{
    ///// <summary>
    ///// A parameter entry
    ///// </summary>
    //public class ParameterValue : IEquatable<ParameterValue>
    //{
    //    private string _value;

    //    public ParameterValue(string value)
    //    {
    //        Value = value;
    //        Children = new ParameterValueTree();
    //    }

    //    /// <summary>
    //    /// The parameter value
    //    /// </summary>
    //    public string Value
    //    {
    //        get { return _value; }
    //        set
    //        {
    //            if (value == null)
    //                throw new ArgumentNullException("value");
    //            if (value.Length == 0)
    //                // TODO: provide reason for exception
    //                // reason is an empty string is an end
    //                // of tree marker.  Can't store one internally
    //                throw new ArgumentException();
    //            _value = value;
    //        }
    //    }
    //    /// <summary>
    //    /// Child parameter values, if any.
    //    /// </summary>
    //    public ParameterValueTree Children { get; private set; }

    //    public System.Xml.XmlWriter CreateXmlWriter()
    //    {
    //        return null;
    //    }

    //    public System.Xml.XmlReader CreateXmlReader()
    //    {
    //        return null;
    //    }

    //    /// <summary>
    //    /// Transforms a string value to a ParameterValue with no children
    //    /// </summary>
    //    /// <param name="value">Any non-null and non-empty string value</param>
    //    /// <returns>A Parameter value with no children</returns>
    //    public static implicit operator ParameterValue(string value)
    //    {
    //        return new ParameterValue(value);
    //    }

    //    #region IEquatable<ParameterValue> Members

    //    public bool Equals(ParameterValue other)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    #endregion

    //    public override bool Equals(object obj)
    //    {
    //        Type argType = obj.GetType();
    //        if (argType != typeof(ParameterValue) &&
    //            argType != typeof(string))
    //            return false;
    //        if (argType == typeof(string))
    //            return Equals((ParameterValue)(string)obj);
    //        return Equals((ParameterValue)obj);
    //    }

    //    public override string ToString()
    //    {
    //        if (Children.Count == 0)
    //            return Value;
    //        StringBuilder builder = new StringBuilder();
    //        BuildString(builder);
    //        return builder.ToString();
    //    }

    //    private void BuildString(StringBuilder builder)
    //    {
    //        if (Children.Count == 0)
    //        {
    //            builder.Append(Value);
    //        }
    //        else
    //        {
    //            builder.Append(Value);
    //            builder.Append("{");
    //            bool needsSeperator = false;
    //            foreach (var entry in Children)
    //            {
    //                entry.BuildString(builder);
    //                if (needsSeperator)
    //                    builder.Append(",");
    //                needsSeperator = true;
    //            }
    //            builder.Append("}");
    //        }
    //    }
    //}

    ///// <summary>
    ///// Values for initializing an application
    ///// </summary>
    //public class ParameterValueTree : KeyedCollection<string, ParameterValue>
    //{
    //    protected override string GetKeyForItem(ParameterValue item)
    //    {
    //        return item.Value;
    //    }

    //    public void Add(string key, string value)
    //    {
    //        ParameterValue entry;
    //        if (TryGetValue(key, out entry))
    //        {
    //            entry.Children.Add(value);
    //        }
    //        else
    //        {
    //            entry = new ParameterValue(key);
    //            entry.Children.Add(value);
    //            Add(entry);
    //        }
    //    }

    //    public bool TryGetValue(string key, out ParameterValue value)
    //    {
    //        if (Dictionary != null)
    //        {
    //            return Dictionary.TryGetValue(key, out value);
    //        }
    //        foreach (var entry in Items)
    //        {
    //            if (entry.Value == key)
    //            {
    //                value = entry;
    //                return true;
    //            }
    //        }
    //        value = null;
    //        return false;
    //    }
    //}

    public class TivoTree : IEnumerable<string>
    {
        private SortedList<string, List<object>> _internalDictionary = new SortedList<string, List<object>>();

        public static TivoTree FromDictionary(IDictionary<string, string> dictionary)
        {
            TivoTree tree = new TivoTree();
            foreach (var entry in dictionary)
            {
                tree.Add(entry.Key, entry.Value);
            }
            return tree;
        }

        public static TivoTree FromNameValueCollection(System.Collections.Specialized.NameValueCollection collection)
        {
            TivoTree tree = new TivoTree();
            int count = collection.Count;
            for (int i = 0; i < count; ++i)
            {
                tree.AddRange(collection.GetKey(i), collection.GetValues(i));
            }
            return tree;
        }

        #region IEnumerable<string> Members

        public IEnumerator<string> GetEnumerator()
        {
            return _internalDictionary.Keys.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public TivoTreeValueCollection GetValues(string key)
        {
            var values = _internalDictionary[key];
            return new TivoTreeValueCollection(values);
        }

        public void Add(string key, string value)
        {
            List<object> values;
            if (_internalDictionary.TryGetValue(key, out values))
            {
                values.Add(value);
            }
            else
            {
                values = new List<object>();
                values.Add(value);
                _internalDictionary.Add(key, values);
            }
        }

        public void Add(string key, TivoTree value)
        {
            List<object> values;
            if (_internalDictionary.TryGetValue(key, out values))
            {
                values.Add(value);
            }
            else
            {
                values = new List<object>();
                values.Add(value);
                _internalDictionary.Add(key, values);
            }
        }

        internal void AddKey(string key)
        {
            _internalDictionary.Add(key, new List<object>());
        }

        private void AddRange(string key, IEnumerable<string> values)
        {
            List<object> internalValues;
            IEnumerable<object> convertedValues;
            if (values is string[])
            {
                object[] objValues = (string[])values;
                convertedValues = objValues;
            }
            else
            {
                convertedValues = Cast(values);
            }
            if (_internalDictionary.TryGetValue(key, out internalValues))
            {
                internalValues.AddRange(convertedValues);
            }
            else
            {
                internalValues = new List<object>();
                internalValues.AddRange(convertedValues);
                _internalDictionary.Add(key, internalValues);
            }
        }

        private IEnumerable<object> Cast(IEnumerable<string> values)
        {
            foreach (var item in values)
                yield return item;
        }

        public bool ContainsKey(string key)
        {
            return _internalDictionary.ContainsKey(key);
        }

        public void RemoveValueAt(string key, int valueIndex)
        {
            _internalDictionary[key].RemoveAt(valueIndex);
        }

        public void Remove(string key)
        {
            _internalDictionary.Remove(key);
        }

        public int GetValueCount(string key)
        {
            return _internalDictionary[key].Count;
        }

        public bool TryGetValue(string key, out TivoTreeValueCollection values)
        {
            values = null;
            List<object> objValues;
            if (_internalDictionary.TryGetValue(key, out objValues))
            {
                values = new TivoTreeValueCollection(objValues);
                return true;
            }
            return false;
        }

        public bool TryGetStringValue(string key, int index, out string value)
        {
            return TryGetValue<string>(key, index, out value);
        }

        public bool TryGetTivoTreeValue(string key, int index, out TivoTree value)
        {
            return TryGetValue<TivoTree>(key, index, out value);
        }

        private bool TryGetValue<T>(string key, int index, out T value)
        {
            value = default(T);
            List<object> values;
            if (!_internalDictionary.TryGetValue(key, out values) ||
                values.Count <= index)
                return false;
            object objValue = values[index];
            if (!(objValue is T))
                return false;
            value = (T)objValue;
            return true;
        }
    }

        public class TivoTreeValueCollection : IList<IEnumerable<string>>
        {
            private List<object> _values;

            public TivoTreeValueCollection(List<object> values)
            {
                _values = values;
            }

            #region IList<IEnumerable<string>> Members

            public int IndexOf(string item)
            {
                return _values.IndexOf(item);
            }

            public int IndexOf(TivoTree item)
            {
                return _values.IndexOf(item);
            }

            int IList<IEnumerable<string>>.IndexOf(IEnumerable<string> item)
            {
                if (item is TivoTree)
                {
                    return IndexOf((TivoTree)item);
                }
                else
                {
                    return IndexOf(GetFirstAndOnly(item));
                }
            }

            public void Insert(int index, string item)
            {
                _values.Insert(index, item);
            }

            public void Insert(int index, TivoTree item)
            {
                _values.Insert(index, item);
            }

            void IList<IEnumerable<string>>.Insert(int index, IEnumerable<string> item)
            {
                if (item is TivoTree)
                    Insert(index, (TivoTree)item);
                else
                    Insert(index, GetFirstAndOnly(item));
            }

            public void RemoveAt(int index)
            {
                _values.RemoveAt(index);
            }

            public object this[int index]
            {
                get
                {
                    return _values[index];
                }
                set
                {
                    if (value is string || value is TivoTree)
                        _values[index] = value;
                    else throw new ArgumentException();
                }
            }

            IEnumerable<string> IList<IEnumerable<string>>.this[int index]
            {
                get
                {
                    object item = this[index];
                    if (item is string)
                        return EnumerableStringHelper((string)item);
                    else
                        return (IEnumerable<string>)item;
                }
                set
                {
                    if (value is TivoTree)
                        this[index] = value;
                    else
                        this[index] = GetFirstAndOnly(value);
                }
            }

            #endregion

            #region ICollection<IEnumerable<string>> Members

            public void Add(string item)
            {
                _values.Add(item);
            }

            public void Add(TivoTree item)
            {
                _values.Add(item);
            }

            void ICollection<IEnumerable<string>>.Add(IEnumerable<string> item)
            {
                if (item is TivoTree)
                {
                    Add((TivoTree)item);
                }
                else
                {
                    Add(GetFirstAndOnly(item));
                }
            }

            public void Clear()
            {
                _values.Clear();
            }

            public bool Contains(string item)
            {
                return _values.Contains(item);
            }

            public bool Contains(TivoTree item)
            {
                return _values.Contains(item);
            }

            bool ICollection<IEnumerable<string>>.Contains(IEnumerable<string> item)
            {
                if (item is TivoTree)
                    return Contains((TivoTree)item);
                else
                    return Contains(GetFirstAndOnly(item));
            }

            void ICollection<IEnumerable<string>>.CopyTo(IEnumerable<string>[] array, int arrayIndex)
            {
                int index = arrayIndex;
                foreach (var item in this)
                {
                    if (index >= array.Length)
                        return;
                    array[index] = item;
                }
            }

            public int Count
            {
                get { return _values.Count; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public bool Remove(string item)
            {
                return _values.Remove(item);
            }

            public bool Remove(TivoTree item)
            {
                return _values.Remove(item);
            }

            bool ICollection<IEnumerable<string>>.Remove(IEnumerable<string> item)
            {
                if (item is TivoTree)
                    return Remove((TivoTree)item);
                else
                    return Remove(GetFirstAndOnly(item));
            }

            #endregion

            #region IEnumerable<IEnumerable<string>> Members

            public IEnumerator<IEnumerable<string>> GetEnumerator()
            {
                foreach (object item in _values)
                {
                    if (item is string)
                        yield return EnumerableStringHelper((string)item);
                    else
                        yield return (IEnumerable<string>)item;
                }
            }

            private static IEnumerable<string> EnumerableStringHelper(string value)
            {
                yield return value;
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion

            private static string GetFirstAndOnly(IEnumerable<string> items)
            {
                bool gotFirst = false;
                string first = null;
                foreach (var item in items)
                {
                    if (gotFirst)
                        throw new InvalidOperationException();
                    first = item;
                    gotFirst = true;
                }
                if (!gotFirst)
                    throw new InvalidOperationException();
                return first;
            }
        }

    // values are a list of strings and/or TivoTrees
    // if the dictionary is IEnumerable<string> for keys
    // then the values could each be IEnumerable<string> where
    // the single values are only one value
    // then need method for IEnumerable<IEnumerable<string>> GetValues(string key)

    // enumerable of ParameterValue
    // sorted dictionary - probably not real since that would expose object as value
    // string key, object value
    // overloads of Add
    // Add(string, string)
    // Add(string, dictionary)
    // Add(ParameterValue)
}
