using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TheraEngine.Worlds
{
    [Serializable]
    public class EventDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public EventDictionary() : base() { }
        public EventDictionary(int capacity) : base(capacity) { }
        public EventDictionary(IEqualityComparer<TKey> comparer) : base(comparer) { }
        public EventDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary) { }
        public EventDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer) { }
        public EventDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer) { }
        protected EventDictionary(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public delegate void DelAdded(TKey key, TValue value);
        public delegate void DelCleared();
        public delegate void DelRemoved(TKey key, TValue value);
        public delegate void DelSet(TKey key, TValue oldValue, TValue newValue);

        public event DelAdded Added;
        public event DelCleared Cleared;
        public event DelRemoved Removed;
        public event DelSet Set;
        public event Action Changed;

        public new TValue this[TKey key]
        {
            get => base[key];
            set
            {
                TValue old = base[key];
                base[key] = value;
                Set?.Invoke(key, old, value);
                Changed?.Invoke();
            }
        }
        public new void Add(TKey key, TValue value)
        {
            base.Add(key, value);
            Added?.Invoke(key, value);
            Changed?.Invoke();
        }
        public new void Clear()
        {
            base.Clear();
            Cleared?.Invoke();
            Changed?.Invoke();
        }
        public new bool Remove(TKey key)
        {
            TValue old = base[key];
            bool success = base.Remove(key);
            if (success)
            {
                Removed?.Invoke(key, old);
                Changed?.Invoke();
            }
            return success;
        }
    }
}