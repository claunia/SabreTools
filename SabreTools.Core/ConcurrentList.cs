using System;
using System.Collections;
using System.Collections.Generic;

namespace SabreTools.Core
{
    /// <summary>
    /// Thread-safe list class
    /// </summary>
    public class ConcurrentList<T> : ICollection<T>, IEnumerable<T>, IEnumerable, IList<T>, IReadOnlyCollection<T>, IReadOnlyList<T>, ICollection, IList
    {
        private List<T> _list = new();
        private readonly object _lock = new();

        public T this[int index]
        {
            get { lock (_lock) return _list[index]; }
            set { lock (_lock) _list[index] = value; }
        }

        object IList.this[int index]
        {
            get { lock (_lock) return ((IList)_list)[index]; }
            set { lock (_lock) ((IList)_list)[index] = value; }
        }

        public int Count
        {
            get { lock (_lock) return _list.Count; }
        }

        public bool IsFixedSize => ((IList)_list).IsFixedSize;

        public bool IsReadOnly => ((IList)_list).IsReadOnly;

        public bool IsSynchronized => ((ICollection)_list).IsSynchronized;

        public object SyncRoot => ((ICollection)_list).SyncRoot;

        public void Add(T item)
        {
            lock (_lock)
                _list.Add(item);
        }

        public int Add(object value)
        {
            lock (_lock)
                return ((IList)_list).Add(value);
        }

        public void AddRange(IEnumerable<T> values)
        {
            lock (_lock)
                _list.AddRange(values);
        }

        public void Clear()
        {
            lock (_lock)
                _list.Clear();
        }

        public bool Contains(T item)
        {
            lock (_lock)
                return _list.Contains(item);
        }

        public bool Contains(object value)
        {
            lock (_lock)
                return ((IList)_list).Contains(value);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (_lock)
                _list.CopyTo(array, arrayIndex);
        }

        public void CopyTo(Array array, int index)
        {
            lock (_lock)
                ((ICollection)_list).CopyTo(array, index);
        }

        public void ForEach(Action<T> action)
        {
            lock (_lock)
                _list.ForEach(action);
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (_lock)
                return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (_lock)
                return ((IEnumerable)_list).GetEnumerator();
        }

        public int IndexOf(T item)
        {
            lock (_lock)
                return _list.IndexOf(item);
        }

        public int IndexOf(object value)
        {
            lock (_lock)
                return ((IList)_list).IndexOf(value);
        }

        public void Insert(int index, T item)
        {
            lock (_lock)
                _list.Insert(index, item);
        }

        public void Insert(int index, object value)
        {
            lock (_lock)
                ((IList)_list).Insert(index, value);
        }

        public bool Remove(T item)
        {
            lock (_lock)
                return _list.Remove(item);
        }

        public void Remove(object value)
        {
            lock (_lock)
                ((IList)_list).Remove(value);
        }

        public void RemoveAt(int index)
        {
            lock (_lock)
                _list.RemoveAt(index);
        }

        public void SetInternalList(List<T> list)
        {
            lock (_lock)
                _list = list;
        }

        public void Sort(Comparison<T> comparison)
        {
            lock (_lock)
                _list.Sort(comparison);
        }
    }
}
