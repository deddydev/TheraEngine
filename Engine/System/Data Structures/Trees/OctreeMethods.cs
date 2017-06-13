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
    public partial class Octree<T, T2> where T : I3DBoundable where T2 : OctreeNode<T>
    {
        public ThreadSafeList<T> FindClosest(Vec3 point) { return _head?.FindClosest(point); }
        public ThreadSafeList<T> FindAllInside(Shape shape, bool allowPartialContains, bool testVisibleOnly)
        {
            ThreadSafeList<T> list = new ThreadSafeList<T>();
            _head?.FindAllInside(shape, list, allowPartialContains, testVisibleOnly);
            return list;
        }

        public void Cull(Frustum frustum, bool debugRender)
        => _head?.Cull(frustum, debugRender);
        public void DebugRender()
            => _head?.DebugRender(true);

        public void Add(T value)
        {
            if (_head == null)
                _head = (T2)Activator.CreateInstance(typeof(T2), _totalBounds);
            _head.Add(value, true);
        }
        public void Add(List<T> value)
        {
            if (_head == null)
                _head = (T2)Activator.CreateInstance(typeof(T2), _totalBounds);
            _head.Add(value, true);
        }
        public bool Remove(T value)
        {
            if (_head == null)
                return false;

            bool removed = _head.Remove(value, out bool destroy);
            if (destroy)
                _head = default(T2);
            return removed;
        }
    }
    public partial class OctreeNode<T> : IOctreeNode where T : I3DBoundable
    {
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
                        if (IsLoopingItems)
                            _itemQueue.Enqueue(new Tuple<bool, T>(false, item));
                        else
                            _items.Remove(item);

                        CreateSubNode(bounds, i);
                        _subNodes[i].Add(item);
                        break;
                    }
                }
            }
            else if (ParentNode != null)
            {
                //Belongs in larger parent volume, remove from this node
                Remove(item, out bool shouldDestroy);
                if (!ParentNode.AddReversedHierarchy(item, shouldDestroy ? _subDivIndex : -1))
                {
                    //Force add to root node
                    OctreeNode<T> m = this;
                    while (m.ParentNode != null)
                        m = m.ParentNode;
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
                if (ParentNode != null)
                {
                    if (ParentNode.AddReversedHierarchy(item, shouldDestroy ? _subDivIndex : -1))
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
        public void DebugRender(bool recurse)
        {
            Engine.Renderer.RenderAABB("OctSubDiv" + _subDivLevel + "-" + _subDivIndex, _bounds.HalfExtents, _bounds.Translation, false, Color.Gray, 5.0f);
            if (recurse)
                foreach (OctreeNode<T> n in _subNodes)
                    n?.DebugRender(true);
        }
        public void Cull(Frustum frustum, bool debugRender)
        {
            EContainment c = frustum.Contains(_bounds);
            if (c != EContainment.Intersects)
                Visible = c == EContainment.Contains;
            else
            {
                IsLoopingItems = true;
                try
                {
                    //Bounds is intersecting edge of frustum
                    foreach (T item in _items)
                        item.IsRendering = item.CullingVolume != null ? item.CullingVolume.ContainedWithin(frustum) != EContainment.Disjoint : true;
                }
                catch
                {
                    Debug.WriteLine("OCTREE: Error looping through items to cull");
                }
                finally
                {
                    IsLoopingItems = false;
                }

                IsLoopingSubNodes = true;
                try
                {
                    foreach (OctreeNode<T> n in _subNodes)
                        n?.Cull(frustum, debugRender);
                }
                catch
                {
                    Debug.WriteLine("OCTREE: Error looping through sub nodes to cull");
                }
                finally
                {
                    IsLoopingSubNodes = false;
                }
            }

            if (debugRender && Visible)
                DebugRender(false);
        }
        internal bool Remove(T value, out bool shouldDestroy)
        {
            bool hasBeenRemoved = false;
            bool anyNotNull = false;
            if (_items.Contains(value))
            {
                QueueRemove(value);
                hasBeenRemoved = true;
            }
            else
            {
                for (int i = 0; i < 8; ++i)
                {
                    OctreeNode<T> node = _subNodes[i];
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
                //if (!anyNotNull)
                //    _subNodes = null;
            }
            shouldDestroy = _items.Count == 0 && !anyNotNull;
            return hasBeenRemoved;
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
                addedAny = addedAny || Add(item, force);
            return addedAny;
        }
        /// <summary>
        /// Adds an item to this node. May subdivide.
        /// </summary>
        /// <param name="items">The item to add.</param>
        /// <param name="force">If true, will add the item regardless of if its culling volume fits within the node's bounds.</param>
        /// <returns>True if the node was added.</returns>
        internal bool Add(T item, bool force = false)
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

        #region Misc Data Methods
        public ThreadSafeList<T> FindClosest(Vec3 point)
        {
            if (_bounds.Contains(point))
            {
                ThreadSafeList<T> list = null;
                lock (_lock)
                {
                    IsLoopingItems = true;
                    try
                    {
                        foreach (OctreeNode<T> node in _subNodes)
                            if (node != null)
                            {
                                list = node.FindClosest(point);
                                if (list != null)
                                    return list;
                            }
                    }
                    catch
                    {
                        Debug.WriteLine("OCTREE: Error looping through items to find closest to point");
                    }
                    finally
                    {
                        IsLoopingItems = false;
                    }
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
            foreach (OctreeNode<T> node in _subNodes)
                if (node != null)
                    list.AddRange(node.CollectChildren());
            return list;
        }
        #endregion

        #region Private Helper Methods
        private void QueueAdd(T item)
        {
            if (IsLoopingItems)
                _itemQueue.Enqueue(new Tuple<bool, T>(true, item));
            else
                _items.Add(item);
        }
        private void QueueRemove(T item)
        {
            if (IsLoopingItems)
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
                    //if (!_subNodes.Any(x => x != null))
                    //    _subNodes = null;
                }
            }
        }
        private bool HasNoSubNodesExcept(int index)
        {
            lock (_lock)
            {
                for (int i = 0; i < 8; ++i)
                    if (_subNodes[i] == null && i != index)
                        return false;
            }
            return true;
        }
        private void CreateSubNode(BoundingBox bounds, int index)
        {
            lock (_lock)
            {
                if (_subNodes[index] != null)
                    return;

                OctreeNode<T> node = new OctreeNode<T>(bounds);
                node.Visible = Visible;
                node.SubDivIndex = index;
                node.SubDivLevel = _subDivLevel + 1;
                node.ParentNode = this;
                _subNodes[index] = node;
            }
        }
    }
    #endregion
}
