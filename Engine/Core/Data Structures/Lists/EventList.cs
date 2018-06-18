using System.Collections.Specialized;
using System.Linq;

namespace System.Collections.Generic
{
    /// <summary>
    /// A derivation of <see cref="ThreadSafeList{T}"/> that monitors all operations and provides events for each kind of operation.
    /// </summary>
    public class EventList<T> : ThreadSafeList<T>, IList, INotifyCollectionChanged
    {
        public delegate void SingleHandler(T item);
        public delegate void MultiHandler(IEnumerable<T> items);
        public delegate void SingleInsertHandler(T item, int index);
        public delegate void MultiInsertHandler(IEnumerable<T> items, int index);

        /// <summary>
        /// Event called for every individual item just before being added to the list.
        /// </summary>
        public event SingleHandler PreAnythingAdded;
        /// <summary>
        /// Event called for every individual item after being added to the list.
        /// </summary>
        public event SingleHandler PostAnythingAdded;
        /// <summary>
        /// Event called for every individual item just before being removed from the list.
        /// </summary>
        public event SingleHandler PreAnythingRemoved;
        /// <summary>
        /// Event called for every individual item after being removed from the list.
        /// </summary>
        public event SingleHandler PostAnythingRemoved;
        /// <summary>
        /// Event called before an item is added using the Add method.
        /// </summary>
        public event SingleHandler PreAdded;
        /// <summary>
        /// Event called after an item is added using the Add method.
        /// </summary>
        public event SingleHandler PostAdded;
        /// <summary>
        /// Event called before an item is added using the AddRange method.
        /// </summary>
        public event MultiHandler PreAddedRange;
        /// <summary>
        /// Event called after an item is added using the AddRange method.
        /// </summary>
        public event MultiHandler PostAddedRange;
        /// <summary>
        /// Event called before an item is removed using the Remove method.
        /// </summary>
        public event SingleHandler PreRemoved;
        /// <summary>
        /// Event called after an item is removed using the Remove method.
        /// </summary>
        public event SingleHandler PostRemoved;
        /// <summary>
        /// Event called before an item is removed using the RemoveRange method.
        /// </summary>
        public event MultiHandler PreRemovedRange;
        /// <summary>
        /// Event called after an item is removed using the RemoveRange method.
        /// </summary>
        public event MultiHandler PostRemovedRange;
        /// <summary>
        /// Event called before an item is inserted using the Insert method.
        /// </summary>
        public event SingleInsertHandler PreInserted;
        /// <summary>
        /// Event called after an item is removed using the Insert method.
        /// </summary>
        public event SingleInsertHandler PostInserted;
        /// <summary>
        /// Event called before an item is inserted using the InsertRange method.
        /// </summary>
        public event MultiInsertHandler PreInsertedRange;
        /// <summary>
        /// Event called after an item is inserted using the InsertRange method.
        /// </summary>
        public event MultiInsertHandler PostInsertedRange;
        /// <summary>
        /// Event called before this list is modified in any way at all.
        /// </summary>
        public event Action PreModified;
        /// <summary>
        /// Event called after this list is modified in any way at all.
        /// </summary>
        public event Action PostModified;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private bool _updating = false;
        private bool _allowDuplicates = true;

        public EventList() { }
        public EventList(bool allowDuplicates) => _allowDuplicates = allowDuplicates;
        public EventList(IEnumerable<T> list) => AddRange(list);
        public EventList(IEnumerable<T> list, bool allowDuplicates) : this(allowDuplicates) => AddRange(list);
        public EventList(int capacity) : base(capacity) { }
        
        /// <summary>
        /// Completely replaces the list's items with the given items.
        /// </summary>
        /// <param name="items">The items to set as the collection.</param>
        /// <param name="reportRemoved">If true, notifies subscribers that previous items were removed.</param>
        /// <param name="reportAdded">If true, notifies subscribers that new items have been added.</param>
        /// <param name="reportModified">If true, notifies subscribers that the list has changed.</param>
        public void Set(IEnumerable<T> items, bool reportRemoved = true, bool reportAdded = true, bool reportModified = true)
        {
            Clear(reportRemoved, false);
            AddRange(items, reportAdded, reportModified);
        }

