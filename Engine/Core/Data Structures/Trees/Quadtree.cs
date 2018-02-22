using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using TheraEngine;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;

namespace System
{
    /// <summary>
    /// The interface for interacting with an internal quadtree subdivision.
    /// </summary>
    public interface IQuadtreeNode
    {
        /// <summary>
        /// Call this when the boundable item has moved,
        /// otherwise the quadtree will not be updated.
        /// </summary>
        void ItemMoved(I2DBoundable item);
        void DebugRender(bool recurse, bool onlyContainingItems, BoundingRectangle visibleBounds, float lineWidth);
        void DebugRender(Color color, float lineWidth);
    }
    public class Quadtree : Quadtree<I2DBoundable>
    {
        public Quadtree(BoundingRectangle bounds) : base(bounds) { }
        public Quadtree(BoundingRectangle bounds, List<I2DBoundable> items) : base(bounds, items) { }
    }
    public class Quadtree<T> where T : class, I2DBoundable
    {
        public const int MaxChildNodeCount = 4;

        private Node _head;

        public BoundingRectangle Bounds => _head.Bounds;

        public int Count { get; private set; } = 0;

        public Quadtree(BoundingRectangle bounds) => _head = new Node(bounds, 0, 0, null, this);
        public Quadtree(BoundingRectangle bounds, List<T> items) => Add(items);

        public void Add(T value) => _head.Add(value, -1);
        public void Add(List<T> value) => _head.Add(value);
        public void Remove(T value) => _head.Remove(value);
        
        public void CollectVisible(BoundingRectangle bounds, RenderPasses2D passes)
            => _head?.CollectVisible(bounds, passes);

        public List<T> FindClosest(Vec2 point) => _head?.FindClosest(point);

        public void DebugRender(bool onlyContainingItems, BoundingRectangle bounds, float lineWidth)
            => _head?.DebugRender(true, onlyContainingItems, bounds, lineWidth);
        public void DebugRender(bool onlyContainingItems, Color color, float lineWidth)
             => _head?.DebugRender(true, onlyContainingItems, color, lineWidth);

        public void Resize(Vec2 bounds) => _head.Resize(bounds);

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
                _parent = parent;
                _owner = owner;
            }
            
            protected int _subDivIndex, _subDivLevel;
            protected BoundingRectangle _bounds;
            protected ThreadSafeList<T> _items;
            protected Node[] _subNodes;
            protected Node _parent;
            private Quadtree<T> _owner;
            private ReaderWriterLockSlim _lock;

            public Quadtree<T> Owner { get => _owner; set => _owner = value; }
            public Node ParentNode { get => _parent; set => _parent = value; }
            public int SubDivIndex { get => _subDivIndex; set => _subDivIndex = value; }
            public int SubDivLevel { get => _subDivLevel; set => _subDivLevel = value; }
            public ThreadSafeList<T> Items => _items;
            public BoundingRectangle Bounds => _bounds;
            public Vec2 Center => (Min + Max) / 2.0f;
            public Vec2 Min => _bounds.BottomLeft;
            public Vec2 Max => _bounds.TopRight;
            public Vec2 Extents => _bounds.Bounds;

            public BoundingRectangle GetSubdivision(int index)
            {
                if (_subNodes != null && _subNodes[index] != null)
                    return _subNodes[index].Bounds;

                Vec2 center = Center;
                Vec2 halfExtents = Extents / 2.0f;
                Vec2 min = Min;
                switch (index)
                {
                    case 0: return new BoundingRectangle(min.X,                 min.Y,                  halfExtents.X, halfExtents.Y);
                    case 1: return new BoundingRectangle(min.X,                 min.Y + halfExtents.Y,  halfExtents.X, halfExtents.Y);
                    case 2: return new BoundingRectangle(min.X + halfExtents.X, min.Y + halfExtents.Y,  halfExtents.X, halfExtents.Y);
                    case 3: return new BoundingRectangle(min.X + halfExtents.X, min.Y,                  halfExtents.X, halfExtents.Y);
                }
                return BoundingRectangle.Empty;
            }

            #region Child movement
            public void ItemMoved(I2DBoundable item) => ItemMoved(item as T);
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
            #endregion

