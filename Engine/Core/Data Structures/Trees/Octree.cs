using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using TheraEngine;
using TheraEngine.Components.Scene;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;

namespace System
{
    public struct BoundingBoxStruct
    {
        public Vec3 HalfExtents;
        public Vec3 Translation;
        public Vec3 Minimum
        {
            get => Translation - HalfExtents;
            set
            {
                Vec3 max = Maximum;
                Translation = (max + value) / 2.0f;
                HalfExtents = (max - value) / 2.0f;
            }
        }
        public Vec3 Maximum
        {
            get => Translation + HalfExtents;
            set
            {
                Vec3 min = Minimum;
                Translation = (value + min) / 2.0f;
                HalfExtents = (value - min) / 2.0f;
            }
        }

        #region Constructors
        public BoundingBoxStruct(float uniformHalfExtents)
            : this(new Vec3(uniformHalfExtents)) { }
        public BoundingBoxStruct(float uniformHalfExtents, Vec3 translation)
            : this(new Vec3(uniformHalfExtents), translation) { }
        public BoundingBoxStruct(float halfExtentX, float halfExtentY, float halfExtentZ)
            : this(new Vec3(halfExtentX, halfExtentY, halfExtentZ)) { }
        public BoundingBoxStruct(float halfExtentX, float halfExtentY, float halfExtentZ, Vec3 translation)
            : this(new Vec3(halfExtentX, halfExtentY, halfExtentZ), translation) { }
        public BoundingBoxStruct(float halfExtentX, float halfExtentY, float halfExtentZ, float x, float y, float z)
            : this(new Vec3(halfExtentX, halfExtentY, halfExtentZ), new Vec3(x, y, z)) { }
        public BoundingBoxStruct(Vec3 halfExtents)
            : this(halfExtents, Vec3.Zero) { }
        public BoundingBoxStruct(EventVec3 halfExtents)
            : this(halfExtents, new EventVec3(Vec3.Zero)) { }
        public BoundingBoxStruct(Vec3 halfExtents, Vec3 translation)
            : this()
        {
            HalfExtents = halfExtents;
            Translation = translation;
        }
        #endregion
        
        //Half extents is one of 8 octants that make up the box, so multiply half extent volume by 8
        public float GetVolume() =>
            HalfExtents.X * HalfExtents.Y * HalfExtents.Z * 8.0f;
        //Each half extent side is one of 4 quadrants on both sides of the box, so multiply each side area by 8
        public float GetSurfaceArea() =>
            HalfExtents.X * HalfExtents.Y * 8.0f +
            HalfExtents.Y * HalfExtents.Z * 8.0f +
            HalfExtents.Z * HalfExtents.X * 8.0f;

        /// <summary>
        /// Expands this bounding box to include the given point.
        /// </summary>
        public void Expand(Vec3 point)
        {
            Vec3 min = Vec3.ComponentMin(point, Minimum);
            Vec3 max = Vec3.ComponentMax(point, Maximum);
            Translation = (max + min) / 2.0f;
            HalfExtents = (max - min) / 2.0f;
        }
        public void Expand(BoundingBoxStruct box)
        {
            Vec3 min = Vec3.ComponentMin(box.Minimum, box.Maximum, Minimum);
            Vec3 max = Vec3.ComponentMax(box.Minimum, box.Maximum, Maximum);
            Translation = (max + min) / 2.0f;
            HalfExtents = (max - min) / 2.0f;
        }

        #region Collision
        /// <summary>
        /// Returns true if the given <see cref="Ray"/> intersects this <see cref="BoundingBoxStruct"/>.
        /// </summary>
        public bool Intersects(Ray ray)
            => Collision.RayIntersectsAABBDistance(ray.StartPoint, ray.Direction, Minimum, Maximum, out float distance);
        /// <summary>
        /// Returns true if the given <see cref="Ray"/> intersects this <see cref="BoundingBoxStruct"/>.
        /// Returns the distance of the closest intersection.
        /// </summary>
        public bool Intersects(Ray ray, out float distance)
            => Collision.RayIntersectsAABBDistance(ray, Minimum, Maximum, out distance);
        public bool Intersects(Vec3 start, Vec3 direction, out float distance)
            => Collision.RayIntersectsAABBDistance(start, direction, Minimum, Maximum, out distance);
        /// <summary>
        /// Returns true if the given <see cref="Ray"/> intersects this <see cref="BoundingBoxStruct"/>.
        /// Returns the position of the closest intersection.
        /// </summary>
        public bool Intersects(Ray ray, out Vec3 point)
            => Collision.RayIntersectsAABB(ray, Minimum, Maximum, out point);
        //public bool Intersects(Vec3 start, Vec3 direction, out Vec3 point)
        //    => Collision.RayIntersectsAABB(start, direction, Minimum, Maximum, out point);

