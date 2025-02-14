﻿using System.Linq;
using System.Threading;
using TheraEngine;
using TheraEngine.Core;

namespace System.Collections.Generic
{
    public interface IEventList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IList, ICollection, IReadOnlyList<T>, IReadOnlyCollection<T>
    {
        new int Count { get; }

        event EventList<T>.SingleCancelableHandler PreAnythingAdded;
        event EventList<T>.SingleHandler PostAnythingAdded;
        event EventList<T>.SingleCancelableHandler PreAnythingRemoved;
        event EventList<T>.SingleHandler PostAnythingRemoved;
        event EventList<T>.SingleCancelableHandler PreAdded;
        event EventList<T>.SingleHandler PostAdded;
        event EventList<T>.MultiCancelableHandler PreAddedRange;
        event EventList<T>.MultiHandler PostAddedRange;
        event EventList<T>.SingleCancelableHandler PreRemoved;
        event EventList<T>.SingleHandler PostRemoved;
        event EventList<T>.MultiCancelableHandler PreRemovedRange;
        event EventList<T>.MultiHandler PostRemovedRange;
        event EventList<T>.SingleCancelableInsertHandler PreInserted;
        event EventList<T>.SingleInsertHandler PostInserted;
        event EventList<T>.MultiCancelableInsertHandler PreInsertedRange;
        event EventList<T>.MultiInsertHandler PostInsertedRange;
        event Func<bool> PreModified;
        event Action PostModified;
        event EventList<T>.PreIndexSetHandler PreIndexSet;
        event EventList<T>.PostIndexSetHandler PostIndexSet;
        event TCollectionChangedEventHandler<T> CollectionChanged;

        new T this[int index] { get; set; }

        void Set(IEnumerable<T> items, bool reportRemoved = true, bool reportAdded = true, bool reportModified = true);

        bool Add(T item, bool reportAdded, bool reportModified);
        new bool Add(T item);

        void AddRange(IEnumerable<T> collection, bool reportAddedRange, bool reportModified);
        void AddRange(IEnumerable<T> collection);

        bool Remove(T item, bool reportRemoved, bool reportModified);
        new bool Remove(T item);

        void RemoveRange(int index, int count, bool reportRemovedRange, bool reportModified);
        void RemoveRange(int index, int count);

        void RemoveAt(int index, bool reportRemoved, bool reportModified);
        void RemoveAt(int index);

        void Clear(bool reportRemovedRange, bool reportModified);
        void Clear();

        void RemoveAll(Predicate<T> match, bool reportRemovedRange, bool reportModified);
        void RemoveAll(Predicate<T> match);

        void Insert(int index, T item, bool reportInserted, bool reportModified);
        void Insert(int index, T item);
        
        void InsertRange(int index, IEnumerable<T> collection, bool reportInsertedRange, bool reportModified);
        void InsertRange(int index, IEnumerable<T> collection);

        void Reverse(int index, int count, bool reportModified);
        void Reverse(int index, int count);

        void Reverse(bool reportModified);
        void Reverse();

        void Sort(int index, int count, IComparer<T> comparer, bool reportModified);
        void Sort(int index, int count, IComparer<T> comparer);

        void Sort(bool reportModified);
        void Sort();

        void Sort(IComparer<T> comparer, bool reportModified);
        void Sort(IComparer<T> comparer);
    }

    /// <summary>
    /// A derivation of <see cref="ThreadSafeList{T}"/> that monitors all operations and provides events for each kind of operation.
    /// </summary>
    public class EventList<T> : TObjectSlim, IEventList<T>
    {
        private readonly List<T> _list;

        public delegate void SingleHandler(T item);
        public delegate bool SingleCancelableHandler(T item);

        public delegate void MultiHandler(IEnumerable<T> items);
        public delegate bool MultiCancelableHandler(IEnumerable<T> items);

