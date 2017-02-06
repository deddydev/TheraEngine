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
    public interface IStaticMesh
    {
        Shape CullingVolume { get; }
        PrimitiveData Data { get; }
        Material Material { get; set; }
        bool VisibleByDefault { get; }
    }
    public interface ISkeletalMesh
    {
        string SingleBindName { get; }
        PrimitiveData Data { get; }
        Material Material { get; set; }
        bool VisibleByDefault { get; }
    }
    public interface IRenderable
    {
        Shape CullingVolume { get; }
        bool IsRendering { get; set; }
        RenderOctree.Node RenderNode { get; set; }
        bool Visible { get; set; }
        void Render();
    }
    public class RenderOctree
    {
        private BoundingBox _totalBounds;
        private Node _head;
        
        public RenderOctree(BoundingBox bounds) { _totalBounds = bounds; }
        public RenderOctree(BoundingBox bounds, List<IRenderable> items)
        {
            _totalBounds = bounds;
            Add(items);
        }

        public void Render() { _head?.Render(); }
        public void Cull(Frustum frustum) { _head?.Cull(frustum); }
        public List<IRenderable> FindClosest(Vec3 point) { return _head.FindClosest(point); }
        public List<IRenderable> FindAllJustOutside(Shape shape) { return _head.FindAllJustOutside(shape); }
        public List<IRenderable> FindAllInside(Shape shape) { return _head.FindAllInside(shape); }
        public void Add(IRenderable value)
        {
            if (_head == null)
                _head = new Node(_totalBounds);
            _head.Add(value);
        }
        public void Add(List<IRenderable> value)
        {
            if (_head == null)
                _head = new Node(_totalBounds);
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

        public class Node
        {
            private bool _visible = true;
            private BoundingBox _bounds;
            public List<IRenderable> _items = new List<IRenderable>();
            public Node[] _subNodes;
            
            public List<IRenderable> Items { get { return _items; } }
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
                    foreach (IRenderable item in _items)
                        item.IsRendering = _visible;
                    if (_subNodes != null)
                        foreach (Node node in _subNodes)
                            if (node != null)
                                node.Visible = _visible;
                }
            }
            public List<IRenderable> FindClosest(Vec3 point)
            {
                if (_bounds.Contains(point))
                {
                    List<IRenderable> list = null;
                    if (_subNodes != null)
                        foreach (Node node in _subNodes)
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
                        if (list[i].CullingVolume != null && !list[i].CullingVolume.Contains(point))
                            list.RemoveAt(i--);

                    return list;
                }
                else
                    return null;
            }
            public List<IRenderable> FindAllJustOutside(Shape shape)
            {
                foreach (Node node in _subNodes)
                    if (node != null)
                    {
                        EContainment c = shape.ContainedWithin(node._bounds);
                        if (c == EContainment.Contains)
                            return node.FindAllJustOutside(shape);
                    }

                return CollectChildren();
            }
            public List<IRenderable> CollectChildren()
            {
                List<IRenderable> list = _items;
                foreach (Node node in _subNodes)
                    if (node != null)
                        list.AddRange(node.CollectChildren());
                return list;
            }
            public List<IRenderable> FindAllInside(Shape shape)
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
                    foreach (IRenderable item in _items)
                        item.IsRendering = item.CullingVolume != null ? item.CullingVolume.ContainedWithin(frustum) != EContainment.Disjoint : true;

                    if (_subNodes != null)
                        foreach (Node n in _subNodes)
                            n?.Cull(frustum);
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
                        Node node = _subNodes[i];
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
            public void Add(List<IRenderable> value)
            {
                bool notSubdivided = true;
                List<IRenderable> items;
                for (int i = 0; i < 8; ++i)
                {
                    items = new List<IRenderable>();
                    BoundingBox bounds = GetSubdivision(i);
                    foreach (IRenderable item in value)
                    {
                        if (item == null)
                            continue;
                        if (item.CullingVolume.ContainedWithin(bounds) == EContainment.Contains)
                        {
                            notSubdivided = false;

                            if (_subNodes == null)
                            {
                                _subNodes = new Node[8];
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
            public void Add(IRenderable item)
            {
                if (item == null)
                    return;

                bool notSubdivided = true;

                if (item.CullingVolume != null)
                    for (int i = 0; i < 8; ++i)
                    {
                        BoundingBox bounds = GetSubdivision(i);
                        if (item.CullingVolume.ContainedWithin(bounds) == EContainment.Contains)
                        {
                            notSubdivided = false;

                            if (_subNodes == null)
                            {
                                _subNodes = new Node[8];
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
            public Node(BoundingBox bounds)
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
                if (!_visible)
                    return;
                foreach (IRenderable r in _items)
                    r.Render();
                if (_subNodes != null)
                    foreach (Node node in _subNodes)
                        node?.Render();
            }
            public static implicit operator Node(BoundingBox bounds) { return new Node(bounds); }
        }
    }
}
