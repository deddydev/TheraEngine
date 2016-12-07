using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    public class MonitoredList<T> : List<T>
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

        bool _updating = false;
        bool _allowDuplicates = true;

        public MonitoredList()
        {

        }
        public MonitoredList(bool allowDuplicates)
        {
            _allowDuplicates = allowDuplicates;
        }
        public MonitoredList(IEnumerable<T> list)
        {
            AddRangeSilent(list);
        }
        public MonitoredList(IEnumerable<T> list, bool allowDuplicates)
        {
            AddRangeSilent(list);
            _allowDuplicates = allowDuplicates;
        }

        public void AddSilent(T item)
        {
            _updating = true;
            Add(item);
            _updating = false;
        }
        public new void Add(T item)
        {
            if (!_allowDuplicates && Contains(item))
                return;
            base.Add(item);
            if (!_updating)
            {
                Added?.Invoke(item);
                Modified?.Invoke();
            }
        }
        public void AddRangeSilent(IEnumerable<T> collection)
        {
            _updating = true;
            AddRange(collection);
            _updating = false;
        }
        public new void AddRange(IEnumerable<T> collection)
        {
            if (collection == null)
                return;
            if (!_allowDuplicates)
            {
                IEnumerable<T> newCollection = collection.Where(x => !Contains(x));
                base.AddRange(newCollection);
                if (!_updating)
                {
                    AddedRange?.Invoke(newCollection);
                    Modified?.Invoke();
                }
            }
            else
            {
                base.AddRange(collection);
                if (!_updating)
                {
                    AddedRange?.Invoke(collection);
                    Modified?.Invoke();
                }
            }
        }
        public bool RemoveSilent(T item)
        {
            _updating = true;
            bool result = Remove(item);
            _updating = false;
            return result;
        }
        public new bool Remove(T item)
        {
            if (base.Remove(item))
            {
                if (!_updating)
                {
                    Removed?.Invoke(item);
                    Modified?.Invoke();
                }
                return true;
            }
            return false;
        }
        public void RemoveRangeSilent(int index, int count)
        {
            _updating = true;
            RemoveRange(index, count);
            _updating = false;
        }
        public new void RemoveRange(int index, int count)
        {
            IEnumerable<T> range = GetRange(index, count);
            base.RemoveRange(index, count);
            if (!_updating)
            {
                RemovedRange?.Invoke(range);
                Modified?.Invoke();
            }
        }
        public void RemoveAtSilent(int index)
        {
            _updating = true;
            RemoveAt(index);
            _updating = false;
        }
        public new void RemoveAt(int index)
        {
            T item = this[index];
            base.RemoveAt(index);
            if (!_updating)
            {
                Removed?.Invoke(item);
                Modified?.Invoke();
            }
        }
        public void ClearSilent()
        {
            _updating = true;
            Clear();
            _updating = false;
        }
        public new void Clear()
        {
            IEnumerable<T> range = GetRange(0, Count);
            base.Clear();
            if (!_updating)
            {
                RemovedRange?.Invoke(range);
                Modified?.Invoke();
            }
        }
        public void RemoveAllSilent(Predicate<T> match)
        {
            _updating = true;
            RemoveAll(match);
            _updating = false;
        }
        public new void RemoveAll(Predicate<T> match)
        {
            if (!_updating)
            {
                IEnumerable<T> matches = FindAll(match);
                base.RemoveAll(match);
                RemovedRange?.Invoke(matches);
                Modified?.Invoke();
            }
            else
                base.RemoveAll(match);
        }
        public void InsertSilent(int index, T item)
        {
            _updating = true;
            Insert(index, item);
            _updating = false;
        }
        public new void Insert(int index, T item)
        {
            if (!_allowDuplicates && Contains(item))
                return;
            base.Insert(index, item);
            if (!_updating)
            {
                Inserted?.Invoke(item, index);
                Modified?.Invoke();
            }
        }
        public void InsertRangeSilent(int index, IEnumerable<T> collection)
        {
            _updating = true;
            InsertRange(index, collection);
            _updating = false;
        }
        public new void InsertRange(int index, IEnumerable<T> collection)
        {
            if (collection == null)
                return;
            if (!_allowDuplicates)
            {
                IEnumerable<T> newCollection = collection.Where(x => !Contains(x));
                base.InsertRange(index, newCollection);
                if (!_updating)
                {
                    InsertedRange?.Invoke(newCollection, index);
                    Modified?.Invoke();
                }
            }
            else
            {
                base.InsertRange(index, collection);
                if (!_updating)
                {
                    InsertedRange?.Invoke(collection, index);
                    Modified?.Invoke();
                }
            }
        }
    }
}
