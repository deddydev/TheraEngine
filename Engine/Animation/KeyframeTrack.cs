using System.Collections.Generic;
using System.Collections;
using System;
using TheraEngine.Files;
using System.ComponentModel;

namespace TheraEngine.Animation
{
    public abstract class BaseKeyframeTrack : FileObject
    {
        public event Action Changed;

        internal protected void OnChanged()
            => Changed?.Invoke();
        
        //protected float _fps = 60.0f;
        private int _keyCount = 0;

        protected internal abstract Keyframe FirstKey { get; internal set; }
        //public abstract BaseAnimation Owner { get; }

        //public float FrameCount => Owner.BakedFrameCount;
        public int KeyCount
        {
            get => _keyCount;
            internal set => _keyCount = value;
        }

        internal void SetLength(float seconds, bool stretchAnimation)
        {
            throw new NotImplementedException();
        }

        //public float FramesPerSecond
        //{
        //    get => _fps;
        //    set
        //    {
        //        float ratio = _fps / value;
        //        Keyframe key = FirstKey;
        //        while (key != null)
        //        {
        //            key._frameIndex *= ratio;
        //            key = key.Next;
        //        }
        //    }
        //}

        //public abstract void SetFrameCount(int frameCount, bool stretchAnimation);
    }
    public class KeyframeTrack<T> : BaseKeyframeTrack, IList, IList<T>, IEnumerable<T> where T : Keyframe
    {
        //BaseAnimation _owner = null;
        private T _first = null;
        private object _syncRoot = null;
        private bool _isSynchronized = false;
        private float _lengthInSeconds = 0.0f;
        
        public T First
        {
            get => _first;
            private set
            {
                if (_first != null)
                    _first.IsFirst = false;
                _first = value;
                if (_first != null)
                {
                    _first.IsFirst = true;
                    _first.OwningTrack = this;
                }
            }
        }
        public T Last => (T)_first.Prev;

        //public override BaseAnimation Owner => _owner;
        protected internal override Keyframe FirstKey
        {
            get => First;
            internal set => First = value as T;
        }

        public bool IsReadOnly => false;
        public bool IsFixedSize => false;
        public int Count => KeyCount;
        public object SyncRoot => _syncRoot;
        public bool IsSynchronized => _isSynchronized;

        public float LengthInSeconds => _lengthInSeconds;

        public void SetLength(float seconds, bool stretch)
        {
            float ratio = seconds / _lengthInSeconds;
            _lengthInSeconds = seconds;
            if (stretch)
            {
                Keyframe key = FirstKey;
                while (key != null)
                {
                    key.Second *= ratio;
                    key = key.Next;
                }
            }
            else
            {
                Keyframe key = FirstKey;
                while (key != null)
                {
                    if (key.Second < 0 || key.Second > _lengthInSeconds)
                        key.Remove();
                    key = key.Next;
                }
            }
        }
        
        T IList<T>.this[int index]
        {
            get
            {
                if (index >= 0 && index < Count)
                    foreach (T key in this)
                        if (key.TrackIndex == index)
                            return key;
                return null;
            }
            set
            {
                if (index >= 0 && index <= Count)
                    foreach (T key in this)
                        if (key.TrackIndex == index)
                        {
                            Keyframe prev = key.Prev;
                            key.Remove();
                            prev.Link(value);
                        }
            }
        }
        public object this[int index]
        {
            get
            {
                if (index >= 0 && index < Count)
                    foreach (T key in this)
                        if (key.TrackIndex == index)
                            return key;
                return null;
            }
            set
            {
                if (value is T keyValue && index >= 0 && index <= Count)
                    foreach (T key in this)
                        if (key.TrackIndex == index)
                        {
                            Keyframe prev = key.Prev;
                            key.Remove();
                            prev.Link(keyValue);
                        }
            }
        }

        public KeyframeTrack(/*BaseAnimation node*/)
        {
            //_owner = node;
            //_owner.FramesPerSecondChanged += _owner_FramesPerSecondChanged;
        }

        //private void _owner_FramesPerSecondChanged()
        //{
        //    FramesPerSecond = Owner.BakedFramesPerSecond;
        //}

        public void Add(T key)
        {
            if (key.Second >= LengthInSeconds || key.Second < 0)
                return;
            if (First == null)
            {
                First = key;
                ++KeyCount;
                OnChanged();
            }
            else if (key.Second < First.Second)
            {
                T temp = First;
                First = key;
                temp.Link(key);
            }
            else
                First.Link(key);
        }
        public void RemoveLast()
        {
            if (First == null)
                return;

            if (First == Last)
            {
                First = null;
                --KeyCount;
                OnChanged();
            }
            else
                Last.Remove();
        }
        public void RemoveFirst()
        {
            if (First == null)
                return;

            if (First.Next == First)
            {
                First = null;
                --KeyCount;
                OnChanged();
            }
            else
            {
                Keyframe temp = First;
                First = (T)First.Next;
                temp.Remove();
            }
        }
        public T GetKeyBefore(float frameIndex)
        {
            foreach (T key in this)
                if (frameIndex >= key.Second)
                    return key;
            return null;
        }
        public IEnumerator GetEnumerator()
        {
            Keyframe node = First;
            do
            {
                if (node == null)
                    break;
                yield return node;
                node = node.Next;
            }
            while (node != First);
        }
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            Keyframe node = First;
            do
            {
                if (node == null)
                    break;
                yield return (T)node;
                node = node.Next;
            }
            while (node != First);
        }
        
