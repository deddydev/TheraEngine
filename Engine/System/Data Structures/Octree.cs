using CustomEngine;
using CustomEngine.Rendering;
using CustomEngine.Rendering.Models;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Worlds.Actors.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public class Octree
    {
        private BoundingBox _totalBounds;
        private Node _head;
        
        public Octree(BoundingBox bounds) { _totalBounds = bounds; }
        public Octree(BoundingBox bounds, List<I3DBoundable> items)
        {
            _totalBounds = bounds;
            Add(items);
        }
        
        public void Cull(Frustum frustum) { _head?.Cull(frustum); }
        public List<I3DBoundable> FindClosest(Vec3 point) { return _head.FindClosest(point); }
        public List<I3DBoundable> FindAllJustOutside(Shape shape) { return _head.FindAllJustOutside(shape); }
        public List<I3DBoundable> FindAllInside(Shape shape) { return _head.FindAllInside(shape); }
        public void Add(I3DBoundable value)
        {
            if (_head == null)
                _head = new Node(_totalBounds);
            _head.Add(value);
        }
        public void Add(List<I3DBoundable> value)
        {
            if (_head == null)
                _head = new Node(_totalBounds);
            _head.Add(value);
        }
        public bool Remove(I3DBoundable value)
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

        public class Node
        {
            private int _subDivIndex = 0, _subDivLevel = 0;
            private bool _visible = true;
            private BoundingBox _bounds;
            public List<I3DBoundable> _items = new List<I3DBoundable>();
            public Node[] _subNodes;
            public Node _parentNode;

            public List<I3DBoundable> Items => _items;
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
                    foreach (I3DBoundable item in _items)
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

            public void ItemMoved(I3DBoundable item)
            {
                Remove(item, out bool shouldDestroy);
                if (!Add(item) && _parentNode != null)
                {
                    //if (shouldDestroy && _parentNode != null)
                    //{
                    //    _parentNode._subNodes[_subDivIndex] = null;
                    //    if (!(_parentNode._subNodes.Any(x => x != null)))
                    //        _parentNode._subNodes = null;
                    //}
                    shouldDestroy = _items.Count == 0 && _subNodes == null;
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
                else //Force add back to this node
                {
                    Items.Add(item);
                    item.RenderNode = this;
                    item.IsRendering = _visible;
                }
            }
            /// <summary>
            /// Returns true if the item was added to this node (clarification: not its subnodes)
            /// </summary>
            private bool AddReversedHierarchy(I3DBoundable item, int childDestroyIndex)
            {
                if (item.CullingVolume.ContainedWithin(_bounds) == EContainment.Contains)
                {
                    Add(item);
                    //Items.Add(item);
                    //item.IsRendering = _visible;
                    //item.RenderNode = this;

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

            public List<I3DBoundable> FindClosest(Vec3 point)
            {
                if (_bounds.Contains(point))
                {
                    List<I3DBoundable> list = null;
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

                    list = new List<I3DBoundable>(_items);
                    for (int i = 0; i < list.Count; ++i)
                        if (list[i].CullingVolume != null && !list[i].CullingVolume.Contains(point))
                            list.RemoveAt(i--);

                    return list;
                }
                else
                    return null;
            }
            public List<I3DBoundable> FindAllJustOutside(Shape shape)
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
            public List<I3DBoundable> CollectChildren()
            {
                List<I3DBoundable> list = _items;
                foreach (Node node in _subNodes)
                    if (node != null)
                        list.AddRange(node.CollectChildren());
                return list;
            }
            public List<I3DBoundable> FindAllInside(Shape shape)
            {
                throw new NotImplementedException();
            }
            public void Cull(Frustum frustum)
            {
                EContainment c = frustum.Contains(_bounds);
                if (c == EContainment.Contains)
                    Visible = true;
                else if (c == EContainment.Disjoint)
                    Visible = false;
                else
                {
                    //Bounds is intersecting edge of frustum
                    foreach (I3DBoundable item in _items)
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
            public bool Remove(I3DBoundable value, out bool shouldDestroy)
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
            public bool Add(List<I3DBoundable> items)
            {
                bool addedAny = false;
                foreach (I3DBoundable item in items)
                    addedAny = addedAny || Add(item);
                return addedAny;

                //bool notSubdivided = true;
                //List<I3DBoundable> items;
                //for (int i = 0; i < 8; ++i)
                //{
                //    items = new List<I3DBoundable>();
                //    BoundingBox bounds = GetSubdivision(i);
                //    foreach (I3DBoundable item in value)
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
            public bool Add(I3DBoundable item)
            {
                if (item == null)
                    return false;

                bool notSubdivided = true;
                if (item.CullingVolume != null)
                {
                    if (item.CullingVolume.ContainedWithin(_bounds) != EContainment.Contains)
                        return false;

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
            //    foreach (I3DBoundable r in _items)
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
