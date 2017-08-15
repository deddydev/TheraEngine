using System.Linq;

namespace System.Collections.Generic
{
    public class MonitoredList<T> : ThreadSafeList<T>
    {
        public delegate void SingleHandler(T item);
        public delegate void MultiHandler(IEnumerable<T> items);
        public delegate void SingleInsertHandler(T item, int index);
        public delegate void MultiInsertHandler(IEnumerable<T> items, int index);

        public event SingleHandler PreAdded;
        public event SingleHandler PostAdded;

        public event MultiHandler PreAddedRange;
        public event MultiHandler PostAddedRange;
        
        public event SingleHandler PreRemoved;
        public event SingleHandler PostRemoved;
        
        public event MultiHandler PreRemovedRange;
        public event MultiHandler PostRemovedRange;
        
        public event SingleInsertHandler PreInserted;
        public event SingleInsertHandler PostInserted;
        
        public event MultiInsertHandler PreInsertedRange;
        public event MultiInsertHandler PostInsertedRange;

        public event Action PreModified;
        public event Action PostModified;

        private bool _updating = false;
        private bool _allowDuplicates = true;

        public MonitoredList() { }
        public MonitoredList(bool allowDuplicates) => _allowDuplicates = allowDuplicates;
        public MonitoredList(IEnumerable<T> list) => AddRange(list);
        public MonitoredList(IEnumerable<T> list, bool allowDuplicates) : this(allowDuplicates) => AddRange(list);
        public MonitoredList(int capacity) : base(capacity) { }

