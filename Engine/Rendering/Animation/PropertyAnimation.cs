using CustomEngine.System;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Collections;
using CustomEngine.Components;

namespace CustomEngine.Rendering.Animation
{
    public abstract class PropertyAnim : ObjectBase, IEnumerable
    {
        int _fps = 60;
        int _frameCount;
        string _propertyName;
        
        event EventHandler AnimationStarted;
        event EventHandler AnimationEnded;

        [EngineFlags(EEngineFlags.Transient)]
        double _currentFrame;
        
        public string PropertyName
        {
            get { return _propertyName; }
            set
            {
                _propertyName = value;
                Changed(MethodBase.GetCurrentMethod());
            }
        }
        public int FrameCount
        {
            get { return _frameCount; }
            set
            {
                int oldCount = _frameCount;
                Resize(value);
                FrameCountUpdated(oldCount);
                Changed(MethodBase.GetCurrentMethod());
            }
        }
        private void FrameCountUpdated(int oldCount) { }
        public abstract void Resize(int newSize);
        public abstract void Stretch(int newSize);
        public abstract void Append(PropertyAnim other);
        public abstract void Bake();
        public abstract IEnumerator GetEnumerator();
    }
    
    
    
    public class KeyframeTrack<T> : IEnumerable where T : AnimKeyFrame
    {
        int _frameCount;
        public KeyframeTrack(PropertyAnim node)
        {
            _frameCount = node.FrameCount;
        }

        private T _first = null;
        private int _count;

        public T First { get { return _first; } }
        public T Last { get { return (T)_first.Prev; } }
        public int Count { get { return _count; } }

        public bool Insert(T key)
        {
            if (key.FrameIndex >= _frameCount || key.FrameIndex < 0)
                return false;

            if (_first == null)
            {
                _first = key;
                return true;
            }
            else
            {
                AnimKeyFrame node = _first;
                do
                {
                    if (key.FrameIndex >= node.FrameIndex)
                    {
                        node.LinkNext(key);
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

            AnimKeyFrame node = _first;
            do
            {
                if (key == node)
                {
                    node.Unlink();
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
        }
        public void RemoveFirst()
        {
            if (_first == null)
                return;

            if (First.Next == First)
                _first = null;
            else
            {
                AnimKeyFrame newFirst = First.Next;
                First.Unlink();
                _first = (T)newFirst;
            }
        }
        public void AddLast(T key)
        {
            if (_first == null)
                _first = key;
            else
                _first.LinkPrev(key);
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
        }

        public IEnumerator GetEnumerator()
        {
            AnimKeyFrame node = _first;
            while (node != null)
            {
                if (node == null)
                    break;
                yield return node;
                node = node.Next;
            }
        }

        public T GetKeyframe(int frameIndex)
        {
            
        }
    }
    public abstract class AnimKeyFrame
    {
        protected int _frameIndex;
        protected AnimKeyFrame _next, _prev;

        public AnimKeyFrame()
        {
            _next = this;
            _prev = this;
        }
        
        public int FrameIndex { get { return _frameIndex; } }

        public AnimKeyFrame Next { get { return _next; } set { _next = value; } }
        public AnimKeyFrame Prev { get { return _prev; } set { _prev = value; } }

        public void Unlink()
        {
            _next.Prev = Prev;
            _prev.Next = Next;
            _next = _prev = this;
        }
        public void LinkNext(AnimKeyFrame next)
        {
            next.Next = _next;
            next.Prev = this;

            _next._prev = next;
            _next = next;
        }
        public void LinkPrev(AnimKeyFrame prev)
        {
            prev.Next = this;
            prev.Prev = _prev;

            _prev._next = prev;
            _prev = prev;
        }
    }
    
    
    
}
