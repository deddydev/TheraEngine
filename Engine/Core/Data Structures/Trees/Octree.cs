using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using TheraEngine;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;

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
        void DebugRender(bool recurse, bool onlyContainingItems, Frustum f, float lineWidth);
        void DebugRender(Color color, float lineWidth);
    }
    /// <summary>
    /// A 3D space partitioning tree that recursively divides aabbs into 8 smaller aabbs depending on the items they contain.
    /// </summary>
    public class Octree : Octree<I3DBoundable>
    {
        public Octree(BoundingBox bounds) : base(bounds) { }
        public Octree(BoundingBox bounds, List<I3DBoundable> items) : base(bounds, items) { }
    }
    /// <summary>
    /// A 3D space partitioning tree that recursively divides aabbs into 8 smaller aabbs depending on the items they contain.
    /// </summary>
    /// <typeparam name="T">The item type to use. Must be a class deriving from I3DBoundable.</typeparam>
    public class Octree<T> where T : class, I3DBoundable
    {
        public const int MaxChildNodeCount = 8;

        private Node _head;
        internal HashSet<T> AllItems { get; } = new HashSet<T>();
        public int Count => AllItems.Count;

        public Octree(BoundingBox bounds) => _head = new Node(bounds, 0, 0, null, this);
        public Octree(BoundingBox bounds, List<T> items) : this(bounds) => _head.Add(items);

        public void Remake(BoundingBox newBounds = null)
        {
            _head = new Node(newBounds ?? _head.Bounds, 0, 0, null, this);
            var array = AllItems.ToArray();
            AllItems.Clear();
            foreach (T item in array)
                if (!_head.Add(item))
                    _head.ForceAdd(item);
        }
        /// <summary>
        /// Returns true if the item was added, and false if it was already in the tree.
        /// </summary>
        public bool Add(T value)
        {
            if (!AllItems.Contains(value))
            {
                if (!_head.Add(value))
                    _head.ForceAdd(value);
                return true;
            }
            return false;
        }
        public void Add(IEnumerable<T> value)
        {
            foreach (T item in value)
                Add(item);
        }
        /// <summary>
        /// Returns true if the item was found and removed, and false if it wasn't found.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Remove(T value)
        {
            if (AllItems.Contains(value))
            {
                _head.Remove(value);
                return true;
            }
            return false;
        }

        public ThreadSafeList<T> FindAll(float radius, Vec3 point, EContainment containment)
            => FindAll(new Sphere(radius, point), containment);
        public ThreadSafeList<T> FindAll(Shape shape, EContainment containment)
        {
            ThreadSafeList<T> list = new ThreadSafeList<T>();
            _head.FindAll(shape, list, containment);
            return list;
        }
        public void CollectVisible(Sphere sphere, RenderPasses3D passes, bool shadowPass)
        {
            passes.SetShadowPass(shadowPass);
            if (sphere != null)
                _head.CollectVisible(sphere, passes, shadowPass);
            else
                _head.CollectAll(passes, shadowPass);
        }
        public void CollectVisible(Frustum frustum, RenderPasses3D passes, bool shadowPass)
        {
            passes.SetShadowPass(shadowPass);
            if (frustum != null)
                _head.CollectVisible(frustum, passes, shadowPass);
            else
                _head.CollectAll(passes, shadowPass);
        }

        /// <summary>
        /// Renders the octree using debug bounding boxes.
        /// </summary>
        /// <param name="f">The frustum to display intersections with. If null, does not show frustum intersections.</param>
        /// <param name="onlyContainingItems">Only renders subdivisions that contain one or more items.</param>
        /// <param name="lineWidth">The width of the bounding box lines.</param>
        public void DebugRender(Frustum f, bool onlyContainingItems, float lineWidth = 2.0f)
            => _head.DebugRender(true, onlyContainingItems, f, lineWidth);

        private class Node : IOctreeNode
        {
            public Node(BoundingBox bounds, int subDivIndex, int subDivLevel, Node parent, Octree<T> owner)
            {
                _bounds = bounds;
                _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
                _items = new ThreadSafeList<T>(_lock);
                _subNodes = new Node[MaxChildNodeCount];
                _subDivIndex = subDivIndex;
                _subDivLevel = subDivLevel;
                _parentNode = parent;
                _owner = owner;
            }

            protected int _subDivIndex, _subDivLevel;
            protected BoundingBox _bounds;
            protected ThreadSafeList<T> _items;
            protected Node[] _subNodes;
            protected Node _parentNode;
            private Octree<T> _owner;
            private ReaderWriterLockSlim _lock;

            public Octree<T> Owner { get => _owner; set => _owner = value; }
            public Node ParentNode { get => _parentNode; set => _parentNode = value; }
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

            #region Child movement
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
                    for (int i = 0; i < MaxChildNodeCount; ++i)
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
                        Owner._head.ForceAdd(item);
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
            #endregion

            #region Debug
            public void DebugRender(bool recurse, bool onlyContainingItems, Frustum f, float lineWidth)
            {
                Color color = Color.Red;
                if (recurse)
                {
                    EContainment containment = f?.Contains(_bounds) ?? EContainment.Contains;
                    color = containment == EContainment.Intersects ? Color.Green : containment == EContainment.Contains ? Color.White : Color.Red;
                    if (containment != EContainment.Disjoint)
                        foreach (Node n in _subNodes)
                            n?.DebugRender(true, onlyContainingItems, f, lineWidth);
                }
                if (!onlyContainingItems || _items.Count != 0)
                    DebugRender(color, lineWidth);
            }
            public void DebugRender(Color color, float lineWidth)
                => Engine.Renderer.RenderAABB(_bounds.HalfExtents, _bounds.Translation, false, color, 1.0f);
            #endregion

            #region Visible collection
            public void CollectVisible(Sphere sphere, RenderPasses3D passes, bool shadowPass)
            {
                EContainment c = sphere.Contains(_bounds);
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
                            bool allowRender = r != null && (shadowPass && r.RenderInfo.CastsShadows) || !shadowPass;
                            if (allowRender && (r.CullingVolume == null || r.CullingVolume.ContainedWithin(sphere) != EContainment.Disjoint))
                                passes.Add(r);
                        }
                        IsLoopingItems = false;

                        IsLoopingSubNodes = true;
                        for (int i = 0; i < MaxChildNodeCount; ++i)
                            _subNodes[i]?.CollectVisible(sphere, passes, shadowPass);
                        IsLoopingSubNodes = false;
                    }
                }
            }
            public void CollectVisible(Frustum frustum, RenderPasses3D passes, bool shadowPass)
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
                        for (int i = 0; i < MaxChildNodeCount; ++i)
                            _subNodes[i]?.CollectVisible(frustum, passes, shadowPass);
                        IsLoopingSubNodes = false;
                    }
                }
            }
            public void CollectAll(RenderPasses3D passes, bool shadowPass)
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
                for (int i = 0; i < MaxChildNodeCount; ++i)
                    _subNodes[i]?.CollectAll(passes, shadowPass);
                IsLoopingSubNodes = false;
            }
            #endregion

            #region Add/Remove
            /// <summary>
            /// Returns true if this node no longer contains anything.
            /// </summary>
            /// <param name="item">The item to remove.</param>
            public bool Remove(T item)
            {
                if (_items.Contains(item))
                    QueueRemove(item);
                else
                    for (int i = 0; i < MaxChildNodeCount; ++i)
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
            /// <param name="forceAddToThisNode">If true, will add each item regardless of if its culling volume fits within this node's bounds.</param>
            /// <returns>True if ANY node was added.</returns>
            internal void Add(List<T> items)
            {
                foreach (T item in items)
                    Add(item, -1);
            }
            /// <summary>
            /// Adds an item to this node. May subdivide.
            /// </summary>
            /// <param name="items">The item to add.</param>
            /// <param name="forceAddToThisNode">If true, will add the item regardless of if its culling volume fits within the node's bounds.</param>
            /// <returns>True if the node was added.</returns>
            internal bool Add(T item, int ignoreSubNode = -1)
            {
                if (item == null)
                    return false;

                if (item.CullingVolume != null)
                {
                    if (item.CullingVolume.ContainedWithin(_bounds) != EContainment.Contains)
                        return false;

                    for (int i = 0; i < MaxChildNodeCount; ++i)
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
                
                return QueueAdd(item);
            }
            #endregion

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
                            Add(result.Item2, -1);
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

            private bool QueueAdd(T item)
            {
                if (IsLoopingItems)
                {
                    _itemQueue.Enqueue(new Tuple<bool, T>(true, item));
                    return false;
                }
                else
                {
                    if (Owner.AllItems.Add(item))
                    {
                        _items.Add(item);
                        item.OctreeNode = this;
                    }
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
                    if (Owner.AllItems.Remove(item))
                    {
                        _items.Remove(item);
                        item.OctreeNode = null;
                    }
                    return true;
                }
            }

            #endregion

            #region Convenience methods
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
            private void ClearSubNode(int index)
            {
                if (index >= 0)
                    _subNodes[index] = null;
            }
            private bool HasNoSubNodesExcept(int index)
            {
                for (int i = 0; i < MaxChildNodeCount; ++i)
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

                    return _subNodes[index] = new Node(bounds, index, _subDivLevel + 1, this, Owner);
                }
                finally
                {
                    IsLoopingSubNodes = false;
                }
            }

            internal void ForceAdd(T value)
            {
                QueueAdd(value);
            }
            #endregion
        }
    }
}
