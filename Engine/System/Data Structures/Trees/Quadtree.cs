using CustomEngine.Rendering;
using CustomEngine.Rendering.Models;
using System.Collections.Generic;
using System.Drawing;
using CustomEngine.Rendering.HUD;
using CustomEngine;

namespace System
{
    public interface IQuadtreeNode
    {
        BoundingRectangle Region { get; }
        Vec2 Center { get; }
        Vec2 Min { get; }
        Vec2 Max { get; }
        Vec2 Extents { get; }
        void ItemMoved(I2DBoundable item);
    }
    public class Quadtree : Quadtree<I2DBoundable>
    {
        public Quadtree(Vec2 bounds) : base(bounds) { }
        public Quadtree(Vec2 bounds, List<I2DBoundable> items) : base(bounds, items) { }
    }
    public class Quadtree<T> where T : I2DBoundable
    {
        private Vec2 _totalBounds;
        private Node _head;
        
        public Quadtree(Vec2 bounds)
            => _totalBounds = bounds;
        public Quadtree(Vec2 bounds, List<T> items)
        {
            _totalBounds = bounds;
            Add(items);
        }
        
        public List<T> FindClosest(Vec2 point)
            => _head.FindClosest(point);
        public void Add(T value)
        {
            if (_head == null)
                _head = new Node(new BoundingRectangle(Vec2.Zero, _totalBounds), null);
            _head.Add(value);
        }
        public void Add(List<T> value)
        {
            if (_head == null)
                _head = new Node(new BoundingRectangle(Vec2.Zero, _totalBounds), null);
            _head.Add(value);
        }
        public bool Remove(T value)
        {
            if (_head == null)
                return false;

            bool removed = _head.Remove(value, out bool destroy);
            if (destroy)
                _head = null;
            return removed;
        }

        public void Resize(Vec2 newBounds)
        {
            Vec2 scaleFactor = newBounds / _totalBounds;
            _totalBounds = newBounds;
            _head?.Resize(scaleFactor);
        }

        public void DebugRender()
            => _head?.DebugRender();
        
        public class Node : IQuadtreeNode
        {
            private Node _parent;
            private BoundingRectangle _region;
            public List<T> _items = new List<T>();
            public Node[] _subNodes;
            
            public List<T> Items
                => _items;
            public BoundingRectangle Region
                => _region;
            public Vec2 Center
                => (Min + Max) / 2.0f;
            public Vec2 Min
                => _region.Translation;
            public Vec2 Max
                => _region.Translation + _region.Bounds;
            public Vec2 Extents
                => _region.Bounds;