        public new void Add(T item) => Add(item, true, true);
        public void Add(T item, bool reportAdded, bool reportModified)
        {
            if (!_allowDuplicates && Contains(item))
                return;

            if (!_updating)
            {
                if (reportAdded)
                {
                    PreAdded?.Invoke(item);
                    PreAnythingAdded?.Invoke(item);
                }
                if (reportModified)
                    PreModified?.Invoke();
            }

            base.Add(item);

            if (!_updating)
            {
                if (reportAdded)
                {
                    PostAdded?.Invoke(item);
                    PostAnythingAdded?.Invoke(item);
                }
                if (reportModified)
                {
                    PostModified?.Invoke();
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add));
                }
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
                {
                    PreAddedRange?.Invoke(collection);
                    if (PreAnythingAdded != null)
                        foreach (T item in collection)
                            PreAnythingAdded(item);
                }
                if (reportModified)
                    PreModified?.Invoke();
            }

            base.AddRange(collection);

            if (!_updating)
            {
                if (reportAddedRange)
                {
                    PostAddedRange?.Invoke(collection);
                    if (PostAnythingAdded != null)
                        foreach (T item in collection)
                            PostAnythingAdded(item);
                }
                if (reportModified)
                {
                    PostModified?.Invoke();
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add));
                }
            }
        }
        public new bool Remove(T item) => Remove(item, true, true);
        public bool Remove(T item, bool reportRemoved, bool reportModified)
        {
            if (!_updating)
            {
                if (reportRemoved)
                {
                    PreRemoved?.Invoke(item);
                    PreAnythingRemoved?.Invoke(item);
                }
                if (reportModified)
                    PreModified?.Invoke();
            }

            if (base.Remove(item))
            {
                if (!_updating)
                {
                    if (reportRemoved)
                    {
                        PostRemoved?.Invoke(item);
                        PostAnythingRemoved?.Invoke(item);
                    }
                    if (reportModified)
                    {
                        PostModified?.Invoke();
                        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove));
                    }
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
                {
                    PreRemovedRange?.Invoke(range);
                    if (PreAnythingRemoved != null)
                        foreach (T item in range)
                            PreAnythingRemoved(item);
                }
                if (reportModified)
                    PreModified?.Invoke();
            }

            base.RemoveRange(index, count);

            if (!_updating)
            {
                if (reportRemovedRange)
                {
                    PostRemovedRange?.Invoke(range);
                    if (PostAnythingRemoved != null)
                        foreach (T item in range)
                            PostAnythingRemoved(item);
                }
                if (reportModified)
                {
                    PostModified?.Invoke();
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove));
                }
            }

        }
        public new void RemoveAt(int index) => RemoveAt(index, true, true);
        public void RemoveAt(int index, bool reportRemoved, bool reportModified)
        {
            T item = this[index];

            if (!_updating)
            {
                if (reportRemoved)
                {
                    PreRemoved?.Invoke(item);
                    PreAnythingRemoved?.Invoke(item);
                }
                if (reportModified)
                    PreModified?.Invoke();
            }

            base.RemoveAt(index);

            if (!_updating)
            {
                if (reportRemoved)
                {
                    PostRemoved?.Invoke(item);
                    PostAnythingRemoved?.Invoke(item);
                }
                if (reportModified)
                {
                    PostModified?.Invoke();
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove));
                }
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
                {
                    PreRemovedRange?.Invoke(range);
                    if (PreAnythingRemoved != null)
                        foreach (T item in range)
                            PreAnythingRemoved(item);
                }
                if (reportModified)
                    PreModified?.Invoke();
            }

            base.Clear();

            if (!_updating)
            {
                if (reportRemovedRange)
                {
                    PostRemovedRange?.Invoke(range);
                    if (PostAnythingRemoved != null)
                        foreach (T item in range)
                            PostAnythingRemoved(item);
                }
                if (reportModified)
                {
                    PostModified?.Invoke();
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
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
                    {
                        PreRemovedRange?.Invoke(matches);
                        if (PreAnythingRemoved != null)
                            foreach (T item in matches)
                                PreAnythingRemoved(item);
                    }
                    if (reportModified)
                        PreModified?.Invoke();
                }

                base.RemoveAll(match);

                if (reportRemovedRange)
                {
                    PostRemovedRange?.Invoke(matches);
                    if (PostAnythingRemoved != null)
                        foreach (T item in matches)
                            PostAnythingRemoved(item);
                }
                if (reportModified)
                {
                    PostModified?.Invoke();
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove));
                }
            }
            else
                base.RemoveAll(match);
        }
        public new void Insert(int index, T item) => Insert(index, item, true, true);
        public void Insert(int index, T item, bool reportInserted, bool reportModified)
        {
            if (!_allowDuplicates && Contains(item))
                return;

            if (!_updating)
            {
                if (reportInserted)
                {
                    PreInserted?.Invoke(item, index);
                    PreAnythingRemoved?.Invoke(item);
                }
                if (reportModified)
                    PreModified?.Invoke();
            }

            base.Insert(index, item);

            if (!_updating)
            {
                if (reportInserted)
                {
                    PostInserted?.Invoke(item, index);
                    PostAnythingRemoved?.Invoke(item);
                }
                if (reportModified)
                {
                    PostModified?.Invoke();
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add));
                }
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
                {
                    PreInsertedRange?.Invoke(collection, index);
                    if (PreAnythingRemoved != null)
                        foreach (T item in collection)
                            PreAnythingRemoved(item);
                }
                if (reportModified)
                    PreModified?.Invoke();
            }

            base.InsertRange(index, collection);

            if (!_updating)
            {
                if (reportInsertedRange)
                {
                    PostInsertedRange?.Invoke(collection, index);
                    if (PostAnythingRemoved != null)
                        foreach (T item in collection)
                            PostAnythingRemoved(item);
                }
                if (reportModified)
                {
                    PostModified?.Invoke();
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove));
                }
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
            {
                PostModified?.Invoke();
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move));
            }
        }
        public new void Reverse() => Reverse(true);
        public void Reverse(bool reportModified)
        {
            bool report = reportModified && !_updating;
            if (report)
                PreModified?.Invoke();
            base.Reverse();
            if (report)
            {
                PostModified?.Invoke();
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move));
            }
        }
        public new void Sort(int index, int count, IComparer<T> comparer) => Sort(index, count, comparer, true);
        public void Sort(int index, int count, IComparer<T> comparer, bool reportModified)
        {
            bool report = reportModified && !_updating;
            if (report)
                PreModified?.Invoke();
            base.Sort(index, count, comparer);
            if (report)
            {
                PostModified?.Invoke();
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move));
            }
        }
        public new void Sort() => Sort(true);
        public void Sort(bool reportModified)
        {
            bool report = reportModified && !_updating;
            if (report)
                PreModified?.Invoke();
            base.Sort();
            if (report)
            {
                PostModified?.Invoke();
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move));
            }
        }
        public new void Sort(IComparer<T> comparer) => Sort(comparer, true);
        public void Sort(IComparer<T> comparer, bool reportModified)
        {
            bool report = reportModified && !_updating;
            if (report)
                PreModified?.Invoke();
            base.Sort(comparer);
            if (report)
            {
                PostModified?.Invoke();
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move));
            }
        }
        public new T this[int index]
        {
            get => base[index];
            set
            {
                PreModified?.Invoke();
                base[index] = value;
                PostModified?.Invoke();
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace));
            }
        }
        object IList.this[int index]
        {
            get => this[index];
            set => this[index] = (T)value;
        }
        bool IList.IsReadOnly => false;
        bool IList.IsFixedSize => false;
        int IList.Add(object value)
        {
            Add((T)value);
            return Count - 1;
        }
        void IList.Clear() => Clear();
        bool IList.Contains(object value) => Contains((T)value);
        int IList.IndexOf(object value) => IndexOf((T)value);
        void IList.Insert(int index, object value) => Insert(index, (T)value);
        void IList.Remove(object value) => Remove((T)value);
        void IList.RemoveAt(int index) => RemoveAt(index);
    }
}
