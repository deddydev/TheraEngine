using CustomEngine.Rendering;
using CustomEngine.Rendering.Models;
using System.Collections.Generic;
using System.Drawing;
using CustomEngine.Rendering.HUD;

namespace System
{
    public interface IBoundable
    {
        BoundingRectangle AxisAlignedBounds { get; }
        Quadtree.QuadtreeNode RenderNode { get; set; }
    }
    public class Quadtree
    {
        private Vec2 _totalBounds;
        private QuadtreeNode _head;
        
        public Quadtree(Vec2 bounds)
            => _totalBounds = bounds;
        public Quadtree(Vec2 bounds, List<IBoundable> items)
        {
            _totalBounds = bounds;
            Add(items);
        }
        
        public List<IBoundable> FindClosest(Vec2 point)
            => _head.FindClosest(point);
        public void Add(IBoundable value)
        {
            if (_head == null)
                _head = new QuadtreeNode(new BoundingRectangle(Vec2.Zero, _totalBounds), null);
            _head.Add(value);
        }
        public void Add(List<IBoundable> value)
        {
            if (_head == null)
                _head = new QuadtreeNode(new BoundingRectangle(Vec2.Zero, _totalBounds), null);
            _head.Add(value);
        }
        public bool Remove(IBoundable value)
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

        public class QuadtreeNode
        {
            private QuadtreeNode _parent;
            private BoundingRectangle _region;
            public List<IBoundable> _items = new List<IBoundable>();
            public QuadtreeNode[] _subNodes;
            
            public List<IBoundable> Items
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

            public List<IBoundable> FindClosest(Vec2 point)
            {
                if (_region.Contains(point))
                {
                    List<IBoundable> list = null;
                    foreach (QuadtreeNode node in _subNodes)
                        if (node != null)
                        {
                            list = node.FindClosest(point);
                            if (list != null)
                                return list;
                        }

                    if (_items.Count == 0)
                        return null;

                    list = new List<IBoundable>(_items);
                    for (int i = 0; i < list.Count; ++i)
                        if (!list[i].AxisAlignedBounds.Contains(point))
                            list.RemoveAt(i--);

                    return list;
                }
                else
                    return null;
            }
            public List<IBoundable> CollectChildren()
            {
                List<IBoundable> list = _items;
                foreach (QuadtreeNode node in _subNodes)
                    if (node != null)
                        list.AddRange(node.CollectChildren());
                return list;
            }
            public List<IBoundable> FindAll(BoundingRectangle bounds, EContainmentFlags containment)
            {
                List<IBoundable> list = new List<IBoundable>();

                //Not searching for anything?
                if (containment == EContainmentFlags.None)
                    return list;

                if (_region.Contains(bounds) == EContainment.Contains)
                {

                }

                return null;
            }
            private void FindAllInternal(RectangleF bounds, EContainmentFlags containment, List<IBoundable> found)
            {

            }
            /// <summary>
            /// shouldDestroy returns true if this node has no items contained within it or its subdivisions.
            /// returns true if the value was found and removed.
            /// </summary>
            public bool Remove(IBoundable value, out bool shouldDestroy)
            {
                bool hasBeenRemoved = false;
                if (_items.Contains(value))
                    hasBeenRemoved = _items.Remove(value);
                else if (_subNodes != null)
                {
                    bool anyNotNull = false;
                    for (int i = 0; i < 8; ++i)
                    {
                        QuadtreeNode node = _subNodes[i];
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
            public void Add(List<IBoundable> value)
            {
                bool notSubdivided = true;
                List<IBoundable> items;
                for (int i = 0; i < 4; ++i)
                {
                    items = new List<IBoundable>();
                    BoundingRectangle bounds = GetSubdivision(i);
                    foreach (IBoundable item in value)
                    {
                        if (item == null)
                            continue;
                        if (bounds.Contains(item.AxisAlignedBounds) == EContainment.Contains)
                        {
                            notSubdivided = false;

                            if (_subNodes == null)
                            {
                                _subNodes = new QuadtreeNode[4];
                                _subNodes[i] = new QuadtreeNode(bounds, this);
                            }
                            else if (_subNodes[i] == null)
                                _subNodes[i] = new QuadtreeNode(bounds, this);

                            items.Add(item);

                            break;
                        }
                    }
                    if (_subNodes[i] != null && items.Count > 0)
                        _subNodes[i].Add(items);
                }

                if (notSubdivided)
                    foreach (IBoundable b in value)
                        if (_parent == null || _region.Contains(b.AxisAlignedBounds) == EContainment.Contains)
                            Items.Add(b);
            }

            public void OnMoved(IBoundable item)
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
                    _parent?.OnMoved(item);
                }
            }

            public void Add(IBoundable item)
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
                            _subNodes = new QuadtreeNode[4];
                            _subNodes[i] = new QuadtreeNode(bounds, this);
                        }
                        else if (_subNodes[i] == null)
                            _subNodes[i] = new QuadtreeNode(bounds, this);

                        _subNodes[i].Add(item);
                        
                        break;
                    }
                }

                if (notSubdivided)
                    Items.Add(item);
            }
            public QuadtreeNode(BoundingRectangle bounds, QuadtreeNode parent)
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
                    foreach (QuadtreeNode node in _subNodes)
                        node?.Resize(scaleFactor);
            }
        }
    }
}
