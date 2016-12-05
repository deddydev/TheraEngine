using CustomEngine.Rendering;
using CustomEngine.Rendering.Models;
using System.Collections.Generic;

namespace System
{
    public class TriangleOctree
    {
        private Box _totalBounds;
        private OctreeNode _head;
        
        public TriangleOctree(Box bounds) { _totalBounds = bounds; }
        public TriangleOctree(Box bounds, List<VertexTriangle> items)
        {
            _totalBounds = bounds;
            Add(items);
        }
        
        public List<VertexTriangle> FindClosest(Vec3 point) { return _head.FindClosest(point); }
        public List<VertexTriangle> FindAllJustOutside(Shape shape) { return _head.FindAllJustOutside(shape); }
        public List<VertexTriangle> FindAllInside(Shape shape) { return _head.FindAllInside(shape); }
        public void Add(VertexTriangle value)
        {
            if (_head == null)
                _head = new OctreeNode(_totalBounds);
            _head.Add(value);
        }
        public void Add(List<VertexTriangle> value)
        {
            if (_head == null)
                _head = new OctreeNode(_totalBounds);
            _head.Add(value);
        }
        public bool Remove(VertexTriangle value)
        {
            if (_head == null)
                return false;

            bool destroy;
            bool removed = _head.Remove(value, out destroy);
            if (destroy)
                _head = null;
            return removed;
        }

        internal class OctreeNode
        {
            private Box _bounds;
            public List<VertexTriangle> _items = new List<VertexTriangle>();
            public OctreeNode[] _subNodes;
            
            public List<VertexTriangle> Items { get { return _items; } }
            public Box Bounds { get { return _bounds; } }
            public Vec3 Center { get { return _bounds.CenterPoint; } }
            public Vec3 Min { get { return _bounds.Minimum; } }
            public Vec3 Max { get { return _bounds.Maximum; } }
            
            public List<VertexTriangle> FindClosest(Vec3 point)
            {
                if (_bounds.Contains(point))
                {
                    List<VertexTriangle> list = null;
                    foreach (OctreeNode node in _subNodes)
                        if (node != null)
                        {
                            list = node.FindClosest(point);
                            if (list != null)
                                return list;
                        }

                    if (_items.Count == 0)
                        return null;

                    list = new List<VertexTriangle>(_items);
                    for (int i = 0; i < list.Count; ++i)
                        if (!list[i].GetCullingVolume().Contains(point))
                            list.RemoveAt(i--);

                    return list;
                }
                else
                    return null;
            }
            public List<VertexTriangle> FindAllJustOutside(Shape shape)
            {
                foreach (OctreeNode node in _subNodes)
                    if (node != null)
                    {
                        EContainment c = node._bounds.Contains(shape);
                        if (c == EContainment.Contains)
                            return node.FindAllJustOutside(shape);
                    }

                return CollectChildren();
            }
            public List<VertexTriangle> CollectChildren()
            {
                List<VertexTriangle> list = _items;
                foreach (OctreeNode node in _subNodes)
                    if (node != null)
                        list.AddRange(node.CollectChildren());
                return list;
            }
            public List<VertexTriangle> FindAllInside(Shape shape)
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// shouldDestroy returns true if this node has no items contained within it or its subdivisions.
            /// returns true if the value was found and removed.
            /// </summary>
            public bool Remove(VertexTriangle value, out bool shouldDestroy)
            {
                bool hasBeenRemoved = false;
                if (_items.Contains(value))
                    hasBeenRemoved = _items.Remove(value);
                else if (_subNodes != null)
                {
                    bool anyNotNull = false;
                    for (int i = 0; i < 8; ++i)
                    {
                        OctreeNode node = _subNodes[i];
                        if (node != null)
                        {
                            if (hasBeenRemoved && anyNotNull)
                                break;
                            else
                            {
                                bool remove;
                                if (hasBeenRemoved = node.Remove(value, out remove))
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
            public void Add(List<VertexTriangle> value)
            {
                bool notSubdivided = true;
                List<VertexTriangle> items;
                for (int i = 0; i < 8; ++i)
                {
                    items = new List<VertexTriangle>();
                    Box bounds = GetSubdivision(i);
                    foreach (VertexTriangle item in value)
                    {
                        if (item == null)
                            continue;
                        if (bounds.Contains(item.GetCullingVolume()) == EContainment.Contains)
                        {
                            notSubdivided = false;

                            if (_subNodes == null)
                            {
                                _subNodes = new OctreeNode[8];
                                _subNodes[i] = bounds;
                            }
                            else if (_subNodes[i] == null)
                                _subNodes[i] = bounds;

                            items.Add(item);

                            break;
                        }
                    }
                    if (_subNodes[i] != null && items.Count > 0)
                        _subNodes[i].Add(items);
                }

                if (notSubdivided)
                    Items.AddRange(value);
            }
            public void Add(VertexTriangle item)
            {
                if (item == null)
                    return;

                bool notSubdivided = true;
                for (int i = 0; i < 8; ++i)
                {
                    Box bounds = GetSubdivision(i);
                    if (bounds.Contains(item.GetCullingVolume()) == EContainment.Contains)
                    {
                        notSubdivided = false;

                        if (_subNodes == null)
                        {
                            _subNodes = new OctreeNode[8];
                            _subNodes[i] = bounds;
                        }
                        else if (_subNodes[i] == null)
                            _subNodes[i] = bounds;

                        _subNodes[i].Add(item);

                        break;
                    }
                }

                if (notSubdivided)
                    Items.Add(item);
            }
            public OctreeNode(Box bounds)
            {
                _bounds = bounds;
            }
            public Box GetSubdivision(int index)
            {
                if (_subNodes != null && _subNodes[index] != null)
                    return _subNodes[index].Bounds;

                Vec3 center = Center;
                switch (index)
                {
                    case 0: return new Box(new Vec3(Min.X, Min.Y, Min.Z), new Vec3(center.X, center.Y, center.Z));
                    case 1: return new Box(new Vec3(Min.X, Min.Y, center.Z), new Vec3(center.X, center.Y, Max.Z));
                    case 2: return new Box(new Vec3(Min.X, center.Y, Min.Z), new Vec3(center.X, Max.Y, center.Z));
                    case 3: return new Box(new Vec3(Min.X, center.Y, center.Z), new Vec3(center.X, Max.Y, Max.Z));
                    case 4: return new Box(new Vec3(center.X, Min.Y, Min.Z), new Vec3(Max.X, center.Y, center.Z));
                    case 5: return new Box(new Vec3(center.X, Min.Y, center.Z), new Vec3(Max.X, center.Y, Max.Z));
                    case 6: return new Box(new Vec3(center.X, center.Y, Min.Z), new Vec3(Max.X, Max.Y, center.Z));
                    case 7: return new Box(new Vec3(center.X, center.Y, center.Z), new Vec3(Max.X, Max.Y, Max.Z));
                }
                return null;
            }
            public static implicit operator OctreeNode(Box bounds) { return new OctreeNode(bounds); }
        }
    }
}
