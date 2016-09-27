using System.Collections.Generic;
using System.Collections;

namespace CustomEngine.Rendering.Animation
{
    public class KeyframeTrack<T> : IEnumerable<T> where T : Keyframe
    {
        PropertyAnimation<T> _owner;

        private int _fps = 60; //Game default is 60
        private int FrameCount { get { return _owner.FrameCount; } }
        private T _first = null;
        private int _count = 0;

        public T First { get { return _first; } }
        public T Last { get { return (T)_first.Prev; } }
        public int Count { get { return _count; } }

        public KeyframeTrack(PropertyAnimation<T> node) { _owner = node; }

        public bool Insert(T key)
        {
            if (key._frameIndex >= FrameCount || key._frameIndex < 0)
                return false;

            if (_first == null)
            {
                _first = key;
                ++_count;
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
                        ++_count;
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
                    --_count;
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
            --_count;
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
            --_count;
        }
        public void AddLast(T key)
        {
            if (_first == null)
                _first = key;
            else
                _first.LinkPrev(key);
            ++_count;
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
            ++_count;
        }
        public T GetKeyBefore(float frameIndex)
        {
            foreach (T key in this)
                if (frameIndex >= key._frameIndex)
                    return key;
            return null;
        }
        public void ConvertFPS(float newFPS)
        {
            float ratio = _fps / newFPS;
            foreach (T key in this)
                key._frameIndex *= ratio;
        }
        public IEnumerator GetEnumerator()
        {
            Keyframe node = _first;
            while (node != null)
            {
                if (node == null)
                    break;
                yield return node;
                node = node.Next;
            }
        }
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            Keyframe node = _first;
            while (node != null)
            {
                if (node == null)
                    break;
                yield return (T)node;
                node = node.Next;
            }
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
        public void LinkNext(Keyframe next)
        {
            next.Next = _next;
            next.Prev = this;

            _next._prev = next;
            _next = next;
        }
        public void LinkPrev(Keyframe prev)
        {
            prev.Next = this;
            prev.Prev = _prev;

            _prev._next = prev;
            _prev = prev;
        }
    }
}
