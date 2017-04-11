using CustomEngine;
using CustomEngine.Rendering;
using CustomEngine.Rendering.Models;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Worlds.Actors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public interface IOctreeNode
    {
        bool Visible { get; set; }
        BoundingBox Bounds { get; }
        Vec3 Center { get; }
        Vec3 Min { get; }
        Vec3 Max { get; }
        void ItemMoved(I3DBoundable item);
        void DebugRender();
    }
    public class Octree : Octree<I3DBoundable>
    {
        public Octree(BoundingBox bounds) : base(bounds) { }
        public Octree(BoundingBox bounds, List<I3DBoundable> items) : base(bounds, items) { }
    }
    public class Octree<T> where T : I3DBoundable
    {
        private BoundingBox _totalBounds;
        private Node _head;
        
        public Octree(BoundingBox bounds) { _totalBounds = bounds; }
        public Octree(BoundingBox bounds, List<T> items)
        {
            _totalBounds = bounds;
            Add(items);
        }
        
        public void Cull(Frustum frustum) { _head?.Cull(frustum); }
        public List<T> FindClosest(Vec3 point) { return _head.FindClosest(point); }
        public List<T> FindAllJustOutside(Shape shape) { return _head.FindAllJustOutside(shape); }
        public List<T> FindAllInside(Shape shape) { return _head.FindAllInside(shape); }
        public void Add(T value)
        {
            if (_head == null)
                _head = new Node(_totalBounds);
            _head.Add(value, true);
        }
        public void Add(List<T> value)
        {
            if (_head == null)
                _head = new Node(_totalBounds);
            _head.Add(value, true);
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

        public void DebugRender()
        {
            _head?.DebugRender();
        }

        public class Node : IOctreeNode
        {
            private int _subDivIndex = 0, _subDivLevel = 0;
            private bool _visible = true;
            private BoundingBox _bounds;
            public List<T> _items = new List<T>();
            public Node[] _subNodes;
            public Node _parentNode;

            public List<T> Items => _items;
            public BoundingBox Bounds => _bounds;
            public Vec3 Center => _bounds.Translation;
            public Vec3 Min => _bounds.Minimum;
            public Vec3 Max => _bounds.Maximum;

            public bool Visible
            {
                get { return _visible; }
                set
                {
                    if (_visible == value)
                        return;
                    _visible = value;
                    foreach (T item in _items)
                        item.IsRendering = _visible;
                    if (_subNodes != null)
                        foreach (Node node in _subNodes)
                            if (node != null)
                                node.Visible = _visible;
                }
            }
            public void DebugRender()
            {
                Engine.Renderer.RenderAABB("OctSubDiv" + _subDivLevel + "-" + _subDivIndex, _bounds.HalfExtents, _bounds.Translation, false, Color.Gray, 5.0f);
                if (_subNodes != null)
                    foreach (Node n in _subNodes)
                        n?.DebugRender();
            }
            public void ItemMoved(I3DBoundable item) => ItemMoved((T)item);
            public void ItemMoved(T item)
            {
                //Still within the same volume?
                if (item.CullingVolume.ContainedWithin(_bounds) == EContainment.Contains)
                {
                    //Try subdividing
                    for (int i = 0; i < 8; ++i)
                    {
                        BoundingBox bounds = GetSubdivision(i);
                        if (item.CullingVolume.ContainedWithin(bounds) == EContainment.Contains)
                        {
                            _items.Remove(item);
                            CreateSubNode(bounds, i);
                            _subNodes[i].Add(item);
                            break;
                        }
                    }
                    return;
                }

                //Belongs in larger parent volume, remove from this node
                if (_parentNode != null)
                {
                    Remove(item, out bool shouldDestroy);
                    if (!_parentNode.AddReversedHierarchy(item, shouldDestroy ? _subDivIndex : -1))
                    {
                        //Force add to root node
                        Node m = this;
                        while (m._parentNode != null)
                            m = m._parentNode;
                        m.Items.Add(item);
                        item.RenderNode = m;
                        item.IsRendering = m._visible;
                    }
                }
            }
            /// <summary>
            /// Returns true if the item was added to this node (clarification: not its subnodes)
            /// </summary>
            private bool AddReversedHierarchy(T item, int childDestroyIndex)
            {
                if (!Add(item))
                {
                    bool shouldDestroy = _items.Count == 0 && HasNoSubNodesExcept(childDestroyIndex);
                    if (_parentNode != null)
                    {
                        if (_parentNode.AddReversedHierarchy(item, shouldDestroy ? _subDivIndex : -1))
                        {
                            if (childDestroyIndex >= 0 &&
                                _subNodes != null &&
                                _subNodes[childDestroyIndex] != null)
                            {
                                _subNodes[childDestroyIndex] = null;
                                if (!_subNodes.Any(x => x != null))
                                    _subNodes = null;
                            }
                            return true;
                        }
                    }
                }
                else
                {
                    if (childDestroyIndex >= 0 &&
                        _subNodes != null &&
                        _subNodes[childDestroyIndex] != null)
                    {
                        _subNodes[childDestroyIndex] = null;
                        if (!_subNodes.Any(x => x != null))
                            _subNodes = null;
                    }
                    return true;
                }

                if (childDestroyIndex >= 0 &&
                    _subNodes != null &&
                    _subNodes[childDestroyIndex] != null)
                {
                    _subNodes[childDestroyIndex] = null;
                    if (!_subNodes.Any(x => x != null))
                        _subNodes = null;
                }
                return false;
            }

            private bool HasNoSubNodesExcept(int index)
            {
                if (_subNodes == null)
                    return true;
                for (int i = 0; i < 8; ++i)
                    if (i != index && _subNodes[i] != null)
                        return false;
                return true;
            }

            public List<T> FindClosest(Vec3 point)
            {
                if (_bounds.Contains(point))
                {
                    List<T> list = null;
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

                    list = new List<T>(_items);
                    for (int i = 0; i < list.Count; ++i)
                        if (list[i].CullingVolume != null && !list[i].CullingVolume.Contains(point))
                            list.RemoveAt(i--);

                    return list;
                }
                else
                    return null;
            }
            public List<T> FindAllJustOutside(Shape shape)
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
            public List<T> CollectChildren()
            {
                List<T> list = _items;
                foreach (Node node in _subNodes)
                    if (node != null)
                        list.AddRange(node.CollectChildren());
                return list;
            }
            public List<T> FindAllInside(Shape shape)
            {
                throw new NotImplementedException();
            }
            internal void Cull(Frustum frustum)
            {
                EContainment c = frustum.Contains(_bounds);
                if (c == EContainment.Contains)
                    Visible = true;
                else if (c == EContainment.Disjoint)
                    Visible = false;
                else
                {
                    //Bounds is intersecting edge of frustum
                    for (int i = 0; i < _items.Count; ++i)
                    {
                        if (i > _items.Count)
                            break;

                        T item = _items[i];
                        item.IsRendering = item.CullingVolume != null ?
                            item.CullingVolume.ContainedWithin(frustum) != EContainment.Disjoint : true;
                    }

                    if (_subNodes != null)
                        foreach (Node n in _subNodes)
                            n?.Cull(frustum);
                }
            }
            /// <summary>
            /// shouldDestroy returns true if this node has no items contained within it or its subdivisions.
            /// returns true if the value was found and removed.
            /// </summary>
            //public bool Remove(I3DBoundable value) => Remove((T)value);
            //public bool Remove(T value)
            //{
            //    bool removed = Remove(value, out bool shouldDestroy);
            //}
            internal bool Remove(T value, out bool shouldDestroy)
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
            private void CreateSubNode(BoundingBox bounds, int index)
            {
                if (_subNodes == null)
                    _subNodes = new Node[8];
                else if (_subNodes[index] != null)
                    return;

                Node node = bounds;
                node.Visible = Visible;
                node._subDivIndex = index;
                node._subDivLevel = _subDivLevel + 1;
                node._parentNode = this;
                _subNodes[index] = node;
            }
            public bool Add(List<T> items, bool force = false)
            {
                bool addedAny = false;
                foreach (T item in items)
                    addedAny = addedAny || Add(item, force);
                return addedAny;

                //bool notSubdivided = true;
                //List<T> items;
                //for (int i = 0; i < 8; ++i)
                //{
                //    items = new List<T>();
                //    BoundingBox bounds = GetSubdivision(i);
                //    foreach (T item in value)
                //    {
                //        if (item == null)
                //            continue;
                //        if (item.CullingVolume.ContainedWithin(bounds) == EContainment.Contains)
                //        {
                //            notSubdivided = false;
                //            CreateSubNode(bounds, i);
                //            items.Add(item);
                //            break;
                //        }
                //    }
                //    if (_subNodes != null && _subNodes[i] != null && items.Count > 0)
                //        _subNodes[i].Add(items);
                //}

                //if (notSubdivided)
                //{
                //    Items.AddRange(value);
                //    foreach (var v in value)
                //    {
                //        v.IsRendering = _visible;
                //        v.RenderNode = this;
                //    }
                //}
            }
            public bool Add(T item, bool force = false)
            {
                if (item == null)
                    return false;

                bool notSubdivided = true;
                if (item.CullingVolume != null)
                {
                    if (item.CullingVolume.ContainedWithin(_bounds) != EContainment.Contains)
                    {
                        if (force)
                        {
                            Items.Add(item);
                            item.IsRendering = _visible;
                            item.RenderNode = this;
                            return true;
                        }
                        return false;
                    }
                    for (int i = 0; i < 8; ++i)
                    {
                        BoundingBox bounds = GetSubdivision(i);
                        if (item.CullingVolume.ContainedWithin(bounds) == EContainment.Contains)
                        {
                            notSubdivided = false;
                            CreateSubNode(bounds, i);
                            _subNodes[i].Add(item);
                            break;
                        }
                    }
                }
                if (notSubdivided)
                {
                    Items.Add(item);
                    item.IsRendering = _visible;
                    item.RenderNode = this;
                }

                return true;
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
                    case 0: return BoundingBox.FromMinMax(new Vec3(Min.X, Min.Y, Min.Z), new Vec3(center.X, center.Y, center.Z));
                    case 1: return BoundingBox.FromMinMax(new Vec3(Min.X, Min.Y, center.Z), new Vec3(center.X, center.Y, Max.Z));
                    case 2: return BoundingBox.FromMinMax(new Vec3(Min.X, center.Y, Min.Z), new Vec3(center.X, Max.Y, center.Z));
                    case 3: return BoundingBox.FromMinMax(new Vec3(Min.X, center.Y, center.Z), new Vec3(center.X, Max.Y, Max.Z));
                    case 4: return BoundingBox.FromMinMax(new Vec3(center.X, Min.Y, Min.Z), new Vec3(Max.X, center.Y, center.Z));
                    case 5: return BoundingBox.FromMinMax(new Vec3(center.X, Min.Y, center.Z), new Vec3(Max.X, center.Y, Max.Z));
                    case 6: return BoundingBox.FromMinMax(new Vec3(center.X, center.Y, Min.Z), new Vec3(Max.X, Max.Y, center.Z));
                    case 7: return BoundingBox.FromMinMax(new Vec3(center.X, center.Y, center.Z), new Vec3(Max.X, Max.Y, Max.Z));
                }
                return null;
            }
            //public void Render()
            //{
            //    if (!_visible)
            //        return;
            //    foreach (T r in _items)
            //    {
            //        //int t = Engine.StartTimer();
            //        r.Render();
            //        //float time = Engine.EndTimer(t);
            //        //if (time > 0.0f)
            //        //    Debug.WriteLine(r.ToString() + " took " + time + " seconds");
            //    }
            //    if (_subNodes != null)
            //        foreach (Node node in _subNodes)
            //            node?.Render();
            //}
            public static implicit operator Node(BoundingBox bounds) { return new Node(bounds); }
        }
    }
}