            public List<T> FindClosest(Vec2 point)
            {
                if (_region.Contains(point))
                {
                    List<T> list = null;
                    foreach (Node node in _subNodes)
                        if (node != null)
                        {
                            list = node.FindClosest(point);
                            if (list != null)
                                return list;
                        }

                    if (_items.Count == 0)
                        return null;

                    list = new List<T>(_items);
                    for (int i = 0; i < list.Count; ++i)
                        if (!list[i].Contains(point))
                            list.RemoveAt(i--);

                    return list;
                }
                else
                    return null;
            }
            public List<T> CollectChildren()
            {
                List<T> list = _items;
                foreach (Node node in _subNodes)
                    if (node != null)
                        list.AddRange(node.CollectChildren());
                return list;
            }
            public List<T> FindAll(BoundingRectangle bounds, EContainmentFlags containment)
            {
                List<T> list = new List<T>();

                //Not searching for anything?
                if (containment == EContainmentFlags.None)
                    return list;

                if (_region.Contains(bounds) == EContainment.Contains)
                {

                }

                return null;
            }
            private void FindAllInternal(RectangleF bounds, EContainmentFlags containment, List<T> found)
            {

            }
            /// <summary>
            /// shouldDestroy returns true if this node has no items contained within it or its subdivisions.
            /// returns true if the value was found and removed.
            /// </summary>
            public bool Remove(T value, out bool shouldDestroy)
            {
                bool hasBeenRemoved = false;
                if (_items.Contains(value))
                    hasBeenRemoved = _items.Remove(value);
                else if (_subNodes != null)
                {
                    bool anyNotNull = false;
                    for (int i = 0; i < 8; ++i)
                    {
                        Node node = _subNodes[i];
                        if (node != null)
                        {
                            if (hasBeenRemoved && anyNotNull)
                                break;
                            else
                            {
                                if (hasBeenRemoved = node.Remove(value, out bool remove))
                                {
                                    if (remove)
                                        _subNodes[i] = null;
                                }
                                else
                                    anyNotNull = true;
                            }
                        }
                    }
                    if (!anyNotNull)
                        _subNodes = null;
                }
                shouldDestroy = _items.Count == 0 && _subNodes == null;
                return hasBeenRemoved;
            }
            public void Add(List<T> value)
            {
                bool notSubdivided = true;
                List<T> items;
                for (int i = 0; i < 4; ++i)
                {
                    items = new List<T>();
                    BoundingRectangle bounds = GetSubdivision(i);
                    foreach (T item in value)
                    {
                        if (item == null)
                            continue;
                        if (bounds.Contains(item.AxisAlignedBounds) == EContainment.Contains)
                        {
                            notSubdivided = false;

                            if (_subNodes == null)
                            {
                                _subNodes = new Node[4];
                                _subNodes[i] = new Node(bounds, this);
                            }
                            else if (_subNodes[i] == null)
                                _subNodes[i] = new Node(bounds, this);

                            items.Add(item);

                            break;
                        }
                    }
                    if (_subNodes[i] != null && items.Count > 0)
                        _subNodes[i].Add(items);
                }

                if (notSubdivided)
                    foreach (T b in value)
                        if (_parent == null || _region.Contains(b.AxisAlignedBounds) == EContainment.Contains)
                            Items.Add(b);
            }
            public void ItemMoved(I2DBoundable item) => ItemMoved((T)item);
            public void ItemMoved(T item)
            {
                if (_region.Contains(item.AxisAlignedBounds) == EContainment.Contains)
                {

                    if (!_items.Contains(item))
                    {
                        _items.Add(item);
                        return;
                    }
                }
                else
                {
                    if (_items.Contains(item))
                        _items.Remove(item);
                    _parent?.ItemMoved(item);
                }
            }

            public void Add(T item)
            {
                if (item == null)
                    return;

                bool notSubdivided = true;
                for (int i = 0; i < 4; ++i)
                {
                    BoundingRectangle bounds = GetSubdivision(i);
                    if (bounds.Contains(item.AxisAlignedBounds) == EContainment.Contains)
                    {
                        notSubdivided = false;

                        if (_subNodes == null)
                        {
                            _subNodes = new Node[4];
                            _subNodes[i] = new Node(bounds, this);
                        }
                        else if (_subNodes[i] == null)
                            _subNodes[i] = new Node(bounds, this);

                        _subNodes[i].Add(item);
                        
                        break;
                    }
                }

                if (notSubdivided)
                    Items.Add(item);
            }
            public Node(BoundingRectangle bounds, Node parent)
            {
                _region = bounds;
                _parent = parent;
            }            
            public BoundingRectangle GetSubdivision(int index)
            {
                if (_subNodes != null && _subNodes[index] != null)
                    return _subNodes[index].Region;

                Vec2 center = Center;
                Vec2 halfExtents = Extents / 2.0f;
                Vec2 min = Min;
                switch (index)
                {
                    case 0: return new BoundingRectangle(min.X,                 min.Y,                 halfExtents.X, halfExtents.Y);
                    case 1: return new BoundingRectangle(min.X,                 min.Y + halfExtents.Y, halfExtents.X, halfExtents.Y);
                    case 2: return new BoundingRectangle(min.X + halfExtents.X, min.Y + halfExtents.Y, halfExtents.X, halfExtents.Y);
                    case 3: return new BoundingRectangle(min.X + halfExtents.X, min.Y,                 halfExtents.X, halfExtents.Y);
                }
                return BoundingRectangle.Empty;
            }
            
            public void Resize(Vec2 scaleFactor)
            {
                _region.Bounds *= scaleFactor;
                _region.Translation *= scaleFactor;
                if (_subNodes != null)
                    foreach (Node node in _subNodes)
                        node?.Resize(scaleFactor);
            }

            public void DebugRender()
            {
                
            }
        }
    }
}
