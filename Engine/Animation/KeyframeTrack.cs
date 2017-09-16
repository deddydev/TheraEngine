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

        protected void OnChanged()
            => Changed?.Invoke();
        
        protected float _fps = 60.0f;
        protected int _keyCount = 0;

        protected abstract Keyframe FirstKey { get; }
        public abstract BaseAnimation Owner { get; }

        public float FrameCount => Owner.FrameCount;
        public int KeyCount => _keyCount;
        public float FramesPerSecond
        {
            get => _fps;
            set
            {
                float ratio = _fps / value;
                Keyframe key = FirstKey;
                while (key != null)
                {
                    key._frameIndex *= ratio;
                    key = key.Next;
                }
            }
        }

        public abstract void SetFrameCount(int frameCount, bool stretchAnimation);
    }
    public class KeyframeTrack<T> : 
        BaseKeyframeTrack, IList, IList<T>, IEnumerable<T> where T : Keyframe
    {
        BaseAnimation _owner = null;
        private T _first = null;
        private object _syncRoot = null;
        private bool _isSynchronized = false;

        public T First
        {
            get => _first;
            set
            {
                if (_first != null)
                    _first.IsFirst = false;
                _first = value;
                if (_first != null)
                    _first.IsFirst = true;
            }
        }
        public T Last => (T)_first.Prev;

        public override BaseAnimation Owner => _owner;
        protected override Keyframe FirstKey => First;

        public bool IsReadOnly => false;
        public bool IsFixedSize => false;
        public int Count => KeyCount;
        public object SyncRoot => _syncRoot;
        public bool IsSynchronized => _isSynchronized;

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
                        if (key.TrackIndex == index - 1)
                            key.LinkNext(value);
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
                            key.Unlink();
                            prev.LinkNext(keyValue);
                        }
            }
        }

        public KeyframeTrack(BaseAnimation node)
        {
            _owner = node;
            _owner.FramesPerSecondChanged += _owner_FramesPerSecondChanged;
        }

        private void _owner_FramesPerSecondChanged()
        {
            FramesPerSecond = Owner.FramesPerSecond;
        }

        public bool Insert(T key)
        {
            if (key._frameIndex >= FrameCount || key._frameIndex < 0)
                return false;

            if (First == null)
            {
                First = key;
                ++_keyCount;
                OnChanged();
                return true;
            }
            else
            {
                Keyframe node = First;
                do
                {
                    if (key._frameIndex >= node._frameIndex)
                    {
                        node.LinkNext(key);
                        ++_keyCount;
                        OnChanged();
                        return true;
                    }
                    node = node.Next;
                }
                while (node != First);
            }
            return false;
        }
        public bool Remove(T key)
        {
            if (First == null)
                return false;

            Keyframe node = First;
            do
            {
                if (key == node)
                {
                    node.Unlink();
                    --_keyCount;
                    OnChanged();
                    return true;
                }
                node = node.Next;
            }
            while (node != First);
            return false;
        }
        public void RemoveLast()
        {
            if (First == null)
                return;

            if (First == Last)
                First = null;
            else
                Last.Unlink();
            --_keyCount;
            OnChanged();
        }
        public void RemoveFirst()
        {
            if (First == null)
                return;

            if (First.Next == First)
                First = null;
            else
            {
                Keyframe newFirst = First.Next;
                First.Unlink();
                First = (T)newFirst;
            }
            --_keyCount;
            OnChanged();
        }
        public void Add(T key)
        {
            if (First == null)
                First = key;
            else if (key._frameIndex < First._frameIndex)
            {
                First.LinkNext(key);
                First = key;
            }
            else
                First.LinkNext(key);
            ++_keyCount;
            OnChanged();
        }
        public T GetKeyBefore(float frameIndex)
        {
            foreach (T key in this)
                if (frameIndex >= key._frameIndex)
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

        public override void SetFrameCount(int frameCount, bool stretchAnimation)
        {
            throw new NotImplementedException();
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
            _keyCount = 0;
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
        [Serialize("FrameIndex", IsXmlAttribute = true)]
        public float _frameIndex;
        protected Keyframe _next, _prev;
        private bool _isFirst;
        private int _trackIndex;

        public Keyframe()
        {
            _next = this;
            _prev = this;
            TrackIndex = 0;
            _isFirst = false;
        }
        public Keyframe Next
        {
            get => _next;
            set
            {
                _next = value;
            }
        }
        public Keyframe Prev
        {
            get => _prev;
            set
            {
                _prev = value;
            }
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

        internal void Unlink()
        {
            TrackIndex = 0;
            _next.Prev = Prev;
            _prev.Next = Next;
            _next = _prev = this;
        }
        internal Keyframe LinkNext(Keyframe key)
        {
            if (key._frameIndex > _next._frameIndex && 
                _next._frameIndex > _frameIndex)
                return _next.LinkNext(key);

            if (key._frameIndex < _frameIndex && 
                _prev._frameIndex < _frameIndex)
                return _prev.LinkNext(key);

            key.Next = _next;
            key.Prev = this;

            _next.Prev = key;
            Next = key;

            key.TrackIndex = key.Prev.TrackIndex + 1;

            return key;
        }

        public abstract string WriteToString();
        public abstract void ReadFromString(string str);
    }
}
