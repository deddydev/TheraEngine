using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using TheraEngine;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;

namespace System
{
    /// <summary>
    /// The interface for interacting with an internal Quadtree subdivision.
    /// </summary>
    public interface IQuadtreeNode
    {
        /// <summary>
        /// Call this when an item that this node contains has moved,
        /// otherwise the Quadtree will not be updated.
        /// </summary>
        void ItemMoved(I2DRenderable item);
        void DebugRender(bool recurse, bool onlyContainingItems, BoundingRectangle? f, float lineWidth);
        void DebugRender(Color color, float lineWidth);
    }
    /// <summary>
    /// A 3D space partitioning tree that recursively divides aabbs into 4 smaller axis-aligned rectangles depending on the items they contain.
    /// </summary>
    public class Quadtree : Quadtree<I2DRenderable>
    {
        public Quadtree(BoundingRectangle bounds) : base(bounds) { }
        public Quadtree(BoundingRectangle bounds, List<I2DRenderable> items) : base(bounds, items) { }
    }
    /// <summary>
    /// A 3D space partitioning tree that recursively divides aabbs into 4 smaller axis-aligned rectangles depending on the items they contain.
    /// </summary>
    /// <typeparam name="T">The item type to use. Must be a class deriving from I2DRenderable.</typeparam>
    public class Quadtree<T> where T : class, I2DRenderable
    {
        public const int MaxChildNodeCount = 4;

        private Node _head;
        internal HashSet<T> AllItems { get; } = new HashSet<T>();
        public int Count => AllItems.Count;

        public BoundingRectangle Bounds => _head.Bounds;

        public Quadtree(BoundingRectangle bounds) => _head = new Node(bounds, 0, 0, null, this);
        public Quadtree(BoundingRectangle bounds, List<T> items) : this(bounds) => _head.Add(items);

        public void Remake(BoundingRectangle? newBounds)
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

        //public ThreadSafeList<T> FindAll(float radius, Vec2 point, EContainment containment)
        //    => FindAll(new Sphere(radius, point), containment);
        //public ThreadSafeList<T> FindAll(Shape shape, EContainment containment)
        //{
        //    ThreadSafeList<T> list = new ThreadSafeList<T>();
        //    _head.FindAll(shape, list, containment);
        //    return list;
        //}
        public void CollectVisible(BoundingRectangle? r, RenderPasses2D passes)
        {
            if (r != null)
                _head.CollectVisible(r.Value, passes);
            else
                _head.CollectAll(passes, true);
        }
        public T FindDeepest(Vec2 point)
        {
            T value = null;
            _head.FindDeepest(point, ref value);
            return value;
        }
        /// <summary>
        /// Finds all renderables that contain the given point.
        /// </summary>
        /// <param name="point">The point that the returned renderables should contain.</param>
        /// <returns>A list of renderables containing the given point.</returns>
        public List<T> FindAllIntersecting(Vec2 point)
        {
            List<T> intersecting = new List<T>();
            _head.FindAllIntersecting(point, intersecting);
            return intersecting;
        }
        /// <summary>
        /// Finds all renderables that contain the given point.
        /// Orders renderables from least deep to deepest.
        /// </summary>
        /// <param name="point">The point that the returned renderables should contain.</param>
        /// <returns>A sorted set of renderables containing the given point.</returns>
        public SortedSet<T> FindAllIntersectingSorted(Vec2 point)
        {
            RenderPasses2D.RenderSort sorter = new RenderPasses2D.RenderSort();
            SortedSet<T> intersecting = new SortedSet<T>(sorter);
            _head.FindAllIntersecting(point, intersecting);
            return intersecting;
        }
        /// <summary>
        /// Renders the Quadtree using debug bounding boxes.
        /// </summary>
        /// <param name="f">The frustum to display intersections with. If null, does not show frustum intersections.</param>
        /// <param name="onlyContainingItems">Only renders subdivisions that contain one or more items.</param>
        /// <param name="lineWidth">The width of the bounding box lines.</param>
        public void DebugRender(BoundingRectangle? f, bool onlyContainingItems, float lineWidth = 0.1f)
            => _head.DebugRender(true, onlyContainingItems, f, lineWidth);

        private class Node : IQuadtreeNode
        {
            public Node(BoundingRectangle bounds, int subDivIndex, int subDivLevel, Node parent, Quadtree<T> owner)
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
            protected BoundingRectangle _bounds;
            protected ThreadSafeList<T> _items;
            protected Node[] _subNodes;
            protected Node _parentNode;
            private Quadtree<T> _owner;
            private ReaderWriterLockSlim _lock;

