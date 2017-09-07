using TheraEngine;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using TheraEngine.Rendering;
using System.Threading.Tasks;
using System.Threading;
using TheraEngine.Core.Shapes;

namespace System
{
    /// <summary>
    /// The interface for interacting with an internal octree subdivision.
    /// </summary>
    public interface IOctreeNode
    {
        /// <summary>
        /// Call this when an item that this node contains has moved,
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
        Node _head;

        public Octree(BoundingBox bounds)
        {
            _head = new Node(bounds)
            {
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
        public ThreadSafeList<T> FindAll(float radius, Vec3 point, EContainment containment)
            => FindAll(new Sphere(radius, point), containment);
        
        //public ThreadSafeList<T> FindClosest(Vec3 point) { return _head?.FindClosest(point); }
        public ThreadSafeList<T> FindAll(Shape shape, EContainment containment)
        {
            ThreadSafeList<T> list = new ThreadSafeList<T>();
            _head.FindAll(shape, list, containment);
            return list;
        }

        public void CollectVisible(Frustum frustum, RenderPasses passes, bool shadowPass)
            => _head.CollectVisible(frustum, passes, shadowPass);
        
        public void DebugRender(Frustum f)
            => _head.DebugRender(true, f);

        public void Add(T value)
            => _head.Add(value, -1, true);
        
        public void Add(List<T> value)
            => _head.Add(value, true);
        
        public void Remove(T value)
            => _head.Remove(value);
        
        private class Node : IOctreeNode
        {
            public Node(BoundingBox bounds)
            {
                _bounds = bounds;
                _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
                _items = new ThreadSafeList<T>(_lock);
                _subNodes = new Node[8];
                _subDivIndex = 0;
                _subDivLevel = 0;
            }

            protected int _subDivIndex, _subDivLevel;
            protected BoundingBox _bounds;
            protected ThreadSafeList<T> _items;
            protected Node[] _subNodes;
            protected Node parentNode;
            private Octree<T> _owner;
            private ReaderWriterLockSlim _lock;

            public Octree<T> Owner { get => _owner; set => _owner = value; }
            public Node ParentNode { get => parentNode; set => parentNode = value; }
            public int SubDivIndex { get => _subDivIndex; set => _subDivIndex = value; }
            public int SubDivLevel { get => _subDivLevel; set => _subDivLevel = value; }
            public ThreadSafeList<T> Items => _items;
            public BoundingBox Bounds => _bounds;
            public Vec3 Center => _bounds.Translation;
            public Vec3 Min => _bounds.Minimum;
            public Vec3 Max => _bounds.Maximum;
            
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
            public void DebugRender(bool recurse, Frustum f)
            {
                Color clr = Color.Red;
                if (recurse)
                {
                    EContainment c = f.Contains(_bounds);
                    clr = c == EContainment.Intersects ? Color.Green : c == EContainment.Contains ? Color.White : Color.Red;
                    if (c == EContainment.Contains)
                        foreach (Node n in _subNodes)
                            n?.DebugRender(true, f);
                }
                Engine.Renderer.RenderAABB(_bounds.HalfExtents, _bounds.Translation, false, clr, 5.0f);
            }
            public void CollectVisible(Frustum frustum, RenderPasses passes, bool shadowPass)
            {
                EContainment c = frustum.Contains(_bounds);
                if (c != EContainment.Disjoint)
                {
                    if (c == EContainment.Contains)
                        CollectAll(passes, shadowPass);
                    else
                    {
                        IsLoopingItems = true;
                        for (int i = 0; i < _items.Count; ++i)
                        {
                            I3DRenderable r = _items[i] as I3DRenderable;
                            bool allowRender = (shadowPass && r.RenderInfo.CastsShadows) || !shadowPass;
                            if (allowRender && (r.CullingVolume == null || (c = r.CullingVolume.ContainedWithin(frustum)) != EContainment.Disjoint))
                                passes.Add(r);
                        }
                        IsLoopingItems = false;

                        IsLoopingSubNodes = true;
                        for (int i = 0; i < 8; ++i)
                            _subNodes[i]?.CollectVisible(frustum, passes, shadowPass);
                        IsLoopingSubNodes = false;
                    }
                }
            }
            private void CollectAll(RenderPasses passes, bool shadowPass)
            {
                IsLoopingItems = true;
                for (int i = 0; i < _items.Count; ++i)
                {
                    I3DRenderable r = _items[i] as I3DRenderable;
                    bool allowRender = (shadowPass && r.RenderInfo.CastsShadows) || !shadowPass;
                    if (allowRender)
                        passes.Add(r);
                }
                IsLoopingItems = false;

                IsLoopingSubNodes = true;
                for (int i = 0; i < 8; ++i)
                    _subNodes[i]?.CollectAll(passes, shadowPass);
                IsLoopingSubNodes = false;
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
            public T FindClosest(Vec3 point, ref float closestDistance)
            {
                if (!_bounds.Contains(point))
                    return null;
                
                IsLoopingSubNodes = true;
                foreach (Node n in _subNodes)
                {
                    T t = n?.FindClosest(point, ref closestDistance);
                    if (t != null)
                        return t;
                }
                IsLoopingSubNodes = false;
                
                if (_items.Count == 0)
                    return null;

                T closest = null;

                IsLoopingItems = true;
                foreach (T item in _items)
                    if (item.CullingVolume != null)
                    {
                        float dist = item.CullingVolume.ClosestPoint(point).DistanceToFast(point);
                        if (dist < closestDistance)
                        {
                            closestDistance = dist;
                            closest = item;
                        }
                    }
                IsLoopingItems = false;

                return closest;
            }
            public void FindAll(Shape shape, ThreadSafeList<T> list, EContainment containment)
            {
                EContainment c = shape.ContainedWithin(Bounds);
                if (c == EContainment.Intersects)
                {
                    //Compare each item separately
                    IsLoopingItems = true;
                    foreach (T item in _items)
                        if (item.CullingVolume != null)
                        {
                            c = shape.Contains(item.CullingVolume);
                            if (c == containment)
                                list.Add(item);
                        }
                    IsLoopingItems = false;
                }
                else if (c == containment)
                {
                    //All items already have this containment
                    IsLoopingItems = true;
                    list.AddRange(_items);
                    IsLoopingItems = false;
                }
                else //Not what we want
                    return;

                IsLoopingSubNodes = true;
                foreach (Node n in _subNodes)
                    n?.FindAll(shape, list, containment);
                IsLoopingSubNodes = false;
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
