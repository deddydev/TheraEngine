using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    public class MonitoredList<T> : ThreadSafeList<T>
    {
        public delegate void SingleHandler(T item);
        public delegate void MultiHandler(IEnumerable<T> items);
        public delegate void SingleInsertHandler(T item, int index);
        public delegate void MultiInsertHandler(IEnumerable<T> items, int index);

        public event SingleHandler Added;
        public event MultiHandler AddedRange;
        public event SingleHandler Removed;
        public event MultiHandler RemovedRange;
        public event SingleInsertHandler Inserted;
        public event MultiInsertHandler InsertedRange;
        public event Action Modified;
        
        private bool _allowDuplicates = true;

        public MonitoredList() { }
        public MonitoredList(bool allowDuplicates) => _allowDuplicates = allowDuplicates;
        public MonitoredList(IEnumerable<T> list) => AddRange(list);
        public MonitoredList(IEnumerable<T> list, bool allowDuplicates) : this(allowDuplicates) => AddRange(list);

        public new void Add(T item) => Add(item, true, true);
        public void Add(T item, bool reportAdded, bool reportModified)
        {
            if (!_allowDuplicates && Contains(item))
                return;
            base.Add(item);
            if (!_updating)
            {
                if (reportAdded)
                    Added?.Invoke(item);
                if (reportModified)
                    Modified?.Invoke();
            }
        }
        public new void AddRange(IEnumerable<T> collection) => AddRange(collection, true, true);
        public void AddRange(IEnumerable<T> collection, bool reportAddedRange, bool reportModified)
        {
            if (collection == null)
                return;

            if (!_allowDuplicates)
               collection = collection.Where(x => !Contains(x));

            base.AddRange(collection);

            if (!_updating)
            {
                if (reportAddedRange)
                    AddedRange?.Invoke(collection);
                if (reportModified)
                    Modified?.Invoke();
            }
        }
        public new bool Remove(T item) => Remove(item, true, true);
        public bool Remove(T item, bool reportRemoved, bool reportModified)
        {
            if (base.Remove(item))
            {
                if (!_updating)
                {
                    if (reportRemoved)
                        Removed?.Invoke(item);
                    if (reportModified)
                        Modified?.Invoke();
                }
                return true;
            }
            return false;
        }
        public new void RemoveRange(int index, int count) => RemoveRange(index, count, true, true);
        public void RemoveRange(int index, int count, bool reportModified, bool reportRemovedRange)
        {
            IEnumerable<T> range = null;

            if (!_updating && reportRemovedRange)
                range = GetRange(index, count);

            base.RemoveRange(index, count);

            if (!_updating)
            {
                if (reportRemovedRange)
                    RemovedRange?.Invoke(range);
                if (reportModified)
                    Modified?.Invoke();
            }

        }
        public new void RemoveAt(int index) => RemoveAt(index, true, true);
        public void RemoveAt(int index, bool reportRemoved, bool reportModified)
        {
            T item = this[index];

            base.RemoveAt(index);

            if (!_updating)
            {
                if (reportRemoved)
                    Removed?.Invoke(item);
                if (reportModified)
                    Modified?.Invoke();
            }
        }
        public new void Clear() => Clear(true, true);
        public void Clear(bool reportRemovedRange, bool reportModified)
        {
            IEnumerable<T> range = null;

            if (reportRemovedRange)
                range = GetRange(0, Count);

            base.Clear();

            if (!_updating)
            {
                if (reportRemovedRange)
                    RemovedRange?.Invoke(range);
                if (reportModified)
                    Modified?.Invoke();
            }
        }
        public new void RemoveAll(Predicate<T> match) => RemoveAll(match, true, true);
        public void RemoveAll(Predicate<T> match, bool reportRemovedRange, bool reportModified)
        {
            if (!_updating)
            {
                IEnumerable<T> matches = null;

                if (reportRemovedRange)
                    matches = FindAll(match);

                base.RemoveAll(match);

                if (reportRemovedRange)
                    RemovedRange?.Invoke(matches);
                if (reportModified)
                    Modified?.Invoke();
            }
            else
                base.RemoveAll(match);
        }
        public new void Insert(int index, T item) => Insert(index, item);
        public void Insert(int index, T item, bool reportInserted, bool reportModified)
        {
            if (!_allowDuplicates && Contains(item))
                return;
            base.Insert(index, item);
            if (!_updating)
            {
                if (reportInserted)
                    Inserted?.Invoke(item, index);
                if (reportModified)
                    Modified?.Invoke();
            }
        }
        public new void InsertRange(int index, IEnumerable<T> collection) => InsertRange(index, collection, true, true);
        public void InsertRange(int index, IEnumerable<T> collection, bool reportInsertedRange, bool reportModified)
        {
            if (collection == null)
                return;

            if (!_allowDuplicates)
                collection = collection.Where(x => !Contains(x));

            base.InsertRange(index, collection);

            if (!_updating)
            {
                if (reportInsertedRange)
                    InsertedRange?.Invoke(collection, index);
                if (reportModified)
                    Modified?.Invoke();
            }
        }
        public new void Reverse(int index, int count) => Reverse(index, count, true);
        public void Reverse(int index, int count, bool reportModified)
        {
            base.Reverse(index, count);
            if (reportModified && !_updating)
                Modified?.Invoke();
        }
        public new void Reverse() => Reverse(true);
        public void Reverse(bool reportModified)
        {
            base.Reverse();
            if (reportModified && !_updating)
                Modified?.Invoke();
        }
        public new void Sort(int index, int count, IComparer<T> comparer) => Sort(index, count, comparer, true);
        public void Sort(int index, int count, IComparer<T> comparer, bool reportModified)
        {
            base.Sort(index, count, comparer);
            if (reportModified && !_updating)
                Modified?.Invoke();
        }
        public new void Sort() => Sort(true);
        public void Sort(bool reportModified)
        {
            base.Sort();
            if (reportModified && !_updating)
                Modified?.Invoke();
        }
        public new void Sort(IComparer<T> comparer) => Sort(comparer, true);
        public void Sort(IComparer<T> comparer, bool reportModified)
        {
            base.Sort(comparer);
            if (reportModified && !_updating)
                Modified?.Invoke();
        }
    }
}
