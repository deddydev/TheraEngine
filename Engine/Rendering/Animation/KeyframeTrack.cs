using System.Collections.Generic;
using System.Collections;
using System;
using CustomEngine.Files;

namespace CustomEngine.Rendering.Animation
{
    public abstract class BaseKeyframeTrack : FileObject
    {
        protected int _fps = 60;
        protected int _keyCount = 0;

        public abstract BasePropertyAnimation Owner { get; }
        public float FrameCount { get { return Owner.FrameCount; } }
        public int KeyCount { get { return _keyCount; } }
        public int FPS
        {
            get { return _fps; }
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
        protected abstract Keyframe FirstKey { get; }
    }
    public class KeyframeTrack<T> : BaseKeyframeTrack, IEnumerable<T> where T : Keyframe
    {
        PropertyAnimation<T> _owner;
        
        private T _first = null;

        public T First { get { return _first; } }
        public T Last { get { return (T)_first.Prev; } }

        public override BasePropertyAnimation Owner { get { return _owner; } }
        protected override Keyframe FirstKey { get { return First; } }

        public KeyframeTrack(PropertyAnimation<T> node) { _owner = node; }

        public bool Insert(T key)
        {
            if (key._frameIndex >= FrameCount || key._frameIndex < 0)
                return false;

            if (_first == null)
            {
                _first = key;
                ++_keyCount;
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
        }
        public void AddLast(T key)
        {
            if (_first == null)
                _first = key;
            else
                _first.LinkPrev(key);
            ++_keyCount;
        }
        public void AddFirst(T key)
        {
            if (_first == null)
                _first = key;
            else
            {
                _first.LinkPrev(key);
                _first = key;
            }
            ++_keyCount;
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
    public abstract class Keyframe
    {
        public float _frameIndex;
        protected Keyframe _next, _prev;

        public Keyframe()
        {
            _next = this;
            _prev = this;
        }
        
        public Keyframe Next { get { return _next; } set { _next = value; } }
        public Keyframe Prev { get { return _prev; } set { _prev = value; } }

        public void Unlink()
        {
            _next.Prev = Prev;
            _prev.Next = Next;
            _next = _prev = this;
        }
        public Keyframe LinkNext(Keyframe next)
        {
            next.Next = _next;
            next.Prev = this;

            _next._prev = next;
            _next = next;

            return next;
        }
        public Keyframe LinkPrev(Keyframe prev)
        {
            prev.Next = this;
            prev.Prev = _prev;

            _prev._next = prev;
            _prev = prev;

            return prev;
        }
    }
}