            public Quadtree<T> Owner { get => _owner; set => _owner = value; }
            public Node ParentNode { get => _parentNode; set => _parentNode = value; }
            public int SubDivIndex { get => _subDivIndex; set => _subDivIndex = value; }
            public int SubDivLevel { get => _subDivLevel; set => _subDivLevel = value; }
            public ThreadSafeList<T> Items => _items;
            public BoundingRectangle Bounds => _bounds;
            public Vec2 Center => _bounds.Translation;
            public Vec2 Min => _bounds.BottomLeft;
            public Vec2 Max => _bounds.TopRight;
            public Vec2 Extents => _bounds.Extents;
            
            public BoundingRectangle GetSubdivision(int index)
            {
                Node node = _subNodes[index];
                if (node != null)
                    return node.Bounds;

                Vec2 center = Center;
                Vec2 halfExtents = Extents / 2.0f;
                Vec2 min = Min;
                switch (index)
                {
                    case 0: return new BoundingRectangle(min.X, min.Y, halfExtents.X, halfExtents.Y);
                    case 1: return new BoundingRectangle(min.X, min.Y + halfExtents.Y, halfExtents.X, halfExtents.Y);
                    case 2: return new BoundingRectangle(min.X + halfExtents.X, min.Y + halfExtents.Y, halfExtents.X, halfExtents.Y);
                    case 3: return new BoundingRectangle(min.X + halfExtents.X, min.Y, halfExtents.X, halfExtents.Y);
                }
                return BoundingRectangle.Empty;
            }

