using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using TheraEngine;
using TheraEngine.Components.Scene;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Shapes;

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
        void ItemMoved(I3DRenderable item);
        void DebugRender(bool recurse, bool onlyContainingItems, Frustum f, float lineWidth);
        void DebugRender(Color color, float lineWidth);
    }
    /// <summary>
    /// A 3D space partitioning tree that recursively divides aabbs into 8 smaller aabbs depending on the items they contain.
    /// </summary>
    public class Octree : Octree<I3DRenderable>
    {
        public Octree(BoundingBoxStruct bounds) : base(bounds) { }
        public Octree(BoundingBoxStruct bounds, List<I3DRenderable> items) : base(bounds, items) { }
    }
    /// <summary>
    /// A 3D space partitioning tree that recursively divides aabbs into 8 smaller aabbs depending on the items they contain.
    /// </summary>
    /// <typeparam name="T">The item type to use. Must be a class deriving from I3DBoundable.</typeparam>
    public class Octree<T> where T : class, I3DRenderable
    {
        public const float MinimumUnit = 10.0f;
        public const int MaxChildNodeCount = 8;
        
        internal int ItemID = 0;

        private Node _head;
        //internal HashSet<T> AllItems { get; } = new HashSet<T>();
        //public int Count => AllItems.Count;

        public Octree(BoundingBoxStruct bounds)
        {
            _head = new Node(bounds, 0, 0, null, this);
            //Engine.PrintLine($"Octree array length with {MinimumUnit} minimum unit: {ArrayLength(bounds.HalfExtents).ToString()}");
        }
        public Octree(BoundingBoxStruct bounds, List<T> items) : this(bounds) => _head.AddHereOrSmaller(items);

        public int ArrayLength(Vec3 halfExtents)
        {
            float minExtent = TMath.Min(halfExtents.X, halfExtents.Y, halfExtents.Z);
            int divisions = 0;
            while (minExtent >= MinimumUnit)
            {
                minExtent *= 0.5f;
                ++divisions;
            }
            return (int)Math.Pow(MaxChildNodeCount, divisions);
        }

        public class RenderEquality : IEqualityComparer<I3DRenderable>
        {
            public bool Equals(I3DRenderable x, I3DRenderable y)
                => x.RenderInfo.SceneID == y.RenderInfo.SceneID;
            public int GetHashCode(I3DRenderable obj)
                => obj.RenderInfo.SceneID;
        }
        public void Remake() => Remake(_head.Bounds);
        public void Remake(BoundingBoxStruct newBounds)
        {
            List<I3DRenderable> renderables = new List<I3DRenderable>();
            _head.CollectAll(renderables);

            ItemID = 0;
            _head = new Node(newBounds, 0, 0, null, this);
            
            foreach (T item in renderables)
                if (!_head.AddHereOrSmaller(item))
                    _head.ForceAdd(item);
        }
        
        internal ConcurrentQueue<T> AddedItems { get; } = new ConcurrentQueue<T>();
        internal ConcurrentQueue<T> RemovedItems { get; } = new ConcurrentQueue<T>();
        internal ConcurrentQueue<T> MovedItems { get; } = new ConcurrentQueue<T>();
        internal void Swap()
        {
            while (MovedItems.TryDequeue(out T item))
                ((Node)item.RenderInfo.OctreeNode).ItemMoved_Internal(item);
            while (AddedItems.TryDequeue(out T item))
            {
                item.RenderInfo.SceneID = ItemID++;
                _head.AddHereOrSmaller(item);
            }
            while (RemovedItems.TryDequeue(out T item))
            {
                _head.RemoveHereOrSmaller(item);
                item.RenderInfo.SceneID = -1;
            }
        }
        
        public void Add(T value)
        {
            AddedItems.Enqueue(value);
        }
        public void Add(IEnumerable<T> value)
        {
            foreach (T item in value)
                Add(item);
        }
        public void Remove(T value)
        {
            RemovedItems.Enqueue(value);
        }

        public ThreadSafeList<T> FindAll(float radius, Vec3 point, EContainment containment)
            => FindAll(new Sphere(radius, point), containment);
        public ThreadSafeList<T> FindAll(TShape shape, EContainment containment)
        {
            ThreadSafeList<T> list = new ThreadSafeList<T>();
            _head.FindAll(shape, list, containment);
            return list;
        }
        public void CollectVisible(IVolume cullingVolume, RenderPasses passes, Camera camera, bool shadowPass)
        {
            passes.ShadowPass = shadowPass;
            if (cullingVolume != null)
                _head.CollectVisible(cullingVolume, passes, camera, shadowPass);
            else
                _head.CollectAll(passes, camera, shadowPass);
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
            public Node(BoundingBoxStruct bounds, int subDivIndex, int subDivLevel, Node parent, Octree<T> owner)
            {
                _bounds = bounds;
                _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
                _items = new ThreadSafeList<T>(_lock);
                _subNodes = new Node[MaxChildNodeCount];
                _subDivIndex = subDivIndex;
                _subDivLevel = subDivLevel;
                _parentNode = parent;

                Owner = owner;
            }

            protected int _subDivIndex, _subDivLevel;
            protected BoundingBoxStruct _bounds;
            protected List<T> _items;
            protected Node[] _subNodes;
            protected Node _parentNode;
            private readonly ReaderWriterLockSlim _lock;

            public Octree<T> Owner { get; set; }
            public Node ParentNode { get => _parentNode; set => _parentNode = value; }
            public int SubDivIndex { get => _subDivIndex; set => _subDivIndex = value; }
            public int SubDivLevel { get => _subDivLevel; set => _subDivLevel = value; }
            public List<T> Items => _items;
            public BoundingBoxStruct Bounds => _bounds;
            public Vec3 Center => _bounds.Translation;
            public Vec3 Min => _bounds.Minimum;
            public Vec3 Max => _bounds.Maximum;
            
            public BoundingBoxStruct? GetSubdivision(int index)
            {
                Node node = _subNodes[index];
                if (node != null)
                    return node.Bounds;

                if (Min.X >= Max.X || 
                    Min.Y >= Max.Y || 
                    Min.Z >= Max.Z)
                    return null;

                Vec3 center = Center;
                switch (index)
                {
                    case 0: return BoundingBoxStruct.FromMinMax(new Vec3(Min.X, Min.Y, Min.Z), new Vec3(center.X, center.Y, center.Z));
                    case 1: return BoundingBoxStruct.FromMinMax(new Vec3(Min.X, Min.Y, center.Z), new Vec3(center.X, center.Y, Max.Z));
                    case 2: return BoundingBoxStruct.FromMinMax(new Vec3(Min.X, center.Y, Min.Z), new Vec3(center.X, Max.Y, center.Z));
                    case 3: return BoundingBoxStruct.FromMinMax(new Vec3(Min.X, center.Y, center.Z), new Vec3(center.X, Max.Y, Max.Z));
                    case 4: return BoundingBoxStruct.FromMinMax(new Vec3(center.X, Min.Y, Min.Z), new Vec3(Max.X, center.Y, center.Z));
                    case 5: return BoundingBoxStruct.FromMinMax(new Vec3(center.X, Min.Y, center.Z), new Vec3(Max.X, center.Y, Max.Z));
                    case 6: return BoundingBoxStruct.FromMinMax(new Vec3(center.X, center.Y, Min.Z), new Vec3(Max.X, Max.Y, center.Z));
                    case 7: return BoundingBoxStruct.FromMinMax(new Vec3(center.X, center.Y, center.Z), new Vec3(Max.X, Max.Y, Max.Z));
                }
                return null;
            }

            #region Child movement
            public void ItemMoved(I3DRenderable item) => ItemMoved(item as T);
            public void ItemMoved(T item)
            {
                //TODO: if the item is the only item within its volume, no need to subdivide more!!!
                //However, if the item is inserted into a volume with at least one other item in it, 
                //need to try subdividing for all items at that point.

                if (item?.RenderInfo.CullingVolume != null)
                    Owner.MovedItems.Enqueue(item);
            }
            internal void ItemMoved_Internal(T item)
            {
                //Still within the same volume?
                if (item.RenderInfo.CullingVolume.ContainedWithin(_bounds) == EContainment.Contains)
                {
                    //Try subdividing
                    for (int i = 0; i < MaxChildNodeCount; ++i)
                    {
                        BoundingBoxStruct? bounds = GetSubdivision(i);
                        if (bounds is null)
                            return;
                        if (item.RenderInfo.CullingVolume.ContainedWithin(bounds.Value) == EContainment.Contains)
                        {
                            bool shouldDestroy = RemoveHereOrSmaller(item);
                            if (shouldDestroy)
                                ClearSubNode(_subDivIndex);
                            CreateSubNode(bounds.Value, i)?.AddHereOrSmaller(item);
                            break;
                        }
                    }
                }
                else if (ParentNode != null)
                {
                    //Belongs in larger parent volume, remove from this node
                    bool shouldDestroy = RemoveHereOrSmaller(item);
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

                if (AddHereOrSmaller(item, childDestroyIndex))
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
            public void CollectVisible(IVolume cullingVolume, RenderPasses passes, Camera camera, bool shadowPass)
            {
                EContainment c = cullingVolume.Contains(_bounds);
                if (c != EContainment.Disjoint)
                {
                    if (c == EContainment.Contains)
                        CollectAll(passes, camera, shadowPass);
                    else
                    {
                        for (int i = 0; i < _items.Count; ++i)
                        {
                            I3DRenderable r = _items[i] as I3DRenderable;
#if EDITOR
                            if ((r is CameraComponent ccomp && ccomp.Camera == camera) || (r is Camera cam && cam == camera))
                                continue;
                            bool editMode = Engine.EditorState.InEditMode;
                            if (!editMode && r.RenderInfo.VisibleInEditorOnly)
                                continue;
#endif
                            bool allowRender = r.RenderInfo.Visible && (!shadowPass || r.RenderInfo.CastsShadows);
                            if (allowRender && (r.RenderInfo.CullingVolume == null || (c = cullingVolume.Contains(r.RenderInfo.CullingVolume)) != EContainment.Disjoint))
                            {
                                r.RenderInfo.LastRenderedTime = DateTime.Now;
                                r.AddRenderables(passes, camera);
                                if (passes.GetCommandsAddedCount() == 0)
                                {
                                    //Engine.LogWarning($"{nameof(I3DRenderable)} type {r.GetType().GetFriendlyName()} added no commands in {nameof(I3DRenderable.AddRenderables)}.");
                                }
                            }
                        }
                        
                        for (int i = 0; i < MaxChildNodeCount; ++i)
                            _subNodes[i]?.CollectVisible(cullingVolume, passes, camera, shadowPass);
                    }
                }
            }
            public void CollectAll(RenderPasses passes, Camera camera, bool shadowPass)
            {
                for (int i = 0; i < _items.Count; ++i)
                {
                    I3DRenderable r = _items[i] as I3DRenderable;
#if EDITOR
                    if ((r is CameraComponent ccomp && ccomp.Camera == camera) || (r is Camera c && c == camera))
                        continue;
                    bool editMode = Engine.EditorState.InEditMode;
                    if (!editMode && r.RenderInfo.VisibleInEditorOnly)
                        continue;
#endif
                    bool allowRender = r.RenderInfo.Visible && (!shadowPass || r.RenderInfo.CastsShadows);
                    if (allowRender)
                    {
                        r.RenderInfo.LastRenderedTime = DateTime.Now;
                        r.AddRenderables(passes, camera);
                        if (passes.GetCommandsAddedCount() == 0)
                        {
                            //Engine.LogWarning($"{nameof(I3DRenderable)} type {r.GetType().GetFriendlyName()} added no commands in {nameof(I3DRenderable.AddRenderables)}.");
                        }
                    }
                }

                for (int i = 0; i < MaxChildNodeCount; ++i)
                    _subNodes[i]?.CollectAll(passes, camera, shadowPass);
            }
            public void CollectAll(List<I3DRenderable> renderables)
            {
                for (int i = 0; i < _items.Count; ++i)
                    renderables.Add(_items[i]);
                
                for (int i = 0; i < MaxChildNodeCount; ++i)
                    _subNodes[i]?.CollectAll(renderables);
            }
            #endregion

            #region Add/Remove
            /// <summary>
            /// Returns true if this node no longer contains anything.
            /// </summary>
            /// <param name="item">The item to remove.</param>
            public bool RemoveHereOrSmaller(T item)
            {
                if (_items.Contains(item))
                    RemoveHere(item);
                else
                    for (int i = 0; i < MaxChildNodeCount; ++i)
                    {
                        Node node = _subNodes[i];
                        if (node != null)
                        {
                            if (node.RemoveHereOrSmaller(item))
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
            internal void AddHereOrSmaller(List<T> items)
            {
                foreach (T item in items)
                    AddHereOrSmaller(item, -1);
            }
            /// <summary>
            /// Adds an item to this node. May subdivide.
            /// </summary>
            /// <param name="items">The item to add.</param>
            /// <param name="forceAddToThisNode">If true, will add the item regardless of if its culling volume fits within the node's bounds.</param>
            /// <returns>True if the node was added.</returns>
            internal bool AddHereOrSmaller(T item, int ignoreSubNode = -1)
            {
                if (item.RenderInfo.CullingVolume != null)
                {
                    if (_bounds.Contains(item.RenderInfo.CullingVolume) != EContainment.Contains)
                        return false;

                    for (int i = 0; i < MaxChildNodeCount; ++i)
                    {
                        if (i == ignoreSubNode)
                            continue;

                        BoundingBoxStruct? subDiv = GetSubdivision(i);
                        if (!(subDiv is null) && subDiv.Value.Contains(item.RenderInfo.CullingVolume) == EContainment.Contains)
                        {
                            CreateSubNode(subDiv.Value, i)?.AddHereOrSmaller(item);
                            return true;
                        }
                    }
                }
                
                AddHere(item);
                return true;
            }
            #endregion
            
            internal void AddHere(T item)
            {
                _items.Add(item);
                item.RenderInfo.OctreeNode = this;
            }
            internal void RemoveHere(T item)
            {
                _items.Remove(item);
                item.RenderInfo.OctreeNode = null;
            }
            
            #region Convenience methods
            public T FindClosest(Vec3 point, ref float closestDistance)
            {
                if (!_bounds.Contains(point))
                    return null;
                
                //IsLoopingSubNodes = true;
                foreach (Node n in _subNodes)
                {
                    T t = n?.FindClosest(point, ref closestDistance);
                    if (t != null)
                        return t;
                }
                //IsLoopingSubNodes = false;
                
                if (_items.Count == 0)
                    return null;

                T closest = null;

                //IsLoopingItems = true;
                foreach (T item in _items)
                    if (item.RenderInfo.CullingVolume != null)
                    {
                        float dist = item.RenderInfo.CullingVolume.ClosestPoint(point).DistanceToFast(point);
                        if (dist < closestDistance)
                        {
                            closestDistance = dist;
                            closest = item;
                        }
                    }
                //IsLoopingItems = false;

                return closest;
            }
            public void FindAll(TShape shape, ThreadSafeList<T> list, EContainment containment)
            {
                EContainment c = shape.ContainedWithin(Bounds);
                if (c == EContainment.Intersects)
                {
                    //Compare each item separately
                    //IsLoopingItems = true;
                    foreach (T item in _items)
                        if (item.RenderInfo.CullingVolume != null)
                        {
                            c = shape.Contains(item.RenderInfo.CullingVolume);
                            if (c == containment)
                                list.Add(item);
                        }
                    //IsLoopingItems = false;
                }
                else if (c == containment)
                {
                    //All items already have this containment
                    //IsLoopingItems = true;
                    list.AddRange(_items);
                    //IsLoopingItems = false;
                }
                else //Not what we want
                    return;

                //IsLoopingSubNodes = true;
                foreach (Node n in _subNodes)
                    n?.FindAll(shape, list, containment);
                //IsLoopingSubNodes = false;
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
            private Node CreateSubNode(BoundingBoxStruct bounds, int index)
            {
                try
                {
                    //IsLoopingSubNodes = true;
                    if (_subNodes[index] != null)
                        return _subNodes[index];

                    return _subNodes[index] = new Node(bounds, index, _subDivLevel + 1, this, Owner);
                }
                finally
                {
                    //IsLoopingSubNodes = false;
                }
            }

            internal void ForceAdd(T value)
            {
                AddHere(value);
            }
            #endregion
        }

        private bool Uncache(T item)
        {
            //bool exists = AllItems.Remove(item);
            item.RenderInfo.SceneID = -1;
            //return exists;
            return true;
        }

        private bool Cache(T item)
        {
            //bool success = AllItems.Add(item);
            item.RenderInfo.SceneID = ItemID++;
            //return success;
            return true;
        }
    }
}