        public delegate void SingleInsertHandler(T item, int index);
        public delegate bool SingleCancelableInsertHandler(T item, int index);

        public delegate void MultiInsertHandler(IEnumerable<T> items, int index);
        public delegate bool MultiCancelableInsertHandler(IEnumerable<T> items, int index);

        public delegate bool PreIndexSetHandler(int index, T newItem);
        public delegate void PostIndexSetHandler(int index, T prevItem);

        /// <summary>
        /// Event called for every individual item just before being added to the list.
        /// </summary>
        public event SingleCancelableHandler PreAnythingAdded;
        /// <summary>
        /// Event called for every individual item after being added to the list.
        /// </summary>
        public event SingleHandler PostAnythingAdded;
        /// <summary>
        /// Event called for every individual item just before being removed from the list.
        /// </summary>
        public event SingleCancelableHandler PreAnythingRemoved;
        /// <summary>
        /// Event called for every individual item after being removed from the list.
        /// </summary>
        public event SingleHandler PostAnythingRemoved;
        /// <summary>
        /// Event called before an item is added using the Add method.
        /// </summary>
        public event SingleCancelableHandler PreAdded;
        /// <summary>
        /// Event called after an item is added using the Add method.
        /// </summary>
        public event SingleHandler PostAdded;
        /// <summary>
        /// Event called before an item is added using the AddRange method.
        /// </summary>
        public event MultiCancelableHandler PreAddedRange;
        /// <summary>
        /// Event called after an item is added using the AddRange method.
        /// </summary>
        public event MultiHandler PostAddedRange;
        /// <summary>
        /// Event called before an item is removed using the Remove method.
        /// </summary>
        public event SingleCancelableHandler PreRemoved;
        /// <summary>
        /// Event called after an item is removed using the Remove method.
        /// </summary>
        public event SingleHandler PostRemoved;
        /// <summary>
        /// Event called before an item is removed using the RemoveRange method.
        /// </summary>
        public event MultiCancelableHandler PreRemovedRange;
        /// <summary>
        /// Event called after an item is removed using the RemoveRange method.
        /// </summary>
        public event MultiHandler PostRemovedRange;
        /// <summary>
        /// Event called before an item is inserted using the Insert method.
        /// </summary>
        public event SingleCancelableInsertHandler PreInserted;
        /// <summary>
        /// Event called after an item is removed using the Insert method.
        /// </summary>
        public event SingleInsertHandler PostInserted;
        /// <summary>
        /// Event called before an item is inserted using the InsertRange method.
        /// </summary>
        public event MultiCancelableInsertHandler PreInsertedRange;
        /// <summary>
        /// Event called after an item is inserted using the InsertRange method.
        /// </summary>
        public event MultiInsertHandler PostInsertedRange;
        /// <summary>
        /// Event called before this list is modified in any way at all.
        /// </summary>
        public event Func<bool> PreModified;
        /// <summary>
        /// Event called after this list is modified in any way at all.
        /// </summary>
        public event Action PostModified;

        public event PreIndexSetHandler PreIndexSet;
        public event PostIndexSetHandler PostIndexSet;

        public event TCollectionChangedEventHandler<T> CollectionChanged;

        public bool _updating = false;
        public bool _allowDuplicates = true;
        public bool _allowNull = true;

        internal bool AllowDuplicates
        {
            get => _allowDuplicates;
            set => _allowDuplicates = value;
        }
        internal bool AllowNull
        {
            get => _allowNull;
            set => _allowNull = value;
        }

        public EventList()
        {
            _list = new List<T>();
        }
        public EventList(bool allowDuplicates, bool allowNull)
        {
            _list = new List<T>();
            _allowDuplicates = allowDuplicates;
            _allowNull = allowNull;
        }
        public EventList(IEnumerable<T> list)
        {
            _list = new List<T>();
            AddRange(list);
        }
        public EventList(IEnumerable<T> list, bool allowDuplicates, bool allowNull)
            : this(allowDuplicates, allowNull) => AddRange(list);
        public EventList(int capacity)
        {
            _list = new List<T>(capacity);
        }

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

