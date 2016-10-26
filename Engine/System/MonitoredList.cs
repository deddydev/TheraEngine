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

        bool _updating = false;

        public MonitoredList() { }
        public MonitoredList(IEnumerable<T> list)
        {
            _updating = true;
            AddRange(list);
            _updating = false;
        }

        public new void Add(T item)
        {
            base.Add(item);
            if (!_updating)
                Added?.Invoke(item);
        }
        public new void AddRange(IEnumerable<T> collection)
        {
            base.AddRange(collection);
            if (!_updating)
                AddedRange?.Invoke(collection);
        }
        public new bool Remove(T item)
        {
            if (base.Remove(item))
            {
                if (!_updating)
                    Removed?.Invoke(item);
                return true;
            }
            return false;
        }
        public new void RemoveRange(int index, int count)
        {
            IEnumerable<T> range = GetRange(index, count);
            base.RemoveRange(index, count);
            if (!_updating)
                RemovedRange?.Invoke(range);
        }
        public new void RemoveAt(int index)
        {
            T item = this[index];
            base.RemoveAt(index);
            if (!_updating)
                Removed?.Invoke(item);
        }
        public new void Clear()
        {
            IEnumerable<T> range = GetRange(0, Count);
            base.Clear();
            if (!_updating)
                RemovedRange?.Invoke(range);
        }
        public new void RemoveAll(Predicate<T> match)
        {
            IEnumerable<T> matches = FindAll(match);
            base.RemoveAll(match);
            if (!_updating)
                RemovedRange?.Invoke(matches);
        }
        public new void Insert(int index, T item)
        {
            base.Insert(index, item);
            if (!_updating)
                Inserted?.Invoke(item, index);
        }
        public new void InsertRange(int index, IEnumerable<T> collection)
        {
            base.InsertRange(index, collection);
            if (!_updating)
                InsertedRange?.Invoke(collection, index);
        }
    }
}