        public int Add(object value)
        {
            if (value is T key)
            {
                Add(key);
                return key.TrackIndex;
            }
            return -1;
        }

        public bool Contains(object value)
        {
            if (value is T keyValue)
                foreach (T key in this)
                    if (key == keyValue)
                        return true;
            return false;
        }

        public void Clear()
        {
            _first = null;
            KeyCount = 0;
        }

        public int IndexOf(object value)
        {
            if (value is T keyValue)
                foreach (T key in this)
                    if (key == keyValue)
                        return key.TrackIndex;

            return -1;
        }

        public void Insert(int index, object value)
        {

        }

        public void Remove(object value)
        {

        }

        public void RemoveAt(int index)
        {

        }

        public void CopyTo(Array array, int index)
        {

        }

        public int IndexOf(T item)
        {
            return -1;
        }

        public void Insert(int index, T item)
        {
            
        }

        public bool Contains(T item)
        {
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {

        }

        public bool Remove(T item)
        {
            if (item.OwningTrack == this)
            {
                item.Remove();
                return true;
            }
            return false;
        }
    }
    public enum RadialInterpType
    {
        Step,
        Linear,
        CubicBezier
    }
    public enum PlanarInterpType
    {
        Step,
        Linear,
        CubicHermite,
        CubicBezier
    }
    public abstract class Keyframe : IParsable
    {
        [Serialize("Second", IsXmlAttribute = true)]
        private float _second;

        protected Keyframe _next, _prev;

        private bool _isFirst;
        private int _trackIndex;
        private BaseKeyframeTrack _owningTrack;

        public Keyframe()
        {
            _next = this;
            _prev = this;
            TrackIndex = 0;
            _isFirst = false;
            _owningTrack = null;
        }

        public float Second
        {
            get => _second;
            set
            {
                _second = value;
                if (Prev != this)
                    Prev.Relink(this);
            }
        }

        public Keyframe Next => _next;
        public Keyframe Prev => _prev;
        public BaseKeyframeTrack OwningTrack
        {
            get => _owningTrack;
            internal set => _owningTrack = value;
        }
        public int TrackIndex
        {
            get => _trackIndex;
            private set
            {
                _trackIndex = value;
                if (Prev != this && Prev.TrackIndex != _trackIndex - 1 && !IsFirst)
                    Prev.TrackIndex = TrackIndex - 1;
                if (Next != this && Next.TrackIndex != _trackIndex + 1 && !Next.IsFirst)
                    Next.TrackIndex = TrackIndex + 1;
            }
        }

        public bool IsFirst
        {
            get => _isFirst;
            internal set
            {
                _isFirst = value;
                TrackIndex = 0;
            }
        }

        private void Relink(Keyframe key)
        {
            if (key.Second > _next.Second && _next.Second > Second)
            {
                _next.Relink(key);
                return;
            }

            if (key.Second < Second && _prev.Second < Second)
            {
                _prev.Relink(key);
                return;
            }

            key._next = _next;
            key._prev = this;

            _next._prev = key;
            _next = key;

            if (key.Next.IsFirst && key.Second < key.Next.Second)
                key.Next.OwningTrack.FirstKey = key;

            if (!key.IsFirst)
            {
                key.TrackIndex = key.Prev.TrackIndex + 1;
                key.OwningTrack = key.Prev.OwningTrack;
            }
            else
            {
                key.TrackIndex = 0;
                key.OwningTrack = key.Next.OwningTrack;
            }

            OwningTrack.OnChanged();
        }

        public Keyframe Link(Keyframe key)
        {
            Relink(key);
            ++key.OwningTrack.KeyCount;
            return key;
        }

        public abstract string WriteToString();
        public abstract void ReadFromString(string str);

        public void Remove()
        {
            _next._prev = Prev;
            _prev._next = Next;
            if (Next != this && Next.TrackIndex != _trackIndex && !Next.IsFirst)
                Next.TrackIndex = TrackIndex;
            _next = _prev = this;
            TrackIndex = 0;
            if (_owningTrack != null)
            {
                --_owningTrack.KeyCount;
                _owningTrack.OnChanged();
            }
            _owningTrack = null;
        }
    }
}
