using CustomEngine;
using CustomEngine.Rendering;
using CustomEngine.Rendering.Models;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Worlds.Actors.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public interface IMesh
    {
        Shape CullingVolume { get; }
        PrimitiveManager PrimitiveManager { get; }
        bool VisibleByDefault { get; }
    }
    public interface IRenderable
    {
        Shape CullingVolume { get; }
        bool IsRendering { get; set; }
        RenderOctree.OctreeNode RenderNode { get; set; }
        bool Visible { get; set; }
        void Render();
    }
    public class RenderOctree
    {
        private BoundingBox _totalBounds;
        private OctreeNode _head;
        
        public RenderOctree(BoundingBox bounds) { _totalBounds = bounds; }
        public RenderOctree(BoundingBox bounds, List<IMesh> items)
        {
            _totalBounds = bounds;
            Add(items);
        }

        public void Render() { _head?.Render(); }
        public void Cull(Frustum frustum) { _head?.Cull(frustum); }
        public List<IMesh> FindClosest(Vec3 point) { return _head.FindClosest(point); }
        public List<IMesh> FindAllJustOutside(Shape shape) { return _head.FindAllJustOutside(shape); }
        public List<IMesh> FindAllInside(Shape shape) { return _head.FindAllInside(shape); }
        public void Add(IMesh value)
        {
            if (_head == null)
                _head = new OctreeNode(_totalBounds);
            _head.Add(value);
        }
        public void Add(List<IMesh> value)
        {
            if (_head == null)
                _head = new OctreeNode(_totalBounds);
            _head.Add(value);
        }
        public bool Remove(IMesh value)
        {
            if (_head == null)
                return false;

            bool destroy;
            bool removed = _head.Remove(value, out destroy);
            if (destroy)
                _head = null;
            return removed;
        }

        public class OctreeNode
        {
            private bool _visible = true;
            private BoundingBox _bounds;
            public List<IMesh> _items = new List<IMesh>();
            public OctreeNode[] _subNodes;
            
            public List<IMesh> Items { get { return _items; } }
            public BoundingBox Bounds { get { return _bounds; } }
            public Vec3 Center { get { return _bounds.Translation; } }
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
                    foreach (IMesh item in _items)
                        item.IsRendering = _visible;
                    if (_subNodes != null)
                        foreach (OctreeNode node in _subNodes)
                            if (node != null)
                                node.Visible = _visible;
                }
            }
            public List<IMesh> FindClosest(Vec3 point)
            {
                if (_bounds.Contains(point))
                {
                    List<IMesh> list = null;
                    foreach (OctreeNode node in _subNodes)
                        if (node != null)
                        {
                            list = node.FindClosest(point);
                            if (list != null)
                                return list;
                        }

                    if (_items.Count == 0)
                        return null;

                    list = new List<IMesh>(_items);
                    for (int i = 0; i < list.Count; ++i)
                        if (!list[i].CullingVolume.Contains(point))
                            list.RemoveAt(i--);

                    return list;
                }
                else
                    return null;
            }
            public List<IMesh> FindAllJustOutside(Shape shape)
            {
                foreach (OctreeNode node in _subNodes)
                    if (node != null)
                    {
                        EContainment c = shape.ContainedWithin(node._bounds);
                        if (c == EContainment.Contains)
                            return node.FindAllJustOutside(shape);
                    }

                return CollectChildren();
            }
            public List<IMesh> CollectChildren()
            {
                List<IMesh> list = _items;
                foreach (OctreeNode node in _subNodes)
                    if (node != null)
                        list.AddRange(node.CollectChildren());
                return list;
            }
            public List<IMesh> FindAllInside(Shape shape)
            {
                throw new NotImplementedException();
            }
            public void Cull(Frustum frustum)
            {
                EContainment c = frustum.Contains(_bounds);
                if (c == EContainment.Contains)
                    Visible = true;
                //else if (c == EContainment.Disjoint)
                //    Visible = false;
                else
                {
                    //Bounds is intersecting edge of frustum
                    foreach (IMesh item in _items)
                    {
                        EContainment containment = item.CullingVolume.ContainedWithin(frustum);
                        item.IsRendering = containment != EContainment.Disjoint;
                    }
                    if (_subNodes != null)
                        foreach (OctreeNode n in _subNodes)
                            n?.Cull(frustum);
                }
            }
            /// <summary>
            /// shouldDestroy returns true if this node has no items contained within it or its subdivisions.
            /// returns true if the value was found and removed.
            /// </summary>
            public bool Remove(IMesh value, out bool shouldDestroy)
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
            public void Add(List<IMesh> value)
            {
                bool notSubdivided = true;
                List<IMesh> items;
                for (int i = 0; i < 8; ++i)
                {
                    items = new List<IMesh>();
                    BoundingBox bounds = GetSubdivision(i);
                    foreach (IMesh item in value)
                    {
                        if (item == null)
                            continue;
                        if (item.CullingVolume.ContainedWithin(bounds) == EContainment.Contains)
                        {
                            notSubdivided = false;

                            if (_subNodes == null)
                            {
                                _subNodes = new OctreeNode[8];
                                _subNodes[i] = bounds;
                                _subNodes[i].Visible = Visible;
                            }
                            else if (_subNodes[i] == null)
                            {
                                _subNodes[i] = bounds;
                                _subNodes[i].Visible = Visible;
                            }

                            items.Add(item);

                            break;
                        }
                    }
                    if (_subNodes != null && _subNodes[i] != null && items.Count > 0)
                        _subNodes[i].Add(items);
                }

                if (notSubdivided)
                {
                    Items.AddRange(value);
                    foreach (var v in value)
                    {
                        v.IsRendering = _visible;
                        v.RenderNode = this;
                    }
                }
            }
            public void Add(IMesh item)
            {
                if (item == null)
                    return;

                bool notSubdivided = true;
                for (int i = 0; i < 8; ++i)
                {
                    BoundingBox bounds = GetSubdivision(i);
                    if (item.CullingVolume.ContainedWithin(bounds) == EContainment.Contains)
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
                {
                    Items.Add(item);
                    item.IsRendering = _visible;
                    item.RenderNode = this;
                }
            }
            public OctreeNode(BoundingBox bounds)
            {
                _bounds = bounds;
            }
            public BoundingBox GetSubdivision(int index)
            {
                if (_subNodes != null && _subNodes[index] != null)
                    return _subNodes[index].Bounds;

                Vec3 center = Center;
                switch (index)
                {
                    case 0: return new BoundingBox(new Vec3(Min.X, Min.Y, Min.Z), new Vec3(center.X, center.Y, center.Z));
                    case 1: return new BoundingBox(new Vec3(Min.X, Min.Y, center.Z), new Vec3(center.X, center.Y, Max.Z));
                    case 2: return new BoundingBox(new Vec3(Min.X, center.Y, Min.Z), new Vec3(center.X, Max.Y, center.Z));
                    case 3: return new BoundingBox(new Vec3(Min.X, center.Y, center.Z), new Vec3(center.X, Max.Y, Max.Z));
                    case 4: return new BoundingBox(new Vec3(center.X, Min.Y, Min.Z), new Vec3(Max.X, center.Y, center.Z));
                    case 5: return new BoundingBox(new Vec3(center.X, Min.Y, center.Z), new Vec3(Max.X, center.Y, Max.Z));
                    case 6: return new BoundingBox(new Vec3(center.X, center.Y, Min.Z), new Vec3(Max.X, Max.Y, center.Z));
                    case 7: return new BoundingBox(new Vec3(center.X, center.Y, center.Z), new Vec3(Max.X, Max.Y, Max.Z));
                }
                return null;
            }
            public void Render()
            {
                foreach (IMesh r in _items)
                    r.Render();
                if (_subNodes != null)
                    foreach (OctreeNode node in _subNodes)
                        node?.Render();
            }
            public static implicit operator OctreeNode(BoundingBox bounds) { return new OctreeNode(bounds); }
        }
    }
}
