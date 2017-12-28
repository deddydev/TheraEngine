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
        protected internal void OnChanged() => Changed?.Invoke();
        
        private int _count = 0;

        protected internal abstract Keyframe FirstKey { get; internal set; }

        public int Count
        {
            get => _count;
            internal set => _count = value;
        }

        public void SetFrameCount(int numFrames, float framesPerSecond, bool stretchAnimation)
            => SetLength(numFrames / framesPerSecond, stretchAnimation);
        public abstract void SetLength(float lengthInSeconds, bool stretchAnimation);
    }
    public class KeyframeTrack<T> : BaseKeyframeTrack, IList, IList<T>, IEnumerable<T> where T : Keyframe
    {
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
        
        protected internal override Keyframe FirstKey
        {
            get => First;
            internal set => First = value as T;
        }

        public bool IsReadOnly => false;
        public bool IsFixedSize => false;
        //public int Count => base.Count;
        public object SyncRoot => _syncRoot;
        public bool IsSynchronized => _isSynchronized;

        public float LengthInSeconds => _lengthInSeconds;

        public override void SetLength(float seconds, bool stretch)
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
                    if (key.Next == FirstKey)
                        break;
                    key = key.Next;
                }
            }
        }
        
        T IList<T>.this[int index]
        {
            get
            {
                if (index >= 0 && index < Count)
                {
                    int i = 0;
                    foreach (T key in this)
                    {
                        if (i == index)
                            return key;
                        ++i;
                    }
                }
                return null;
            }
            set
            {
                if (index >= 0 && index <= Count)
                {
                    int i = 0;
                    foreach (T key in this)
                    {
                        if (i == index)
                        {
                            Keyframe prev = key.Prev;
                            key.Remove();
                            prev.Link(value);
                            break;
                        }
                        ++i;
                    }
                }
            }
        }

        public object this[int index]
        {
            get
            {
                if (index >= 0 && index < Count)
                {
                    int i = 0;
                    foreach (T key in this)
                    {
                        if (i == index)
                            return key;
                        ++i;
                    }
                }
                return null;
            }
            set
            {
                if (value is T keyValue && index >= 0 && index <= Count)
                {
                    int i = 0;
                    foreach (T key in this)
                    {
                        if (i == index)
                        {
                            Keyframe prev = key.Prev;
                            key.Remove();
                            prev.Link(keyValue);
                            break;
                        }
                        ++i;
                    }
                }
            }
        }

        public void Add(T key)
        {
            if (key.Second < 0)
                return;
            if (key.Second > LengthInSeconds)
                SetLength(key.Second, false);
            if (First == null)
            {
                First = key;
                ++base.Count;
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
                --base.Count;
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
                --base.Count;
                OnChanged();
            }
            else
            {
                Keyframe temp = First;
                First = (T)First.Next;
                temp.Remove();
            }
        }
        public T GetKeyBefore(float second)
        {
            foreach (T key in this)
                if (second >= key.Second)
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
                return 0;//key.TrackIndex;
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
            base.Count = 0;
        }

        public int IndexOf(object value)
        {
            int i = 0;
            if (value is T keyValue)
                foreach (T key in this)
                {
                    if (key == keyValue)
                        return i;
                    ++i;
                }

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

        //TODO: write keyframe append method
        public void Append(KeyframeTrack<T> keyframes)
        {
            foreach (Keyframe k in keyframes)
            {

            }
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
        [TSerialize("Second", XmlNodeType = EXmlNodeType.Attribute)]
        private float _second;

        protected Keyframe _next, _prev;

        private bool _isFirst;
        private int _trackIndex;
        private BaseKeyframeTrack _owningTrack;

        public Keyframe()
        {
            _next = this;
            _prev = this;
            //TrackIndex = 0;
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
        //public int TrackIndex
        //{
        //    get => _trackIndex;
        //    private set
        //    {
        //        _trackIndex = value;
        //        if (Prev != this && Prev.TrackIndex != _trackIndex - 1 && !IsFirst)
        //            Prev.TrackIndex = TrackIndex - 1;
        //        if (Next != this && Next.TrackIndex != _trackIndex + 1 && !Next.IsFirst)
        //            Next.TrackIndex = TrackIndex + 1;
        //    }
        //}

        public bool IsFirst
        {
            get => _isFirst;
            internal set
            {
                _isFirst = value;
                //TrackIndex = 0;
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
                //key.TrackIndex = key.Prev.TrackIndex + 1;
                key.OwningTrack = key.Prev.OwningTrack;
            }
            else
            {
                //key.TrackIndex = 0;
                key.OwningTrack = key.Next.OwningTrack;
            }

            OwningTrack.OnChanged();
        }

        public Keyframe Link(Keyframe key)
        {
            Relink(key);
            ++key.OwningTrack.Count;
            return key;
        }

        public abstract string WriteToString();
        public abstract void ReadFromString(string str);

        public void Remove()
        {
            if (_isFirst && _owningTrack != null)
                _owningTrack.FirstKey = Next != this ? Next : null;
            _next._prev = Prev;
            _prev._next = Next;
            //if (Next != this && Next.TrackIndex != _trackIndex && !Next.IsFirst)
            //    Next.TrackIndex = TrackIndex;
            _next = _prev = this;
            //TrackIndex = 0;
            if (_owningTrack != null)
            {
                --_owningTrack.Count;
                _owningTrack.OnChanged();
            }
            _owningTrack = null;
        }
    }
}
