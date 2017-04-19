using System.Collections.Generic;
using System.Collections;
using System;
using CustomEngine.Files;

namespace CustomEngine.Rendering.Animation
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
    }
    public class KeyframeTrack<T> : BaseKeyframeTrack, IEnumerable<T> where T : Keyframe
    {
        BaseAnimation _owner;
        
        private T _first = null;

        public T First => _first;
        public T Last => (T)_first.Prev;

        public override BaseAnimation Owner => _owner;
        protected override Keyframe FirstKey => First;

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

            if (_first == null)
            {
                _first = key;
                ++_keyCount;
                OnChanged();
                return true;
            }
            else
            {
                Keyframe node = _first;
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
                while (node != _first);
            }
            return false;
        }
        public bool Remove(T key)
        {
            if (_first == null)
                return false;

            Keyframe node = _first;
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
            while (node != _first);
            return false;
        }
        public void RemoveLast()
        {
            if (_first == null)
                return;

            if (First == Last)
                _first = null;
            else
                Last.Unlink();
            --_keyCount;
            OnChanged();
        }
        public void RemoveFirst()
        {
            if (_first == null)
                return;

            if (First.Next == First)
                _first = null;
            else
            {
                Keyframe newFirst = First.Next;
                First.Unlink();
                _first = (T)newFirst;
            }
            --_keyCount;
            OnChanged();
        }
        public void Add(T key)
        {
            if (_first == null)
                _first = key;
            else if (key._frameIndex < _first._frameIndex)
            {
                _first.LinkPrev(key);
                _first = key;
            }
            else
                _first.LinkNext(key);
            ++_keyCount;
            OnChanged();
        }
        //public void AddLast(T key)
        //{
        //    if (_first == null)
        //        _first = key;
        //    else
        //        _first.LinkPrev(key);
        //    ++_keyCount;
        //}
        //public void AddFirst(T key)
        //{
        //    if (_first == null)
        //        _first = key;
        //    else
        //    {
        //        _first.LinkPrev(key);
        //        _first = key;
        //    }
        //    ++_keyCount;
        //}
        public T GetKeyBefore(float frameIndex)
        {
            foreach (T key in this)
                if (frameIndex >= key._frameIndex)
                    return key;
            return null;
        }
        public IEnumerator GetEnumerator()
        {
            Keyframe node = _first;
            do
            {
                if (node == null)
                    break;
                yield return node;
                node = node.Next;
            }
            while (node != _first);
        }
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            Keyframe node = _first;
            do
            {
                if (node == null)
                    break;
                yield return (T)node;
                node = node.Next;
            }
            while (node != _first);
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
    public abstract class Keyframe
    {
        public float _frameIndex;
        protected Keyframe _next, _prev;

        public Keyframe()
        {
            _next = this;
            _prev = this;
        }
        public Keyframe Next
        {
            get => _next;
            set => _next = value;
        }
        public Keyframe Prev
        {
            get => _prev;
            set => _prev = value;
        }
        public void Unlink()
        {
            _next.Prev = Prev;
            _prev.Next = Next;
            _next = _prev = this;
        }
        public Keyframe LinkNext(Keyframe next)
        {
            if (next._frameIndex > _next._frameIndex && _next != this)
                return _next.LinkNext(next);
            if (next._frameIndex < _prev._frameIndex && _prev != this)
                return _prev.LinkPrev(next);

            next.Next = _next;
            next.Prev = this;

            _next._prev = next;
            _next = next;

            return next;
        }
        public Keyframe LinkPrev(Keyframe prev)
        {
            if (prev._frameIndex < _prev._frameIndex && _prev != this)
                return _prev.LinkPrev(prev);
            if (prev._frameIndex > _next._frameIndex && _next != this)
                return _next.LinkNext(prev);
            
            prev.Next = this;
            prev.Prev = _prev;

            _prev._next = prev;
            _prev = prev;

            return prev;
        }
    }
}