        public new void Add(T item) => Add(item, true, true);
        public void Add(T item, bool reportAdded, bool reportModified)
        {
            if (!_allowDuplicates && Contains(item))
                return;

            if (!_updating)
            {
                if (reportAdded)
                    PreAdded?.Invoke(item);
                if (reportModified)
                    PreModified?.Invoke();
            }

            base.Add(item);

            if (!_updating)
            {
                if (reportAdded)
                    PostAdded?.Invoke(item);
                if (reportModified)
                    PostModified?.Invoke();
            }
        }
        public new void AddRange(IEnumerable<T> collection) => AddRange(collection, true, true);
        public void AddRange(IEnumerable<T> collection, bool reportAddedRange, bool reportModified)
        {
            if (collection == null)
                return;

            if (!_allowDuplicates)
               collection = collection.Where(x => !Contains(x));

            if (!_updating)
            {
                if (reportAddedRange)
                    PreAddedRange?.Invoke(collection);
                if (reportModified)
                    PreModified?.Invoke();
            }

            base.AddRange(collection);

            if (!_updating)
            {
                if (reportAddedRange)
                    PostAddedRange?.Invoke(collection);
                if (reportModified)
                    PostModified?.Invoke();
            }
        }
        public new bool Remove(T item) => Remove(item, true, true);
        public bool Remove(T item, bool reportRemoved, bool reportModified)
        {
            if (!_updating)
            {
                if (reportRemoved)
                    PreRemoved?.Invoke(item);
                if (reportModified)
                    PreModified?.Invoke();
            }

            if (base.Remove(item))
            {
                if (!_updating)
                {
                    if (reportRemoved)
                        PostRemoved?.Invoke(item);
                    if (reportModified)
                        PostModified?.Invoke();
                }
                return true;
            }
            return false;
        }
        public new void RemoveRange(int index, int count) => RemoveRange(index, count, true, true);
        public void RemoveRange(int index, int count, bool reportRemovedRange, bool reportModified)
        {
            IEnumerable<T> range = null;

            if (!_updating && reportRemovedRange)
                range = GetRange(index, count);

            if (!_updating)
            {
                if (reportRemovedRange)
                    PreRemovedRange?.Invoke(range);
                if (reportModified)
                    PreModified?.Invoke();
            }

            base.RemoveRange(index, count);

            if (!_updating)
            {
                if (reportRemovedRange)
                    PostRemovedRange?.Invoke(range);
                if (reportModified)
                    PostModified?.Invoke();
            }

        }
        public new void RemoveAt(int index) => RemoveAt(index, true, true);
        public void RemoveAt(int index, bool reportRemoved, bool reportModified)
        {
            T item = this[index];

            if (!_updating)
            {
                if (reportRemoved)
                    PreRemoved?.Invoke(item);
                if (reportModified)
                    PreModified?.Invoke();
            }

            base.RemoveAt(index);

            if (!_updating)
            {
                if (reportRemoved)
                    PostRemoved?.Invoke(item);
                if (reportModified)
                    PostModified?.Invoke();
            }
        }
        public new void Clear() => Clear(true, true);
        public void Clear(bool reportRemovedRange, bool reportModified)
        {
            IEnumerable<T> range = null;

            if (reportRemovedRange)
                range = GetRange(0, Count);

            if (!_updating)
            {
                if (reportRemovedRange)
                    PreRemovedRange?.Invoke(range);
                if (reportModified)
                    PreModified?.Invoke();
            }

            base.Clear();

            if (!_updating)
            {
                if (reportRemovedRange)
                    PostRemovedRange?.Invoke(range);
                if (reportModified)
                    PostModified?.Invoke();
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

                if (!_updating)
                {
                    if (reportRemovedRange)
                        PreRemovedRange?.Invoke(matches);
                    if (reportModified)
                        PreModified?.Invoke();
                }

                base.RemoveAll(match);

                if (reportRemovedRange)
                    PostRemovedRange?.Invoke(matches);
                if (reportModified)
                    PostModified?.Invoke();
            }
            else
                base.RemoveAll(match);
        }
        public new void Insert(int index, T item) => Insert(index, item);
        public void Insert(int index, T item, bool reportInserted, bool reportModified)
        {
            if (!_allowDuplicates && Contains(item))
                return;

            if (!_updating)
            {
                if (reportInserted)
                    PreInserted?.Invoke(item, index);
                if (reportModified)
                    PreModified?.Invoke();
            }

            base.Insert(index, item);

            if (!_updating)
            {
                if (reportInserted)
                    PostInserted?.Invoke(item, index);
                if (reportModified)
                    PostModified?.Invoke();
            }
        }
        public new void InsertRange(int index, IEnumerable<T> collection) => InsertRange(index, collection, true, true);
        public void InsertRange(int index, IEnumerable<T> collection, bool reportInsertedRange, bool reportModified)
        {
            if (collection == null)
                return;

            if (!_allowDuplicates)
                collection = collection.Where(x => !Contains(x));

            if (!_updating)
            {
                if (reportInsertedRange)
                    PreInsertedRange?.Invoke(collection, index);
                if (reportModified)
                    PreModified?.Invoke();
            }

            base.InsertRange(index, collection);

            if (!_updating)
            {
                if (reportInsertedRange)
                    PostInsertedRange?.Invoke(collection, index);
                if (reportModified)
                    PostModified?.Invoke();
            }
        }
        public new void Reverse(int index, int count) => Reverse(index, count, true);
        public void Reverse(int index, int count, bool reportModified)
        {
            bool report = reportModified && !_updating;
            if (report)
                PreModified?.Invoke();
            base.Reverse(index, count);
            if (report)
                PostModified?.Invoke();
        }
        public new void Reverse() => Reverse(true);
        public void Reverse(bool reportModified)
        {
            bool report = reportModified && !_updating;
            if (report)
                PreModified?.Invoke();
            base.Reverse();
            if (report)
                PostModified?.Invoke();
        }
        public new void Sort(int index, int count, IComparer<T> comparer) => Sort(index, count, comparer, true);
        public void Sort(int index, int count, IComparer<T> comparer, bool reportModified)
        {
            bool report = reportModified && !_updating;
            if (report)
                PreModified?.Invoke();
            base.Sort(index, count, comparer);
            if (report)
                PostModified?.Invoke();
        }
        public new void Sort() => Sort(true);
        public void Sort(bool reportModified)
        {
            bool report = reportModified && !_updating;
            if (report)
                PreModified?.Invoke();
            base.Sort();
            if (report)
                PostModified?.Invoke();
        }
        public new void Sort(IComparer<T> comparer) => Sort(comparer, true);
        public void Sort(IComparer<T> comparer, bool reportModified)
        {
            bool report = reportModified && !_updating;
            if (report)
                PreModified?.Invoke();
            base.Sort(comparer);
            if (report)
                PostModified?.Invoke();
        }
    }
}
