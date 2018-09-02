using System.Collections.Generic;
using System.Collections;
using System;
using TheraEngine.Files;
using System.ComponentModel;

namespace TheraEngine.Animation
{
    public abstract class BaseKeyframeTrack : TFileObject
    {
        public event Action Changed;
        protected internal void OnChanged() => Changed?.Invoke();

        protected internal abstract Keyframe FirstKey { get; internal set; }

        [Browsable(false)]
        public int Count { get; internal set; } = 0;

        public void SetFrameCount(int numFrames, float framesPerSecond, bool stretchAnimation)
            => SetLength(numFrames / framesPerSecond, stretchAnimation);
        public abstract void SetLength(float lengthInSeconds, bool stretchAnimation);
    }
    public class KeyframeTrack<T> : BaseKeyframeTrack, IList, IList<T>, IEnumerable<T> where T : Keyframe
    {
        private T _first = null;

        [Browsable(false)]
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
        [Browsable(false)]
        public T Last => _first?.Prev as T;
        
        protected internal override Keyframe FirstKey
        {
            get => First;
            internal set => First = value as T;
        }

        [Browsable(false)]
        public bool IsReadOnly => false;
        [Browsable(false)]
        public bool IsFixedSize => false;
        //public int Count => base.Count;
        [Browsable(false)]
        public object SyncRoot { get; } = null;
        [Browsable(false)]
        public bool IsSynchronized { get; } = false;
        [Browsable(false)]
        public float LengthInSeconds { get; private set; } = 0.0f;

        public override void SetLength(float seconds, bool stretch)
        {
            float ratio = seconds / LengthInSeconds;
            LengthInSeconds = seconds;
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
                    if (key.Second < 0 || key.Second > LengthInSeconds)
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

        public void Add(IEnumerable<T> keys)
        {
            keys.ForEach(x => Add(x));
        }
        public void Add(params T[] keys)
        {
            keys.ForEach(x => Add(x));
        }
        public void Add(T key)
        {
            key.OwningTrack = this;
            if (key.Second < 0)
                return;
            if (key.Second > LengthInSeconds)
                SetLength(key.Second, false);
            if (First == null)
            {
                First = key;
                ++Count;
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
                --Count;
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
                --Count;
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
            Count = 0;
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

        public Keyframe()
        {
            _next = this;
            _prev = this;
            //TrackIndex = 0;
            IsFirst = false;
            OwningTrack = null;
        }

        public float Second
        {
            get => _second;
            set
            {
                _second = value;
                if (float.IsNaN(_second))
                    _second = 0.0f;
                if (Prev != this)
                    Prev.Relink(this);
            }
        }

        [Browsable(false)]
        public Keyframe Next => _next;
        [Browsable(false)]
        public Keyframe Prev => _prev;
        [Browsable(false)]
        public bool IsFirst { get; internal set; }
        [Browsable(false)]
        public BaseKeyframeTrack OwningTrack { get; internal set; }

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

        private void Relink(Keyframe key)
        {
            if (key == null)
                return;

            if (_next != null && key.Second > _next.Second && _next.Second > Second)
            {
                _next.Relink(key);
                return;
            }

            if (_prev != null && key.Second < Second && _prev.Second < Second)
            {
                _prev.Relink(key);
                return;
            }

            key._next = _next;
            key._prev = this;

            if (_next != null)
                _next._prev = key;
            _next = key;

            if (key.Next != null && key.Next.IsFirst && key.Second < key.Next.Second)
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

            OwningTrack?.OnChanged();
        }

        public Keyframe Link(Keyframe key)
        {
            if (key.OwningTrack != null)
                ++key.OwningTrack.Count;
            Relink(key);
            return key;
        }

        public abstract string WriteToString();
        public abstract void ReadFromString(string str);
        public override string ToString() => WriteToString();

        public void Remove()
        {
            if (IsFirst && OwningTrack != null)
                OwningTrack.FirstKey = Next != this ? Next : null;
            _next._prev = Prev;
            _prev._next = Next;
            //if (Next != this && Next.TrackIndex != _trackIndex && !Next.IsFirst)
            //    Next.TrackIndex = TrackIndex;
            _next = _prev = this;
            //TrackIndex = 0;
            if (OwningTrack != null)
            {
                --OwningTrack.Count;
                OwningTrack.OnChanged();
            }
            OwningTrack = null;
        }
    }
}
