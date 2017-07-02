using TheraEngine;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Worlds.Actors;
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
    /// <summary>
    /// The interface for interacting with an internal octree subdivision.
    /// </summary>
    public interface IOctreeNode
    {
        /// <summary>
        /// Call this when the boundable item has moved,
        /// otherwise the octree will not be updated.
        /// </summary>
        void ItemMoved(I3DBoundable item);
    }
    /// <summary>
    /// 
    /// </summary>
    public class Octree : Octree<I3DBoundable>
    {
        public Octree(BoundingBox bounds) : base(bounds) { }
        public Octree(BoundingBox bounds, List<I3DBoundable> items) : base(bounds, items) { }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">The item type to use.</typeparam>
    public class Octree<T> where T : class, I3DBoundable
    {
        /// <summary>
        /// Event called when the "IsRendering" property of an item is changed.
        /// </summary>
        public event Action<T> ItemRenderChanged;

        Node _head;

        public Octree(BoundingBox bounds)
        {
            _head = new Node(bounds)
            {
                Visible = true,
                SubDivIndex = 0,
                SubDivLevel = 0,
                ParentNode = null,
                Owner = this
            };
        }
        public Octree(BoundingBox bounds, List<T> items)
        {
            _head = new Node(bounds)
            {
                Visible = true,
                SubDivIndex = 0,
                SubDivLevel = 0,
                ParentNode = null,
                Owner = this
            };
            _head.Add(items, true);
        }

        /// <summary>
        /// Finds all items contained within the given radius of a given point.
        /// </summary>
        /// <param name="radius">The distance from the point that returned items must be contained within.</param>
        /// <param name="point">The center point of the sphere.</param>
        /// <param name="list">The list of items that are contained within the shape.</param>
        /// <param name="allowPartialContains">If true, adds items if they're even partially contained within the shape.</param>
        /// <param name="testVisibleOnly">If true, only tests for containment of visible items.</param>
        public void FindAllWithinRadius(float radius, Vec3 point, ThreadSafeList<T> list, bool allowPartialContains, bool testVisibleOnly)
        {
            _head.FindAllInside(new Sphere(radius, point), list, allowPartialContains, testVisibleOnly);
        }
        //public ThreadSafeList<T> FindClosest(Vec3 point) { return _head?.FindClosest(point); }
        public ThreadSafeList<T> FindAllInside(Shape shape, bool allowPartialContains, bool testVisibleOnly)
        {
            ThreadSafeList<T> list = new ThreadSafeList<T>();
            _head.FindAllInside(shape, list, allowPartialContains, testVisibleOnly);
            return list;
        }

        public void Cull(Frustum frustum, bool resetVisibility = true, bool cullOffscreen = true, bool debugRender = false)
            => _head.Cull(frustum, resetVisibility, cullOffscreen, debugRender);
        
        public void DebugRender(Color color)
            => _head.DebugRender(true, color);

        public void Add(T value)
            => _head.Add(value, -1, true);
        
        public void Add(List<T> value)
            => _head.Add(value, true);
        
        public void Remove(T value)
            => _head.Remove(value);
        
        public class Node : IOctreeNode
        {
            public Node(BoundingBox bounds) { _bounds = bounds; }

            protected int _subDivIndex = 0, _subDivLevel = 0;
            protected bool _visible = false;
            protected BoundingBox _bounds;
            protected ThreadSafeList<T> _items = new ThreadSafeList<T>();
            protected Node[] _subNodes = new Node[8];
            protected Node parentNode;
            protected object _lock = new object();
            private Octree<T> _owner;

            public Octree<T> Owner { get => _owner; set => _owner = value; }
            public Node ParentNode { get => parentNode; set => parentNode = value; }
            public int SubDivIndex { get => _subDivIndex; set => _subDivIndex = value; }
            public int SubDivLevel { get => _subDivLevel; set => _subDivLevel = value; }
            public ThreadSafeList<T> Items => _items;
            public BoundingBox Bounds => _bounds;
            public Vec3 Center => _bounds.Translation;
            public Vec3 Min => _bounds.Minimum;
            public Vec3 Max => _bounds.Maximum;
            public bool Visible
            {
                get => _visible;
                set
                {
                    //if (_visible == value)
                    //    return;

                    _visible = value;

                    IsLoopingItems = true;
                    foreach (T item in _items)
                    {
                        item.IsRendering = _visible;
                        Owner.ItemRenderChanged?.Invoke(item);
                    }
                    IsLoopingItems = false;

                    IsLoopingSubNodes = true;
                    foreach (Node node in _subNodes)
                        if (node != null)
                            node.Visible = _visible;
                    IsLoopingSubNodes = false;
                }
            }
            public BoundingBox GetSubdivision(int index)
            {
                Node node = _subNodes[index];
                if (node != null)
                    return node.Bounds;
                
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

            public void ItemMoved(I3DBoundable item) => ItemMoved(item as T);
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
                            QueueRemove(item);
                            CreateSubNode(bounds, i)?.Add(item);
                            break;
                        }
                    }
                }
                else if (ParentNode != null)
                {
                    //Belongs in larger parent volume, remove from this node
                    bool shouldDestroy = Remove(item);
                    if (!ParentNode.TryAddUp(item, shouldDestroy ? _subDivIndex : -1))
                    {
                        //Force add to root node
                        Owner.Add(item);
                    }
                }
            }
            /// <summary>
            /// Moves up the heirarchy instead of down to add an item.
            /// Returns true if the item was added to this node.
            /// </summary>
            private bool TryAddUp(T item, int childDestroyIndex)
            {
                ClearSubNode(childDestroyIndex);

                if (Add(item, childDestroyIndex))
                    return true;
                else
                {
                    bool shouldDestroy = _items.Count == 0 && HasNoSubNodesExcept(-1);
                    if (ParentNode != null)
                        return ParentNode.TryAddUp(item, shouldDestroy ? _subDivIndex : -1);
                }
                
                return false;
            }
            public void DebugRender(bool recurse, Color color)
            {
                Engine.Renderer.RenderAABB(_bounds.HalfExtents, _bounds.Translation, false, color, 5.0f);
                if (recurse)
                    foreach (Node n in _subNodes)
                        n?.DebugRender(true, color);
            }
            public void Cull(Frustum frustum, bool resetVisibility, bool cullOffscreen, bool debugRender)
            {
                EContainment c = frustum.Contains(_bounds);
                if (c != EContainment.Intersects)
                    Visible = c == EContainment.Contains;
                else
                {
                    //Bounds is intersecting edge of frustum
                    _visible = true;

                    IsLoopingItems = true;
                    foreach (T item in _items)
                    {
                        item.IsRendering = item.CullingVolume != null ? item.CullingVolume.ContainedWithin(frustum) != EContainment.Disjoint : true;
                        Owner.ItemRenderChanged?.Invoke(item);
                    }
                    IsLoopingItems = false;

                    IsLoopingSubNodes = true;
                    foreach (Node n in _subNodes)
                        n?.Cull(frustum, resetVisibility, cullOffscreen, debugRender);
                    IsLoopingSubNodes = false;
                }
                if (debugRender)
                {
                    Color clr = c == EContainment.Intersects ? Color.Green : c == EContainment.Contains ? Color.White : Color.Red;
                    DebugRender(c == EContainment.Contains, clr);
                }
            }
            internal bool Remove(T item)
            {
                if (_items.Contains(item))
                    QueueRemove(item);
                else
                    for (int i = 0; i < 8; ++i)
                    {
                        Node node = _subNodes[i];
                        if (node != null)
                        {
                            if (node.Remove(item))
                                _subNodes[i] = null;
                            else
                                return false;
                        }
                    }
                
                return _items.Count == 0 && HasNoSubNodesExcept(-1);
            }
            /// <summary>
            /// Adds a list of items to this node. May subdivide.
            /// </summary>
            /// <param name="items">The items to add.</param>
            /// <param name="force">If true, will add each item regardless of if its culling volume fits within the node's bounds.</param>
            /// <returns>True if ANY node was added.</returns>
            internal bool Add(List<T> items, bool force = false)
            {
                bool addedAny = false;
                foreach (T item in items)
                    addedAny = addedAny || Add(item, -1, force);
                return addedAny;
            }
            /// <summary>
            /// Adds an item to this node. May subdivide.
            /// </summary>
            /// <param name="items">The item to add.</param>
            /// <param name="force">If true, will add the item regardless of if its culling volume fits within the node's bounds.</param>
            /// <returns>True if the node was added.</returns>
            internal bool Add(T item, int ignoreSubNode = -1, bool force = false)
            {
                if (item == null)
                    return false;
                
                if (item.CullingVolume != null)
                {
                    if (item.CullingVolume.ContainedWithin(_bounds) != EContainment.Contains)
                    {
                        if (force)
                        {
                            if (QueueAdd(item))
                            {
                                item.IsRendering = _visible;
                                item.OctreeNode = this;
                                return true;
                            }
                        }
                        return false;
                    }
                    for (int i = 0; i < 8; ++i)
                    {
                        if (i == ignoreSubNode)
                            continue;

                        BoundingBox bounds = GetSubdivision(i);
                        if (item.CullingVolume.ContainedWithin(bounds) == EContainment.Contains)
                        {
                            CreateSubNode(bounds, i)?.Add(item);
                            return true;
                        }
                    }
                }

                if (QueueAdd(item))
                {
                    item.IsRendering = _visible;
                    item.OctreeNode = this;
                    return true;
                }

                return false;
            }

            #region Loop Threading Backlog

            //Backlog for adding and removing items when other threads are currently looping
            protected ConcurrentQueue<Tuple<bool, T>> _itemQueue = new ConcurrentQueue<Tuple<bool, T>>();
            //Backlog for setting sub nodes when other threads are currently looping
            protected ConcurrentQueue<Tuple<int, Node>> _subNodeQueue = new ConcurrentQueue<Tuple<int, Node>>();
            private bool _isLoopingItems = false;
            private bool _isLoopingSubNodes = false;

            protected bool IsLoopingItems
            {
                get => _isLoopingItems;
                set
                {
                    _isLoopingItems = value;
                    while (!_isLoopingItems && !_itemQueue.IsEmpty && _itemQueue.TryDequeue(out Tuple<bool, T> result))
                    {
                        if (result.Item1)
                            Add(result.Item2, -1, true);
                        else
                            Remove(result.Item2);
                    }
                }
            }
            protected bool IsLoopingSubNodes
            {
                get => _isLoopingSubNodes;
                set
                {
                    _isLoopingSubNodes = value;
                    while (!_isLoopingSubNodes && !_subNodeQueue.IsEmpty && _subNodeQueue.TryDequeue(out Tuple<int, Node> result))
                        _subNodes[result.Item1] = result.Item2;
                }
            }

            #endregion

            #region Misc Data Methods
            //public ThreadSafeList<T> FindClosest(Vec3 point)
            //{
            //    if (_bounds.Contains(point))
            //    {
            //        ThreadSafeList<T> list = null;
            //        foreach (Node node in _subNodes)
            //        {
            //            list = node?.FindClosest(point);
            //            if (list != null)
            //                return list;
            //        }
                    
            //        if (_items.Count == 0)
            //            return null;

            //        list = new ThreadSafeList<T>(_items);
            //        for (int i = 0; i < list.Count; ++i)
            //            if (list[i].CullingVolume != null && !list[i].CullingVolume.Contains(point))
            //                list.RemoveAt(i--);

            //        return list;
            //    }
            //    else
            //        return null;
            //}

            /// <summary>
            /// Finds all items contained within the given shape.
            /// </summary>
            /// <param name="shape">The shape to use for containment.</param>
            /// <param name="list">The list of items that are contained within the shape.</param>
            /// <param name="allowPartialContains">If true, adds items if they're even partially contained within the shape.</param>
            /// <param name="testVisibleOnly">If true, only tests for containment of visible items.</param>
            public void FindAllInside(Shape shape, ThreadSafeList<T> list, bool allowPartialContains, bool testVisibleOnly)
            {
                if ((testVisibleOnly ? Visible : true) && shape.ContainedWithin(Bounds) != EContainment.Disjoint)
                {
                    foreach (T item in _items)
                        if (item.CullingVolume != null && (testVisibleOnly ? item.IsRendering : true))
                        {
                            EContainment c = shape.Contains(item.CullingVolume);
                            if (c == EContainment.Contains || (allowPartialContains && c == EContainment.Intersects))
                                list.Add(item);
                        }
                }
            }
            /// <summary>
            /// Finds all items contained within the given radius of a given point.
            /// </summary>
            /// <param name="radius">The distance from the point that returned items must be contained within.</param>
            /// <param name="point">The center point of the sphere.</param>
            /// <param name="list">The list of items that are contained within the shape.</param>
            /// <param name="allowPartialContains">If true, adds items if they're even partially contained within the shape.</param>
            /// <param name="testVisibleOnly">If true, only tests for containment of visible items.</param>
            public void FindAllWithinRadius(float radius, Vec3 point, ThreadSafeList<T> list, bool allowPartialContains, bool testVisibleOnly)
            {
                FindAllInside(new Sphere(radius, point), list, allowPartialContains, testVisibleOnly);
            }
            /// <summary>
            /// Simply collects all the items contained in this node and all of its sub nodes.
            /// </summary>
            /// <returns></returns>
            public ThreadSafeList<T> CollectChildren()
            {
                ThreadSafeList<T> list = new ThreadSafeList<T>(_items);
                foreach (Node node in _subNodes)
                    if (node != null)
                        list.AddRange(node.CollectChildren());
                return list;
            }
            #endregion

            #region Private Helper Methods
            private bool QueueAdd(T item)
            {
                if (IsLoopingItems)
                {
                    _itemQueue.Enqueue(new Tuple<bool, T>(true, item));
                    return false;
                }
                else
                {
                    _items.Add(item);
                    return true;
                }
            }
            private bool QueueRemove(T item)
            {
                if (IsLoopingItems)
                {
                    _itemQueue.Enqueue(new Tuple<bool, T>(false, item));
                    return false;
                }
                else
                {
                    _items.Remove(item);
                    return true;
                }
            }
            private void ClearSubNode(int index)
            {
                if (index >= 0)
                    _subNodes[index] = null;
            }
            private bool HasNoSubNodesExcept(int index)
            {
                for (int i = 0; i < 8; ++i)
                    if (i != index && _subNodes[i] != null)
                        return false;
                return true;
            }
            private Node CreateSubNode(BoundingBox bounds, int index)
            {
                try
                {
                    IsLoopingSubNodes = true;
                    if (_subNodes[index] != null)
                        return _subNodes[index];

                    return _subNodes[index] = new Node(bounds)
                    {
                        Visible = Visible,
                        SubDivIndex = index,
                        SubDivLevel = _subDivLevel + 1,
                        ParentNode = this,
                        Owner = Owner
                    };
                }
                finally
                {
                    IsLoopingSubNodes = false;
                }
            }
        }
        #endregion
    }
}
