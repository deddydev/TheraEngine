using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    public class ThreadSafeList<T> : List<T>
    {
        protected bool _updating = false;
        protected readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public ThreadSafeList() : base() { }
        public ThreadSafeList(int capacity) : base(capacity) { }
        public ThreadSafeList(IEnumerable<T> list) : base(list) { }

        public new void Add(T item)
        {
            using (_lock.Write())
                base.Add(item);
        }
        public new void AddRange(IEnumerable<T> collection)
        {
            using (_lock.Write())
                base.AddRange(collection);
        }
        public new bool Remove(T item)
        {
            using (_lock.Write())
                return base.Remove(item);
        }
        public new void RemoveRange(int index, int count)
        {
            using (_lock.Write())
                base.RemoveRange(index, count);
        }
        public new void RemoveAt(int index)
        {
            using (_lock.Write())
                base.RemoveAt(index);
        }
        public new void Clear()
        {
            using (_lock.Write())
                base.Clear();
        }
        public new void RemoveAll(Predicate<T> match)
        {
            using (_lock.Write())
                base.RemoveAll(match);
        }
        public new void Insert(int index, T item)
        {
            using (_lock.Write())
                base.Insert(index, item);
        }
        public new void InsertRange(int index, IEnumerable<T> collection)
        {
            using (_lock.Write())
                base.InsertRange(index, collection);
        }
        public new ReadOnlyCollection<T> AsReadOnly()
        {
            using (_lock.Read())
                return base.AsReadOnly();
        }
        public new int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            using (_lock.Read())
                return base.BinarySearch(index, count, item, comparer);
        }
        public new int BinarySearch(T item)
        {
            using (_lock.Read())
                return base.BinarySearch(item);
        }
        public new int BinarySearch(T item, IComparer<T> comparer)
        {
            using (_lock.Read())
                return base.BinarySearch(item, comparer);
        }
        public new bool Contains(T item)
        {
            using (_lock.Read())
                return base.Contains(item);
        }
        public new List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
            using (_lock.Read())
                return base.ConvertAll(converter);
        }
        public new void CopyTo(T[] array, int arrayIndex)
        {
            using (_lock.Read())
                base.CopyTo(array, arrayIndex);
        }
        public new void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            using (_lock.Read())
                base.CopyTo(array, arrayIndex);
        }
        public new void CopyTo(T[] array)
        {
            using (_lock.Read())
                base.CopyTo(array);
        }
        public new bool Exists(Predicate<T> match)
        {
            using (_lock.Read())
                return base.Exists(match);
        }
        public new T Find(Predicate<T> match)
        {
            using (_lock.Read())
                return base.Find(match);
        }
        public new List<T> FindAll(Predicate<T> match)
        {
            using (_lock.Read())
                return base.FindAll(match);
        }
        public new int FindIndex(Predicate<T> match)
        {
            using (_lock.Read())
                return base.FindIndex(match);
        }
        public new int FindIndex(int startIndex, Predicate<T> match)
        {
            using (_lock.Read())
                return base.FindIndex(startIndex, match);
        }
        public new int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            using (_lock.Read())
                return base.FindIndex(startIndex, count, match);
        }
        public new T FindLast(Predicate<T> match)
        {
            using (_lock.Read())
                return base.FindLast(match);
        }
        public new int FindLastIndex(Predicate<T> match)
        {
            using (_lock.Read())
                return base.FindLastIndex(match);
        }
        public new int FindLastIndex(int startIndex, Predicate<T> match)
        {
            using (_lock.Read())
                return base.FindLastIndex(startIndex, match);
        }
        public new int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            using (_lock.Read())
                return base.FindLastIndex(startIndex, count, match);
        }
        public new void ForEach(Action<T> action)
        {
            using (_lock.Read())
                base.ForEach(action);
        }
        public new Enumerator GetEnumerator()
        {
            return new ThreadSafeEnumerator<T>(GetEnumerator(), _lock);
        }
        public new List<T> GetRange(int index, int count)
        {
            using (_lock.Read())
                return base.GetRange(index, count);
        }
        public new int IndexOf(T item, int index, int count)
        {
            using (_lock.Read())
                return base.IndexOf(item, index, count);
        }
        public new int IndexOf(T item, int index)
        {
            using (_lock.Read())
                return base.IndexOf(item, index);
        }
        public new int IndexOf(T item)
        {
            using (_lock.Read())
                return base.IndexOf(item);
        }
        public new int LastIndexOf(T item)
        {
            using (_lock.Read())
                return base.LastIndexOf(item);
        }
        public new int LastIndexOf(T item, int index)
        {
            using (_lock.Read())
                return base.LastIndexOf(item, index);
        }
        public new int LastIndexOf(T item, int index, int count)
        {
            using (_lock.Read())
                return base.LastIndexOf(item, index, count);
        }
        public new void Reverse(int index, int count)
        {
            using (_lock.Write())
                base.Reverse(index, count);
        }
        public new void Reverse()
        {
            using (_lock.Write())
                base.Reverse();
        }
        public new void Sort(int index, int count, IComparer<T> comparer)
        {
            using (_lock.Write())
                base.Sort(index, count, comparer);
        }
        public new void Sort(Comparison<T> comparison)
        {
            using (_lock.Write())
                base.Sort(comparison);
        }
        public new void Sort()
        {
            using (_lock.Write())
                base.Sort();
        }
        public new void Sort(IComparer<T> comparer)
        {
            using (_lock.Write())
                base.Sort(comparer);
        }
        public new T[] ToArray()
        {
            using (_lock.Read())
                return base.ToArray();
        }
        public new bool TrueForAll(Predicate<T> match)
        {
            using (_lock.Read())
                return base.TrueForAll(match);
        }
    }
}