            #region Debug
            public void DebugRender(bool recurse, bool onlyContainingItems, BoundingRectangle bounds, float lineWidth)
            {
                Color color = Color.Red;
                if (recurse)
                {
                    EContainment containment = bounds.ContainmentOf(_bounds);
                    color = containment == EContainment.Intersects ? Color.Green : containment == EContainment.Contains ? Color.White : Color.Red;
                    if (containment != EContainment.Disjoint)
                        foreach (Node n in _subNodes)
                            n?.DebugRender(true, onlyContainingItems, bounds, lineWidth);
                }
                if (!onlyContainingItems || _items.Count != 0)
                    DebugRender(color, lineWidth);
            }
            public void DebugRender(bool recurse, bool onlyContainingItems, Color color, float lineWidth)
            {
                if (recurse)
                    foreach (Node n in _subNodes)
                        n?.DebugRender(true, onlyContainingItems, color, lineWidth);
                if (!onlyContainingItems || _items.Count != 0)
                    DebugRender(color, lineWidth);
            }
            public void DebugRender(Color color, float lineWidth)
            {
                Vec2 halfBounds = (_bounds.Bounds / 2.0f);
                Engine.Renderer.RenderQuad(_bounds.Position - _bounds.LocalOrigin + halfBounds, Vec3.Forward, halfBounds, false, color, lineWidth);
            }
            #endregion

            #region Visible collection
            public void CollectVisible(BoundingRectangle bounds, RenderPasses2D passes)
            {
                EContainment c = bounds.ContainmentOf(_bounds);
                if (c != EContainment.Disjoint)
                {
                    if (c == EContainment.Contains)
                        CollectAll(passes);
                    else
                    {
                        IsLoopingItems = true;
                        for (int i = 0; i < _items.Count; ++i)
                        {
                            I2DRenderable r = _items[i] as I2DRenderable;
                            if ((c = r.AxisAlignedRegion.ContainmentWithin(bounds)) != EContainment.Disjoint)
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
            private void CollectAll(RenderPasses2D passes)
            {
                IsLoopingItems = true;
                for (int i = 0; i < _items.Count; ++i)
                    passes.Add(_items[i] as I2DRenderable);
                IsLoopingItems = false;

                IsLoopingSubNodes = true;
                for (int i = 0; i < MaxChildNodeCount; ++i)
                    _subNodes[i]?.CollectAll(passes);
                IsLoopingSubNodes = false;
            }
            #endregion

            #region Add/Remove
            internal bool Remove(T item)
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
               
                if (force || item.AxisAlignedRegion.ContainmentWithin(_bounds) != EContainment.Contains)
                {
                    if (force)
                    {
                        if (QueueAdd(item))
                        {
                            item.QuadtreeNode = this;
                            return true;
                        }
                    }
                    return false;
                }
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

                if (QueueAdd(item))
                {
                    item.QuadtreeNode = this;
                    return true;
                }

                return false;
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

            #endregion

            #region Convenience methods
            public List<T> FindClosest(Vec2 point)
            {
                if (_bounds.Contains(point))
                {
                    List<T> list = null;
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
                        if (!list[i].Contains(point))
                            list.RemoveAt(i--);

                    return list;
                }
                else
                    return null;
            }
            public List<T> CollectChildren()
            {
                List<T> list = _items;
                foreach (Node node in _subNodes)
                    if (node != null)
                        list.AddRange(node.CollectChildren());
                return list;
            }
            public List<T> FindAll(BoundingRectangle bounds, EContainmentFlags containment)
            {
                List<T> list = new List<T>();

                //Not searching for anything?
                if (containment == EContainmentFlags.None)
                    return list;

                //if (_region.Contains(bounds) == EContainment.Contains)
                //{

                //}

                return null;
            }
            private void FindAllInternal(BoundingRectangle bounds, EContainmentFlags containment, List<T> found)
            {

            }
            public void Resize(Vec2 bounds)
            {
                _bounds.Bounds *= bounds;
                _bounds.Translation *= bounds;

                IsLoopingSubNodes = true;
                foreach (Node node in _subNodes)
                    if (node != null)
                        node.Resize(bounds);
                IsLoopingSubNodes = false;

                IsLoopingItems = true;
                foreach (T item in _items)
                    ItemMoved(item);
                IsLoopingItems = false;
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