        public bool Add(T item) => Add(item, true, true);
        public bool Add(T item, bool reportAdded, bool reportModified)
        {
            if (!_allowNull && item == null)
                return false;

            if (!_allowDuplicates && Contains(item))
                return false;

            if (!_updating)
            {
                if (reportAdded)
                {
                    if (!(PreAdded?.Invoke(item) ?? true))
                        return false;

                    if (!(PreAnythingAdded?.Invoke(item) ?? true))
                        return false;
                }
                if (reportModified)
                {
                    if (!(PreModified?.Invoke() ?? true))
                        return false;
                }
            }

            try
            {
                //_locker?.EnterWriteLock();
                _list.Add(item);
            }
            //catch (Exception ex)
            //{
            //    Engine.LogException(ex);
            //}
            finally
            {
                //_locker?.ExitWriteLock();
            }

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
                    CollectionChanged?.Invoke(this, new TCollectionChangedEventArgs<T>(ECollectionChangedAction.Add, item));
                }
            }

            return true;
        }

        public bool Contains(T item)
        {
            try
            {
                //_locker?.EnterReadLock();
                return _list.Contains(item);
            }      
            //catch (Exception ex)
            //{
            //    Engine.LogException(ex);
            //    return false;
            //}
            finally
            {
                //_locker?.ExitReadLock();
            }
        }
        public void AddRange(IEnumerable<T> collection) => AddRange(collection, true, true);
        public void AddRange(IEnumerable<T> collection, bool reportAddedRange, bool reportModified)
        {
            if (collection is null)
                return;

            if (!_allowDuplicates)
                collection = collection.Where(x => !Contains(x));
            if (!_allowNull)
                collection = collection.Where(x => x != null);

            if (!_updating)
            {
                if (reportModified)
                {
                    if (!(PreModified?.Invoke() ?? true))
                        return;
                }
                if (reportAddedRange)
                {
                    if (!(PreAddedRange?.Invoke(collection) ?? true))
                        return;

                    if (PreAnythingAdded != null)
                        foreach (T item in collection)
                            if (!PreAnythingAdded(item))
                                collection = collection.Where(x => !ReferenceEquals(x, item));
                }
            }

            try
            {
                //_locker?.EnterWriteLock();
                _list.AddRange(collection);
            }
            //catch (Exception ex)
            //{
            //    Engine.LogException(ex);
            //}
            finally
            {
                //_locker?.ExitWriteLock();
            }

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
                    CollectionChanged?.Invoke(this, new TCollectionChangedEventArgs<T>(ECollectionChangedAction.Add, collection.ToList()));
                }
            }
        }
        public bool Remove(T item) => Remove(item, true, true);
        public bool Remove(T item, bool reportRemoved, bool reportModified)
        {
            if (!_updating)
            {
                if (reportModified)
                {
                    if (!(PreModified?.Invoke() ?? true))
                        return false;
                }
                if (reportRemoved)
                {
                    if (!(PreRemoved?.Invoke(item) ?? true))
                        return false;

                    if (!(PreAnythingRemoved?.Invoke(item) ?? true))
                        return false;
                }
            }

            bool success;
            try
            {
                //_locker?.EnterWriteLock();
                success = _list.Remove(item);
            }
            //catch (Exception ex)
            //{
            //    Engine.LogException(ex);
            //}
            finally
            {
                //_locker?.ExitWriteLock();
            }

            if (success)
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
                        CollectionChanged?.Invoke(this, new TCollectionChangedEventArgs<T>(ECollectionChangedAction.Remove, item));
                    }
                }
                return true;
            }
            return false;
        }
        public void RemoveRange(int index, int count) => RemoveRange(index, count, true, true);
        public void RemoveRange(int index, int count, bool reportRemovedRange, bool reportModified)
        {
            IEnumerable<T> range = null;

            if (!_updating && reportRemovedRange)
                range = GetRange(index, count);

            if (!_updating)
            {
                if (reportModified)
                {
                    if (!(PreModified?.Invoke() ?? true))
                        return;
                }
                if (reportRemovedRange)
                {
                    if (!(PreRemovedRange?.Invoke(range) ?? true))
                        return;

                    if (PreAnythingRemoved != null)
                        foreach (T item in range)
                            if (!PreAnythingRemoved(item))
                                range = range.Where(x => !ReferenceEquals(x, item));
                }
            }

            try
            {
                //_locker?.EnterWriteLock();
                _list.RemoveRange(index, count);
            }
            //catch (Exception ex)
            //{
            //    Engine.LogException(ex);
            //}
            finally
            {
                //_locker?.ExitWriteLock();
            }

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
                    CollectionChanged?.Invoke(this, new TCollectionChangedEventArgs<T>(ECollectionChangedAction.Remove, range.ToList()));
                }
            }

        }

        public IEnumerable<T> GetRange(int index, int count) => _list.GetRange(index, count);

        public void RemoveAt(int index) => RemoveAt(index, true, true);
        public void RemoveAt(int index, bool reportRemoved, bool reportModified)
        {
            T item = this[index];

            if (!_updating)
            {
                if (reportModified)
                {
                    if (!(PreModified?.Invoke() ?? true))
                        return;
                }
                if (reportRemoved)
                {
                    if (!(PreRemoved?.Invoke(item) ?? true))
                        return;

                    if (!(PreAnythingRemoved?.Invoke(item) ?? true))
                        return;
                }
            }

            try
            {
                //_locker?.EnterWriteLock();
                _list.RemoveAt(index);
            }
            //catch (Exception ex)
            //{
            //    Engine.LogException(ex);
            //}
            finally
            {
                //_locker?.ExitWriteLock();
            }

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
                    CollectionChanged?.Invoke(this, new TCollectionChangedEventArgs<T>(ECollectionChangedAction.Remove, item));
                }
            }
        }
        public void Clear() => Clear(true, true);
        public void Clear(bool reportRemovedRange, bool reportModified)
        {
            IEnumerable<T> range = null;

            if (reportRemovedRange)
                range = GetRange(0, Count);

            if (!_updating)
            {
                if (reportModified)
                {
                    if (!(PreModified?.Invoke() ?? true))
                        return;
                }
                if (reportRemovedRange)
                {
                    if (!(PreRemovedRange?.Invoke(range) ?? true))
                        return;

                    if (PreAnythingRemoved != null)
                        foreach (T item in range)
                            if (!PreAnythingRemoved(item))
                                range = range.Where(x => !ReferenceEquals(x, item));
                }
            }

            try
            {
                //_locker?.EnterWriteLock();
                _list.Clear();
            }
            //catch (Exception ex)
            //{
            //    Engine.LogException(ex);
            //}
            finally
            {
                //_locker?.ExitWriteLock();
            }

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
                    CollectionChanged?.Invoke(this, new TCollectionChangedEventArgs<T>(ECollectionChangedAction.Clear));
                }
            }
        }
        public void RemoveAll(Predicate<T> match) => RemoveAll(match, true, true);
        public void RemoveAll(Predicate<T> match, bool reportRemovedRange, bool reportModified)
        {
            IEnumerable<T> matches = null;

            if (!_updating)
            {
                if (reportRemovedRange)
                    matches = FindAll(match);

                if (!_updating)
                {
                    if (reportModified)
                    {
                        if (!(PreModified?.Invoke() ?? true))
                            return;
                    }
                    if (reportRemovedRange)
                    {
                        if (!(PreRemovedRange?.Invoke(matches) ?? true))
                            return;

                        if (PreAnythingRemoved != null)
                            foreach (T item in matches)
                                if (!PreAnythingRemoved(item))
                                    matches = matches.Where(x => !ReferenceEquals(x, item));
                    }
                }
            }

            try
            {
                //_locker?.EnterWriteLock();
                _list.RemoveAll(match);
            }
            //catch (Exception ex)
            //{
            //    Engine.LogException(ex);
            //}
            finally
            {
                //_locker?.ExitWriteLock();
            }

            if (!_updating)
            {
                if (reportRemovedRange)
                {
                    PostRemovedRange?.Invoke(matches);
                    if (PostAnythingRemoved != null && matches != null)
                        foreach (T item in matches)
                            PostAnythingRemoved(item);
                }
                if (reportModified)
                {
                    PostModified?.Invoke();
                    CollectionChanged?.Invoke(this, new TCollectionChangedEventArgs<T>(ECollectionChangedAction.Remove, matches.ToArray()));
                }
            }
        }

        public IEnumerable<T> FindAll(Predicate<T> match)
        {
            try
            {
                //_locker?.EnterReadLock();
                return _list.FindAll(match);
            }
            //catch (Exception ex)
            //{
            //    Engine.LogException(ex);
            //    return default;
            //}
            finally
            {
                //_locker?.ExitReadLock();
            }
        }

        public void Insert(int index, T item) => Insert(index, item, true, true);
        public void Insert(int index, T item, bool reportInserted, bool reportModified)
        {
            if (!_allowNull && item == null)
                return;

            if (!_allowDuplicates && Contains(item))
                return;

            if (!_updating)
            {
                if (reportModified)
                {
                    if (!(PreModified?.Invoke() ?? true))
                        return;
                }
                if (reportInserted)
                {
                    if (!(PreInserted?.Invoke(item, index) ?? true))
                        return;

                    if (!(PreAnythingAdded?.Invoke(item) ?? true))
                        return;
                }
            }

            try
            {
                //_locker?.EnterWriteLock();
                _list.Insert(index, item);
            }
            //catch (Exception ex)
            //{
            //    Engine.LogException(ex);
            //}
            finally
            {
                //_locker?.ExitWriteLock();
            }

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
                    CollectionChanged?.Invoke(this, new TCollectionChangedEventArgs<T>(ECollectionChangedAction.Add, item));
                }
            }
        }
        public void InsertRange(int index, IEnumerable<T> collection) => InsertRange(index, collection, true, true);
        public void InsertRange(int index, IEnumerable<T> collection, bool reportInsertedRange, bool reportModified)
        {
            if (collection is null)
                return;

            if (!_allowDuplicates)
                collection = collection.Where(x => !Contains(x));
            if (!_allowNull)
                collection = collection.Where(x => x != null);

            if (!_updating)
            {
                if (reportModified)
                {
                    if (!(PreModified?.Invoke() ?? true))
                        return;
                }
                if (reportInsertedRange)
                {
                    if (!(PreInsertedRange?.Invoke(collection, index) ?? true))
                        return;

                    if (PreAnythingAdded != null)
                        foreach (T item in collection)
                            if (!PreAnythingAdded(item))
                                collection = collection.Where(x => !ReferenceEquals(x, item));
                }
            }

            try
            {
                //_locker?.EnterWriteLock();
                _list.InsertRange(index, collection);
            }
            //catch (Exception ex)
            //{
            //    Engine.LogException(ex);
            //}
            finally
            {
                //_locker?.ExitWriteLock();
            }

            if (!_updating)
            {
                if (reportInsertedRange)
                {
                    PostInsertedRange?.Invoke(collection, index);
                    if (PostAnythingAdded != null)
                        foreach (T item in collection)
                            PostAnythingAdded(item);
                }
                if (reportModified)
                {
                    PostModified?.Invoke();
                    CollectionChanged?.Invoke(this, new TCollectionChangedEventArgs<T>(ECollectionChangedAction.Remove, collection.ToList()));
                }
            }
        }
        public void Reverse(int index, int count) => Reverse(index, count, true);
        public void Reverse(int index, int count, bool reportModified)
        {
            bool report = reportModified && !_updating;
            if (report)
            {
                if (!(PreModified?.Invoke() ?? true))
                    return;
            }
            try
            {
                //_locker?.EnterWriteLock();
                _list.Reverse(index, count);
            }
            //catch (Exception ex)
            //{
            //    Engine.LogException(ex);
            //}
            finally
            {
                //_locker?.ExitWriteLock();
            }
            if (report)
            {
                PostModified?.Invoke();
                CollectionChanged?.Invoke(this, new TCollectionChangedEventArgs<T>(ECollectionChangedAction.Move, this));
            }
        }
        public void Reverse() => Reverse(true);
        public void Reverse(bool reportModified)
        {
            bool report = reportModified && !_updating;
            if (report)
            {
                if (!(PreModified?.Invoke() ?? true))
                    return;
            }
            try
            {
                //_locker?.EnterWriteLock();
                _list.Reverse();
            }
            //catch (Exception ex)
            //{
            //    Engine.LogException(ex);
            //}
            finally
            {
                //_locker?.ExitWriteLock();
            }
            if (report)
            {
                PostModified?.Invoke();
                CollectionChanged?.Invoke(this, new TCollectionChangedEventArgs<T>(ECollectionChangedAction.Move, this));
            }
        }
        public void Sort(int index, int count, IComparer<T> comparer) => Sort(index, count, comparer, true);
        public void Sort(int index, int count, IComparer<T> comparer, bool reportModified)
        {
            bool report = reportModified && !_updating;
            if (report)
            {
                if (!(PreModified?.Invoke() ?? true))
                    return;
            }
            try
            {
                //_locker?.EnterWriteLock();
                _list.Sort(index, count, comparer);
            }
            //catch (Exception ex)
            //{
            //    Engine.LogException(ex);
            //}
            finally
            {
                //_locker?.ExitWriteLock();
            }
            if (report)
            {
                PostModified?.Invoke();
                CollectionChanged?.Invoke(this, new TCollectionChangedEventArgs<T>(ECollectionChangedAction.Move, this));
            }
        }
        public void Sort() => Sort(true);
        public void Sort(bool reportModified)
        {
            bool report = reportModified && !_updating;
            if (report)
            {
                if (!(PreModified?.Invoke() ?? true))
                    return;
            }
            try
            {
                //_locker?.EnterWriteLock();
                _list.Sort();
            }
            //catch (Exception ex)
            //{
            //    Engine.LogException(ex);
            //}
            finally
            {
                //_locker?.ExitWriteLock();
            }
            if (report)
            {
                PostModified?.Invoke();
                CollectionChanged?.Invoke(this, new TCollectionChangedEventArgs<T>(ECollectionChangedAction.Move, this));
            }
        }
        public void Sort(IComparer<T> comparer) => Sort(comparer, true);
        public void Sort(IComparer<T> comparer, bool reportModified)
        {
            bool report = reportModified && !_updating;
            if (report)
            {
                if (!(PreModified?.Invoke() ?? true))
                    return;
            }
            try
            {
                //_locker?.EnterWriteLock();
                _list.Sort(comparer);
            }
            //catch (Exception ex)
            //{
            //    Engine.LogException(ex);
            //}
            finally
            {
                //_locker?.ExitWriteLock();
            }
            if (report)
            {
                PostModified?.Invoke();
                CollectionChanged?.Invoke(this, new TCollectionChangedEventArgs<T>(ECollectionChangedAction.Move, this));
            }
        }
        public T this[int index]
        {
            get
            {
                try
                {
                    //_locker?.EnterReadLock();
                    return _list[index];
                }
                //catch (Exception ex)
                //{
                //    Engine.LogException(ex);
                //    return default;
                //}
                finally
                {
                    //_locker?.ExitReadLock();
                }
            }
            set
            {
                if (!_allowNull && value == null)
                    return;
                if (!_allowDuplicates && Contains(value))
                    return;
                if (!_updating)
                {
                    if (!(PreModified?.Invoke() ?? true))
                        return;
                    if (!(PreAdded?.Invoke(value) ?? true))
                        return;
                    if (!(PreAnythingAdded?.Invoke(value) ?? true))
                        return;
                    if (!(PreIndexSet?.Invoke(index, value) ?? true))
                        return;
                }
                T prev;
                try
                {
                    //_locker?.EnterWriteLock();
                    prev = _list[index];
                    _list[index] = value;
                }
                //catch (Exception ex)
                //{
                //    Engine.LogException(ex);
                //}
                finally
                {
                    //_locker?.ExitWriteLock();
                }
                if (!_updating)
                {
                    PostAdded?.Invoke(value);
                    PostAnythingAdded?.Invoke(value);
                    PostIndexSet?.Invoke(index, prev);
                    PostModified?.Invoke();
                    CollectionChanged?.Invoke(this, new TCollectionChangedEventArgs<T>(ECollectionChangedAction.Replace, value, index));
                }
            }
        }
        object IList.this[int index]
        {
            get => this[index];
            set => this[index] = (T)value;
        }
        bool IList.IsReadOnly => ((IList)_list).IsReadOnly;
        bool IList.IsFixedSize => ((IList)_list).IsFixedSize;

        int ICollection<T>.Count => ((ICollection)_list).Count;
        bool ICollection<T>.IsReadOnly => ((ICollection<T>)_list).IsReadOnly;
        int ICollection.Count => ((ICollection)_list).Count;
        object ICollection.SyncRoot => ((ICollection)_list).SyncRoot;
        bool ICollection.IsSynchronized => ((ICollection)_list).IsSynchronized;
        int IReadOnlyCollection<T>.Count => ((IReadOnlyCollection<T>)_list).Count;

        public int Count => _list.Count;

        T IReadOnlyList<T>.this[int index] => this[index];
        T IList<T>.this[int index] { get => this[index]; set => this[index] = value; }
        T IEventList<T>.this[int index] { get => this[index]; set => this[index] = value; }

        int IList.Add(object value)
        {
            Add((T)value);
            return Count - 1;
        }
        bool IList.Contains(object value) => Contains((T)value);
        int IList.IndexOf(object value) => IndexOf((T)value);
        public int IndexOf(T value) => _list.IndexOf(value);

        void IList.Insert(int index, object value) => Insert(index, (T)value);
        void IList.Remove(object value) => Remove((T)value);

        void ICollection<T>.Add(T item) => Add(item);
        public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
        void ICollection.CopyTo(Array array, int index) => CopyTo((T[])array, index);

        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private class ThreadSafeEnumerator : IEnumerator<T>
        {
            private int _currentIndex = 0;
            private readonly List<T> _list;
            private readonly ReaderWriterLockSlim _locker;

            public ThreadSafeEnumerator(List<T> list, ReaderWriterLockSlim locker)
            {
                _list = list;
                _locker = locker;
            }

            public T Current { get; private set; }
            object IEnumerator.Current => Current;

            public void Dispose() => _locker?.ExitReadLock();
            public bool MoveNext()
            {
                int index = _currentIndex;
                bool valid = index >= 0 && index < _list.Count;

                if (!valid)
                    return false;

                ++_currentIndex;
                Current = _list[index];

                if (index == 0)
                    _locker?.EnterReadLock();
                
                if (index == _list.Count - 1)
                    _locker?.ExitReadLock();

                return true;
            }
            public void Reset()
            {
                if (_currentIndex != 0)
                    _locker?.ExitReadLock();

                _currentIndex = 0;
                Current = _list.Count == 0 ? default : _list[0];
            }
        }
    }
}
