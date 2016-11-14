using CustomEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public class RenderOctree
    {
        Box _totalBounds;
        OctreeNode _head;
        
        public RenderOctree(Box bounds) { _totalBounds = bounds; }

        public void Render() { _head?.Render(); }
        public void Cull(Frustrum frustrum) { _head?.Cull(frustrum); }
        public List<IRenderable> FindClosest(Vec3 point) { return _head.FindClosest(point); }
        public void Add(IRenderable value)
        {
            if (_head == null)
                _head = new OctreeNode(_totalBounds);
            _head.Add(value);
        }
        public bool Remove(IRenderable value)
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
            private bool _visible;
            private Box _bounds;
            public List<IRenderable> _items = new List<IRenderable>();
            public OctreeNode[] _subNodes;
            
            public List<IRenderable> Items { get { return _items; } }
            public Box Bounds { get { return _bounds; } }
            public Vec3 Center { get { return _bounds.CenterPoint; } }
            public Vec3 Min { get { return _bounds.Minimum; } }
            public Vec3 Max { get { return _bounds.Maximum; } }

            public bool Visible
            {
                get { return _visible; }
                set
                {
                    if (_visible == value)
                        return;

                    _visible = value;
                    foreach (IRenderable item in _items)
                        item.IsRendering = _visible;
                    if (_subNodes != null)
                        foreach (OctreeNode node in _subNodes)
                            if (node != null)
                                node.Visible = _visible;
                }
            }
            public List<IRenderable> FindClosest(Vec3 point)
            {
                if (_bounds.Contains(point))
                {
                    List<IRenderable> list = null;
                    foreach (OctreeNode node in _subNodes)
                        if (node != null)
                        {
                            list = node.FindClosest(point);
                            if (list != null)
                                return list;
                        }

                    if (_items.Count == 0)
                        return null;

                    list = new List<IRenderable>(_items);
                    for (int i = 0; i < list.Count; ++i)
                        if (!list[i].CullingVolume.Contains(point))
                            list.RemoveAt(i--);

                    return list;
                }
                else
                    return null;
            }
            public void Cull(Frustrum frustrum)
            {
                EContainment c = _bounds.WithinFrustrum(frustrum);
                if (c == EContainment.Contains)
                    Visible = true;
                else if (c == EContainment.Disjoint)
                    Visible = false;
                else //Bounds is intersecting edge of frustrum
                {
                    foreach (IRenderable item in _items)
                        item.IsRendering = item.CullingVolume.WithinFrustrum(frustrum) != EContainment.Disjoint;
                    if (_subNodes != null)
                        foreach (OctreeNode n in _subNodes)
                            n.Cull(frustrum);
                }
            }
            /// <summary>
            /// shouldDestroy returns true if this node has no items contained within it or its subdivisions.
            /// returns true if the value was found and removed.
            /// </summary>
            public bool Remove(IRenderable value, out bool shouldDestroy)
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
            public void Add(IRenderable item)
            {
                if (item == null)
                    return;

                bool notSubdivided = true;
                for (int i = 0; i < 8; ++i)
                {
                    Box bounds = GetSubdivision(i);
                    if (bounds.Contains(item.CullingVolume) == EContainment.Contains)
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
            public void Render()
            {
                foreach (IRenderable r in _items)
                    r.Render();
                if (_subNodes != null)
                    foreach (OctreeNode node in _subNodes)
                        node.Render();
            }
            public static implicit operator OctreeNode(Box bounds) { return new OctreeNode(bounds); }
        }
    }
}
