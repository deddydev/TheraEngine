using System.Collections;
using System.Collections.Generic;
using Extensions;

namespace TheraEngine.Core
{
    /// <summary>
    /// A list that contains items whose indices never change even if items before them are removed.
    /// </summary>
    public class ConsistentIndexList<T> : IEnumerable<T>
    {
        private readonly List<T> _list = new List<T>();
        private readonly List<int> _nullIndices = new List<int>();
        private readonly List<int> _activeIndices = new List<int>();

        public int Count => _activeIndices.Count;
        public T this[int index]
        {
            get => _list[index];
            set
            {
                lock (_activeIndices)
                {
                    if (_list[index] == default && value != default)
                        _activeIndices.Add(index);
                    else if (_list[index] != default && value == default)
                        _activeIndices.Remove(index);
                }

                _list[index] = value;
            }
        }
        //public int IndexOfNextAddition(int offset)
        //{
        //    if (_nullIndices.Count > offset)
        //        return _nullIndices[offset];
        //    else
        //        return _list.Count + offset - _nullIndices.Count;
        //}
        public int Add(T item)
        {
            int index;
            if (_nullIndices.Count > 0)
            {
                index = _nullIndices[0];
                _nullIndices.RemoveAt(0);
                _list[index] = item;
            }
            else
            {
                index = _list.Count;
                _list.Add(item);
            }
            lock (_activeIndices)
            {
                _activeIndices.Add(index);
            }
            return index;
        }
        public void Remove(T item)
        {
            RemoveAt(_list.IndexOf(item));
        }
        public void RemoveAt(int index)
        {
            if (index < 0)
                return;

            if (index == _list.Count - 1)
            {
                _list.RemoveAt(index);
                while (_list.Count > 0 && _list[_list.Count - 1] == default)
                    _list.RemoveAt(_list.Count - 1);
            }
            else
            {
                _list[index] = default;
                int addIndex = _nullIndices.BinarySearch(index);
                if (addIndex < 0)
                    addIndex = ~addIndex;
                _nullIndices.Insert(addIndex, index);
            }
            lock (_activeIndices)
            {
                _activeIndices.Remove(index);
            }
        }

        public bool HasValueAtIndex(int index)
            => index >= 0 && index < _list.Count && _list[index] != default;

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            lock (_activeIndices)
            {
                foreach (int i in _activeIndices)
                    if (_list.IndexInRange(i))
                        yield return _list[i];
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (_activeIndices)
            {
                foreach (int i in _activeIndices)
                    if (_list.IndexInRange(i))
                        yield return _list[i];
            }
        }
    }
}
