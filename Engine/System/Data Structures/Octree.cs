using CustomEngine;
using CustomEngine.Rendering;
using CustomEngine.Rendering.Models;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Worlds.Actors;
using System;
using System.Collections.Concurrent;
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
        public ThreadSafeList<T> FindClosest(Vec3 point) { return _head.FindClosest(point); }
        public ThreadSafeList<T> FindAllJustOutside(Shape shape) { return _head.FindAllJustOutside(shape); }
        public ThreadSafeList<T> FindAllInside(Shape shape) { return _head.FindAllInside(shape); }
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
            => _head?.DebugRender();
        
        public class Node : IOctreeNode
        {
            private int _subDivIndex = 0, _subDivLevel = 0;
            private bool _visible = true;
            private BoundingBox _bounds;
            public ThreadSafeList<T> _items = new ThreadSafeList<T>();
            public Node[] _subNodes;
            public Node _parentNode;
            object _lock = new object();

            ConcurrentQueue<Tuple<bool, T>> _itemQueue = new ConcurrentQueue<Tuple<bool, T>>();
            private bool _isLoopingItems = false;

            private bool IsLoopingItems
            {
                get => _isLoopingItems;
                set
                {
                    _isLoopingItems = value;
                    while (!_isLoopingItems && !_itemQueue.IsEmpty && _itemQueue.TryDequeue(out Tuple<bool, T> result))
                    {
                        if (result.Item1)
                            _items.Add(result.Item2);
                        else
                            _items.Remove(result.Item2);
                    }
                }
            }

            public ThreadSafeList<T> Items => _items;
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
                    try
                    {
                        IsLoopingItems = true;
                        foreach (T item in _items)
                            item.IsRendering = _visible;
                        IsLoopingItems = false;
                    }
                    catch
                    {

                    }
                    finally
                    {
                        IsLoopingItems = false;
                    }
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
                //TODO: if the item is the only item within its volume, no need to subdivide more!!!
                //However, if the item is inserted into a volume with at least one other item in it, 
                //need to try subdividing for all items at that point.

                if (item == null || item.CullingVolume == null)
                    return;

                //Still within the same volume?
                if (item.CullingVolume.ContainedWithin(_bounds) == EContainment.Contains)
                {
                    //Try subdividing
                    for (int i = 0; i < 8; ++i)
                    {
                        BoundingBox bounds = GetSubdivision(i);
                        if (item.CullingVolume.ContainedWithin(bounds) == EContainment.Contains)
                        {
                            if (_isLoopingItems)
                                _itemQueue.Enqueue(new Tuple<bool, T>(false, item));
                            else
                                _items.Remove(item);

                            CreateSubNode(bounds, i);
                            _subNodes[i].Add(item);
                            break;
                        }
                    }
                }
                else if (_parentNode != null)
                {
                    //Belongs in larger parent volume, remove from this node
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
                            ClearSubNode(childDestroyIndex);
                            return true;
                        }
                    }
                }
                else
                {
                    ClearSubNode(childDestroyIndex);
                    return true;
                }

                ClearSubNode(childDestroyIndex);
                return false;
            }
            public ThreadSafeList<T> FindClosest(Vec3 point)
            {
                if (_bounds.Contains(point))
                {
                    ThreadSafeList<T> list = null;
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

                    list = new ThreadSafeList<T>(_items);
                    for (int i = 0; i < list.Count; ++i)
                        if (list[i].CullingVolume != null && !list[i].CullingVolume.Contains(point))
                            list.RemoveAt(i--);

                    return list;
                }
                else
                    return null;
            }
            public ThreadSafeList<T> FindAllJustOutside(Shape shape)
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
            public ThreadSafeList<T> CollectChildren()
            {
                ThreadSafeList<T> list = new ThreadSafeList<T>(_items);
                foreach (Node node in _subNodes)
                    if (node != null)
                        list.AddRange(node.CollectChildren());
                return list;
            }
            public ThreadSafeList<T> FindAllInside(Shape shape)
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
                    //lock (_lock)
                    //{
                    //    IsLoopingItems = true;
                    //    try
                    //    {
                    //        //Bounds is intersecting edge of frustum
                    //        //for (int i = 0; i < _items.Count; ++i)
                    //        foreach (T item in _items)
                    //        {
                    //            //if (i >= _items.Count)
                    //            //    break;

                    //            //T item = _items[i];
                    //            //if (item == null)
                    //            //    _items.RemoveAt(i--);
                    //            //else
                    //                item.IsRendering = item.CullingVolume != null ? item.CullingVolume.ContainedWithin(frustum) != EContainment.Disjoint : true;
                    //        }
                    //    }
                    //    finally
                    //    {
                    //        IsLoopingItems = false;
                    //    }
                    //}

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
                {
                    QueueRemove(value);
                    hasBeenRemoved = true;
                }
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
                            QueueAdd(item);

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
                    QueueAdd(item);

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
                //IsLoopingItems = true;
                Node[] subNodes = _subNodes;
                Node node;
                if (subNodes != null && (node = subNodes[index]) != null)
                {
                    //IsLoopingItems = false;
                    return node.Bounds;
                }

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

            #region Private Helper Methods
            private void QueueAdd(T item)
            {
                if (_isLoopingItems)
                    _itemQueue.Enqueue(new Tuple<bool, T>(true, item));
                else
                    _items.Add(item);
            }
            private void QueueRemove(T item)
            {
                if (_isLoopingItems)
                    _itemQueue.Enqueue(new Tuple<bool, T>(false, item));
                else
                    _items.Remove(item);
            }
            private void ClearSubNode(int index)
            {
                lock (_lock)
                {
                    if (index >= 0 &&
                       _subNodes != null &&
                       _subNodes[index] != null)
                    {
                        _subNodes[index] = null;
                        if (!_subNodes.Any(x => x != null))
                            _subNodes = null;
                    }
                }
            }
            private bool HasNoSubNodesExcept(int index)
            {
                if (_subNodes == null)
                    return true;
                lock (_lock)
                {
                    for (int i = 0; i < 8; ++i)
                        if (i != index && _subNodes[i] != null)
                            return false;
                }
                return true;
            }
            private void CreateSubNode(BoundingBox bounds, int index)
            {
                lock (_lock)
                {
                    Node[] subNodes = _subNodes;
                    if (subNodes == null)
                        subNodes = new Node[8];
                    else if (subNodes[index] != null)
                        return;

                    Node node = bounds;
                    node.Visible = Visible;
                    node._subDivIndex = index;
                    node._subDivLevel = _subDivLevel + 1;
                    node._parentNode = this;
                    subNodes[index] = node;
                    _subNodes = subNodes;
                }
            }
            public static implicit operator Node(BoundingBox bounds) { return new Node(bounds); }
        }
        #endregion
    }
}