        public bool Contains(Vec3 point)
            => Collision.AABBContainsPoint(Minimum, Maximum, point);
        public EContainment Contains(BoundingBox box)
            => Collision.AABBContainsAABB(Minimum, Maximum, box.Minimum, box.Maximum);
        public EContainment Contains(BoundingBoxStruct box)
            => Collision.AABBContainsAABB(Minimum, Maximum, box.Minimum, box.Maximum);
        public EContainment Contains(Box box)
            => Collision.AABBContainsBox(Minimum, Maximum, box.HalfExtents, box.Transform.Matrix);
        public EContainment Contains(Sphere sphere)
            => Collision.AABBContainsSphere(Minimum, Maximum, sphere.Center, sphere.Radius);
        public EContainment Contains(Cone cone)
        {
            bool top = Contains(cone.GetTopPoint());
            bool bot = Contains(cone.GetBottomCenterPoint());
            if (top && bot)
                return EContainment.Contains;
            else if (!top && !bot)
                return EContainment.Disjoint;
            return EContainment.Intersects;
        }
        public EContainment Contains(Cylinder cylinder)
        {
            bool top = Contains(cylinder.GetTopCenterPoint());
            bool bot = Contains(cylinder.GetBottomCenterPoint());
            if (top && bot)
                return EContainment.Contains;
            else if (!top && !bot)
                return EContainment.Disjoint;
            return EContainment.Intersects;
        }
        public EContainment Contains(Capsule capsule)
        {
            Vec3 top = capsule.GetTopCenterPoint();
            Vec3 bot = capsule.GetBottomCenterPoint();
            Vec3 radiusVec = new Vec3(capsule.Radius);
            Vec3 capsuleMin = Vec3.ComponentMin(top, bot) - radiusVec;
            Vec3 capsuleMax = Vec3.ComponentMax(top, bot) + radiusVec;
            Vec3 min = Minimum;
            Vec3 max = Maximum;

            bool containsX = false, containsY = false, containsZ = false;
            bool disjointX = false, disjointY = false, disjointZ = false;

            containsX = capsuleMin.X >= min.X && capsuleMax.X <= max.X;
            containsY = capsuleMin.Y >= min.Y && capsuleMax.Y <= max.Y;
            containsZ = capsuleMin.Z >= min.Z && capsuleMax.Z <= max.Z;

            if (!containsX) disjointX = capsuleMax.X < min.X || capsuleMin.X > max.X;
            if (!containsY) disjointY = capsuleMax.Y < min.Y || capsuleMin.Y > max.Y;
            if (!containsZ) disjointZ = capsuleMax.Z < min.Z || capsuleMin.Z > max.Z;

            if (containsX && containsY && containsZ)
                return EContainment.Contains;
            if (disjointX && disjointY && disjointZ)
                return EContainment.Disjoint;
            return EContainment.Intersects;
        }
        public EContainment Contains(Shape shape)
        {
            if (shape != null)
            {
                if (shape is BoundingBox bb)
                    return Contains(bb);
                else if (shape is Box box)
                    return Contains(box);
                else if (shape is Sphere sphere)
                    return Contains(sphere);
                else if (shape is Cone cone)
                    return Contains(cone);
                else if (shape is Cylinder cylinder)
                    return Contains(cylinder);
                else if (shape is Capsule capsule)
                    return Contains(capsule);
            }
            return EContainment.Contains;
        }
        #endregion

        #region Static Constructors
        /// <summary>
        /// Creates a new bounding box from minimum and maximum coordinates.
        /// </summary>
        public static BoundingBoxStruct FromMinMax(Vec3 min, Vec3 max)
            => new BoundingBoxStruct((max - min) * 0.5f, (max + min) * 0.5f);
        /// <summary>
        /// Creates a new bounding box from half extents and a translation.
        /// </summary>
        public static BoundingBoxStruct FromHalfExtentsTranslation(Vec3 halfExtents, Vec3 translation)
            => new BoundingBoxStruct(halfExtents, translation);
        /// <summary>
        /// Creates a new bounding box from half extents and a translation.
        /// </summary>
        public static BoundingBoxStruct FromHalfExtentsTranslation(EventVec3 halfExtents, EventVec3 translation)
            => new BoundingBoxStruct(halfExtents, translation);
        /// <summary>
        /// Creates a bounding box that encloses the given sphere.
        /// </summary>
        public static BoundingBoxStruct EnclosingSphere(Sphere sphere)
            => FromMinMax(sphere.Center - sphere.Radius, sphere.Center + sphere.Radius);
        /// <summary>
        /// Creates a bounding box that includes both given bounding boxes.
        /// </summary>
        public static BoundingBoxStruct Merge(BoundingBoxStruct box1, BoundingBoxStruct box2)
            => FromMinMax(Vec3.ComponentMin(box1.Minimum, box2.Maximum), Vec3.ComponentMax(box1.Maximum, box2.Maximum));
        #endregion

        public static bool operator ==(BoundingBoxStruct left, BoundingBoxStruct right) => left.Equals(ref right);
        public static bool operator !=(BoundingBoxStruct left, BoundingBoxStruct right) => !left.Equals(ref right);