            #region Child movement
            public void ItemMoved(I2DRenderable item) => ItemMoved(item as T);
            public void ItemMoved(T item)
            {
                //TODO: if the item is the only item within its volume, no need to subdivide more!!!
                //However, if the item is inserted into a volume with at least one other item in it, 
                //need to try subdividing for all items at that point.

                if (item == null)
                    return;

                //Still within the same volume?
                if (item.AxisAlignedRegion.ContainmentWithin(_bounds) == EContainment.Contains)
                {
                    //Try subdividing
                    for (int i = 0; i < MaxChildNodeCount; ++i)
                    {
                        BoundingRectangle bounds = GetSubdivision(i);
                        if (item.AxisAlignedRegion.ContainmentWithin(bounds) == EContainment.Contains)
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
            static readonly Rotator UIRotation = new Rotator(90.0f, 0.0f, 0.0f, RotationOrder.YPR);
            static readonly Vec3 Bias = new Vec3(0.0f, 0.0f, 0.1f);
            public void DebugRender(bool recurse, bool onlyContainingItems, BoundingRectangle? f, float lineWidth)
            {
                Color color = Color.Red;
                if (recurse)
                {
                    EContainment containment = f?.ContainmentOf(_bounds) ?? EContainment.Contains;
                    color = containment == EContainment.Intersects ? Color.Green : containment == EContainment.Contains ? Color.White : Color.Red;
                    if (containment != EContainment.Disjoint)
                        foreach (Node n in _subNodes)
                            n?.DebugRender(true, onlyContainingItems, f, lineWidth);
                }
                if (!onlyContainingItems || _items.Count != 0)
                {
                    BoundingRectangle region;
                    //bool anyVisible = false;
                    for (int i = 0; i < _items.Count; ++i)
                    {
                        region = _items[i].AxisAlignedRegion;
                        Engine.Renderer.RenderQuad(region.Center + Bias, UIRotation, region.Extents, false, Color.Orange, lineWidth);
                    }
                    //if (anyVisible)
                        DebugRender(color, lineWidth);
                }
            }
            public void DebugRender(Color color, float lineWidth)
                => Engine.Renderer.RenderQuad(_bounds.Center + Bias, UIRotation, _bounds.Extents, false, color, lineWidth);
            #endregion

            #region Visible collection
            public void CollectVisible(BoundingRectangle bounds, RenderPasses2D passes)
            {
                EContainment c = bounds.ContainmentOf(_bounds);
                if (c != EContainment.Disjoint)
                {
                    if (c == EContainment.Contains)
                        CollectAll(passes, true);
                    else
                    {
                        IsLoopingItems = true;
                        for (int i = 0; i < _items.Count; ++i)
                        {
                            I2DRenderable r = _items[i] as I2DRenderable;
                            if (r.AxisAlignedRegion.ContainmentWithin(bounds) != EContainment.Disjoint)
                                passes.Add(r);
                        }
                        IsLoopingItems = false;

                        IsLoopingSubNodes = true;
                        for (int i = 0; i < MaxChildNodeCount; ++i)
                            _subNodes[i]?.CollectVisible(bounds, passes);
                        IsLoopingSubNodes = false;
                    }
                }
            }
            public void CollectAll(RenderPasses2D passes, bool visibleOnly)
            {
                IsLoopingItems = true;
                for (int i = 0; i < _items.Count; ++i)
                    if (_items[i] is I2DRenderable r &&  (!visibleOnly || r.RenderInfo.Visible))
                        passes.Add(r);
                IsLoopingItems = false;

                IsLoopingSubNodes = true;
                for (int i = 0; i < MaxChildNodeCount; ++i)
                    _subNodes[i]?.CollectAll(passes, visibleOnly);
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
                    Add(item);
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
                
                if (item.AxisAlignedRegion.ContainmentWithin(_bounds) != EContainment.Contains)
                    return false;

                for (int i = 0; i < MaxChildNodeCount; ++i)
                {
                    if (i == ignoreSubNode)
                        continue;

                    BoundingRectangle bounds = GetSubdivision(i);
                    if (item.AxisAlignedRegion.ContainmentWithin(bounds) == EContainment.Contains)
                    {
                        CreateSubNode(bounds, i)?.Add(item);
                        return true;
                    }
                }

                return QueueAdd(item);
            }

            public void ForceAdd(T value)
            {
                if (value != null)
                    QueueAdd(value);
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
                        item.QuadtreeNode = this;
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
                        item.QuadtreeNode = null;
                    }
                    return true;
                }
            }

            #endregion

            #region Convenience methods
            public T FindNearest(Vec2 point, ref float closestDistance)
            {
                if (!_bounds.Contains(point))
                    return null;
                
                IsLoopingSubNodes = true;
                foreach (Node n in _subNodes)
                {
                    T t = n?.FindNearest(point, ref closestDistance);
                    if (t != null)
                        return t;
                }
                IsLoopingSubNodes = false;
                
                if (_items.Count == 0)
                    return null;

                T closest = null;

                IsLoopingItems = true;
                foreach (T item in _items)
                {
                    float dist = item.AxisAlignedRegion.ClosestPoint(point).DistanceToFast(point);
                    if (dist < closestDistance)
                    {
                        closestDistance = dist;
                        closest = item;
                    }
                }
                IsLoopingItems = false;

                return closest;
            }
            public void FindDeepest(Vec2 point, ref T currentDeepest)
            {
                if (!_bounds.Contains(point))
                    return;

                IsLoopingSubNodes = true;
                foreach (Node n in _subNodes)
                    n?.FindDeepest(point, ref currentDeepest);
                IsLoopingSubNodes = false;

                if (_items.Count == 0)
                    return;

                IsLoopingItems = true;
                foreach (T item in _items)
                    if (item.AxisAlignedRegion.Contains(point) && 
                        item.RenderInfo.DeeperThan(currentDeepest?.RenderInfo))
                        currentDeepest = item;
                IsLoopingItems = false;
            }
            public void FindAllIntersecting(Vec2 point, List<T> intersecting)
            {
                if (!_bounds.Contains(point))
                    return;

                IsLoopingSubNodes = true;
                foreach (Node n in _subNodes)
                    n?.FindAllIntersecting(point, intersecting);
                IsLoopingSubNodes = false;

                if (_items.Count == 0)
                    return;
                
                IsLoopingItems = true;
                foreach (T item in _items)
                    if (item.AxisAlignedRegion.Contains(point))
                        intersecting.Add(item);
                IsLoopingItems = false;
            }
            public void FindAllIntersecting(Vec2 point, SortedSet<T> intersecting)
            {
                if (!_bounds.Contains(point))
                    return;

                IsLoopingSubNodes = true;
                foreach (Node n in _subNodes)
                    n?.FindAllIntersecting(point, intersecting);
                IsLoopingSubNodes = false;

                if (_items.Count == 0)
                    return;

                IsLoopingItems = true;
                foreach (T item in _items)
                    if (item.AxisAlignedRegion.Contains(point))
                        intersecting.Add(item);
                IsLoopingItems = false;
            }
            //public void FindAll(Shape shape, ThreadSafeList<T> list, EContainment containment)
            //{
            //    EContainment c = shape.ContainedWithin(Bounds);
            //    if (c == EContainment.Intersects)
            //    {
            //        //Compare each item separately
            //        IsLoopingItems = true;
            //        foreach (T item in _items)
            //            if (item.CullingVolume != null)
            //            {
            //                c = shape.Contains(item.CullingVolume);
            //                if (c == containment)
            //                    list.Add(item);
            //            }
            //        IsLoopingItems = false;
            //    }
            //    else if (c == containment)
            //    {
            //        //All items already have this containment
            //        IsLoopingItems = true;
            //        list.AddRange(_items);
            //        IsLoopingItems = false;
            //    }
            //    else //Not what we want
            //        return;

            //    IsLoopingSubNodes = true;
            //    foreach (Node n in _subNodes)
            //        n?.FindAll(shape, list, containment);
            //    IsLoopingSubNodes = false;
            //}
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
            private Node CreateSubNode(BoundingRectangle bounds, int index)
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
            #endregion
        }
    }
}
