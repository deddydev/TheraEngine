using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    public class ThreadSafeList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IList, ICollection, IReadOnlyList<T>, IReadOnlyCollection<T>
    {
        protected readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        protected List<T> _inner;
        
        public int Count => ((IList<T>)_inner).Count;
        public bool IsReadOnly => ((IList<T>)_inner).IsReadOnly;
        public bool IsFixedSize => ((IList)_inner).IsFixedSize;
        public object SyncRoot => ((IList)_inner).SyncRoot;
        public bool IsSynchronized => ((IList)_inner).IsSynchronized;
        object IList.this[int index]
        {
            get
            {
                using (_lock.Read())
                    return ((IList)_inner)[index];
            }
            set
            {
                using (_lock.Write())
                    ((IList)_inner)[index] = value;
            }
        }
        public T this[int index]
        {
            get
            {
                using (_lock.Read())
                    return ((IList<T>)_inner)[index];
            }
            set
            {
                using (_lock.Write())
                    ((IList<T>)_inner)[index] = value;
            }
        }

        public ThreadSafeList() : base()
        {
            _inner = new List<T>();
        }
        public ThreadSafeList(int capacity)
        {
            _inner = new List<T>(capacity);
        }
        public ThreadSafeList(IEnumerable<T> list)
        {
            _inner = new List<T>(list);
        }

        public void Add(T item)
        {
            using (_lock.Write())
                _inner.Add(item);
        }
        public void AddRange(IEnumerable<T> collection)
        {
            using (_lock.Write())
                _inner.AddRange(collection);
        }
        public bool Remove(T item)
        {
            using (_lock.Write())
                return _inner.Remove(item);
        }
        public void RemoveRange(int index, int count)
        {
            using (_lock.Write())
                _inner.RemoveRange(index, count);
        }
        public void RemoveAt(int index)
        {
            using (_lock.Write())
                _inner.RemoveAt(index);
        }
        public void Clear()
        {
            using (_lock.Write())
                _inner.Clear();
        }
        public void RemoveAll(Predicate<T> match)
        {
            using (_lock.Write())
                _inner.RemoveAll(match);
        }
        public void Insert(int index, T item)
        {
            using (_lock.Write())
                _inner.Insert(index, item);
        }
        public void InsertRange(int index, IEnumerable<T> collection)
        {
            using (_lock.Write())
                _inner.InsertRange(index, collection);
        }
        public ReadOnlyCollection<T> AsReadOnly()
        {
            using (_lock.Read())
                return _inner.AsReadOnly();
        }
        public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            using (_lock.Read())
                return _inner.BinarySearch(index, count, item, comparer);
        }
        public int BinarySearch(T item)
        {
            using (_lock.Read())
                return _inner.BinarySearch(item);
        }
        public int BinarySearch(T item, IComparer<T> comparer)
        {
            using (_lock.Read())
                return _inner.BinarySearch(item, comparer);
        }
        public bool Contains(T item)
        {
            using (_lock.Read())
                return _inner.Contains(item);
        }
        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
            using (_lock.Read())
                return _inner.ConvertAll(converter);
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            using (_lock.Read())
                _inner.CopyTo(array, arrayIndex);
        }
        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            using (_lock.Read())
                _inner.CopyTo(array, arrayIndex);
        }
        public void CopyTo(T[] array)
        {
            using (_lock.Read())
                _inner.CopyTo(array);
        }
        public bool Exists(Predicate<T> match)
        {
            using (_lock.Read())
                return _inner.Exists(match);
        }
        public T Find(Predicate<T> match)
        {
            using (_lock.Read())
                return _inner.Find(match);
        }
        public List<T> FindAll(Predicate<T> match)
        {
            using (_lock.Read())
                return _inner.FindAll(match);
        }
        public int FindIndex(Predicate<T> match)
        {
            using (_lock.Read())
                return _inner.FindIndex(match);
        }
        public int FindIndex(int startIndex, Predicate<T> match)
        {
            using (_lock.Read())
                return _inner.FindIndex(startIndex, match);
        }
        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            using (_lock.Read())
                return _inner.FindIndex(startIndex, count, match);
        }
        public T FindLast(Predicate<T> match)
        {
            using (_lock.Read())
                return _inner.FindLast(match);
        }
        public int FindLastIndex(Predicate<T> match)
        {
            using (_lock.Read())
                return _inner.FindLastIndex(match);
        }
        public int FindLastIndex(int startIndex, Predicate<T> match)
        {
            using (_lock.Read())
                return _inner.FindLastIndex(startIndex, match);
        }
        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            using (_lock.Read())
                return _inner.FindLastIndex(startIndex, count, match);
        }
        public void ForEach(Action<T> action)
        {
            using (_lock.Read())
                _inner.ForEach(action);
        }
        public IEnumerator<T> GetEnumerator()
            => new ThreadSafeEnumerator<T>(_inner.GetEnumerator(), _lock);
        public List<T> GetRange(int index, int count)
        {
            using (_lock.Read())
                return _inner.GetRange(index, count);
        }
        public int IndexOf(T item, int index, int count)
        {
            using (_lock.Read())
                return _inner.IndexOf(item, index, count);
        }
        public int IndexOf(T item, int index)
        {
            using (_lock.Read())
                return _inner.IndexOf(item, index);
        }
        public int IndexOf(T item)
        {
            using (_lock.Read())
                return _inner.IndexOf(item);
        }
        public int LastIndexOf(T item)
        {
            using (_lock.Read())
                return _inner.LastIndexOf(item);
        }
        public int LastIndexOf(T item, int index)
        {
            using (_lock.Read())
                return _inner.LastIndexOf(item, index);
        }
        public int LastIndexOf(T item, int index, int count)
        {
            using (_lock.Read())
                return _inner.LastIndexOf(item, index, count);
        }
        public void Reverse(int index, int count)
        {
            using (_lock.Write())
                _inner.Reverse(index, count);
        }
        public void Reverse()
        {
            using (_lock.Write())
                _inner.Reverse();
        }
        public void Sort(int index, int count, IComparer<T> comparer)
        {
            using (_lock.Write())
                _inner.Sort(index, count, comparer);
        }
        public void Sort(Comparison<T> comparison)
        {
            using (_lock.Write())
                _inner.Sort(comparison);
        }
        public void Sort()
        {
            using (_lock.Write())
                _inner.Sort();
        }
        public void Sort(IComparer<T> comparer)
        {
            using (_lock.Write())
                _inner.Sort(comparer);
        }
        public T[] ToArray()
        {
            using (_lock.Read())
                return _inner.ToArray();
        }
        public bool TrueForAll(Predicate<T> match)
        {
            using (_lock.Read())
                return _inner.TrueForAll(match);
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
        
        public int Add(object value)
        {
            using (_lock.Write())
                return ((IList)_inner).Add(value);
        }
        public bool Contains(object value)
        {
            using (_lock.Read())
                return ((IList)_inner).Contains(value);
        }
        public int IndexOf(object value)
        {
            using (_lock.Read())
                return ((IList)_inner).IndexOf(value);
        }
        public void Insert(int index, object value)
        {
            using (_lock.Write())
                ((IList)_inner).Insert(index, value);
        }
        public void Remove(object value)
        {
            using (_lock.Write())
                ((IList)_inner).Remove(value);
        }
        public void CopyTo(Array array, int index)
        {
            using (_lock.Read())
                ((IList)_inner).CopyTo(array, index);
        }
    }
}