        public bool Equals(ref BoundingBoxStruct value)
            => Minimum == value.Minimum && Maximum == value.Maximum;
        
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "Minimum:{0} Maximum:{1}", Minimum.ToString(), Maximum.ToString());
        }
        public override bool Equals(object value)
        {
            if (!(value is BoundingBoxStruct))
                return false;

            var strongValue = (BoundingBoxStruct)value;
            return Equals(ref strongValue);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return (Minimum.GetHashCode() * 397) ^ Maximum.GetHashCode();
            }
        }
        public Vec3 ClosestPoint(Vec3 point)
            => Collision.ClosestPointAABBPoint(Minimum, Maximum, point);
    }
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
        internal HashSet<T> AllItems { get; } = new HashSet<T>();
        public int Count => AllItems.Count;

        public Octree(BoundingBoxStruct bounds)
        {
            _head = new Node(bounds, 0, 0, null, this);
            Engine.PrintLine($"Octree array length with {MinimumUnit} minimum unit: {ArrayLength(bounds.HalfExtents).ToString()}");
        }
        public Octree(BoundingBoxStruct bounds, List<T> items) : this(bounds) => _head.Add(items);

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
            _head = new Node(newBounds, 0, 0, null, this);
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
            protected ThreadSafeList<T> _items;
            protected Node[] _subNodes;
            protected Node _parentNode;
            private readonly ReaderWriterLockSlim _lock;

            public Octree<T> Owner { get; set; }
            public Node ParentNode { get => _parentNode; set => _parentNode = value; }
            public int SubDivIndex { get => _subDivIndex; set => _subDivIndex = value; }
            public int SubDivLevel { get => _subDivLevel; set => _subDivLevel = value; }
            public ThreadSafeList<T> Items => _items;
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
                        BoundingBoxStruct? bounds = GetSubdivision(i);
                        if (bounds is null)
                            return;
                        if (item.CullingVolume.ContainedWithin(bounds.Value) == EContainment.Contains)
                        {
                            bool shouldDestroy = Remove(item);
                            if (shouldDestroy)
                                ClearSubNode(_subDivIndex);
                            CreateSubNode(bounds.Value, i)?.Add(item);
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
            public void CollectVisible(IVolume cullingVolume, RenderPasses passes, Camera camera, bool shadowPass)
            {
                EContainment c = cullingVolume.Contains(_bounds);
                if (c != EContainment.Disjoint)
                {
                    if (c == EContainment.Contains)
                        CollectAll(passes, camera, shadowPass);
                    else
                    {
                        IsLoopingItems = true;
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
                            if (allowRender && (r.CullingVolume == null || (c = cullingVolume.Contains(r.CullingVolume)) != EContainment.Disjoint))
                            {
                                r.RenderInfo.LastRenderedTime = DateTime.Now;
                                r.AddRenderables(passes, camera);
                                if (passes.GetCommandsAddedCount() == 0)
                                {
                                    //Engine.LogWarning($"{nameof(I3DRenderable)} type {r.GetType().GetFriendlyName()} added no commands in {nameof(I3DRenderable.AddRenderables)}.");
                                }
                            }
                        }
                        IsLoopingItems = false;

                        IsLoopingSubNodes = true;
                        for (int i = 0; i < MaxChildNodeCount; ++i)
                            _subNodes[i]?.CollectVisible(cullingVolume, passes, camera, shadowPass);
                        IsLoopingSubNodes = false;
                    }
                }
            }
            public void CollectAll(RenderPasses passes, Camera camera, bool shadowPass)
            {
                IsLoopingItems = true;
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
                IsLoopingItems = false;

                IsLoopingSubNodes = true;
                for (int i = 0; i < MaxChildNodeCount; ++i)
                    _subNodes[i]?.CollectAll(passes, camera, shadowPass);
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

                        BoundingBoxStruct? bounds = GetSubdivision(i);
                        if (bounds is null)
                        {
                            return QueueAdd(item);
                        }
                        if (item.CullingVolume.ContainedWithin(bounds.Value) == EContainment.Contains)
                        {
                            CreateSubNode(bounds.Value, i)?.Add(item);
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
                if (item == null)
                    return false;

                if (IsLoopingItems)
                {
                    _itemQueue.Enqueue(new Tuple<bool, T>(true, item));
                    return false;
                }
                else
                {
                    if (Owner.Cache(item))
                    {
                        _items.Add(item);
                        item.OctreeNode = this;
                    }
                    return true;
                }
            }
            private bool QueueRemove(T item)
            {
                if (item == null)
                    return false;

                if (IsLoopingItems)
                {
                    _itemQueue.Enqueue(new Tuple<bool, T>(false, item));
                    return false;
                }
                else
                {
                    if (Owner.Uncache(item))
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
            private Node CreateSubNode(BoundingBoxStruct bounds, int index)
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

        private bool Uncache(T item)
        {
            bool exists = AllItems.Remove(item);
            item.RenderInfo.SceneID = -1;
            return exists;
        }

        private bool Cache(T item)
        {
            bool success = AllItems.Add(item);
            item.RenderInfo.SceneID = ItemID++;
            return success;
        }
    }
}
