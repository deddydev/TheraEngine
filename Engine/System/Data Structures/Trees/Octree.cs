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
    public interface IOctreeNode
    {
        bool Visible { get; set; }
        BoundingBox Bounds { get; }
        Vec3 Center { get; }
        Vec3 Min { get; }
        Vec3 Max { get; }
        int SubDivLevel { get; set; }
        int SubDivIndex { get; set; }
        
        void Cull(Frustum frustum, bool debugRender);
        void DebugRender(bool v);
        void ItemMoved(I3DBoundable item);
    }
    public class Octree : Octree<I3DBoundable, OctreeNode<I3DBoundable>>
    {
        public Octree(BoundingBox bounds) : base(bounds) { }
        public Octree(BoundingBox bounds, List<I3DBoundable> items) : base(bounds, items) { }
    }
    public partial class Octree<T, T2> where T : I3DBoundable where T2 : OctreeNode<T>
    {
        private BoundingBox _totalBounds;
        protected T2 _head;
        
        public Octree(BoundingBox bounds) { _totalBounds = bounds; }
        public Octree(BoundingBox bounds, List<T> items)
        {
            _totalBounds = bounds;
            Add(items);
        }
    }
    public partial class OctreeNode<T> : IOctreeNode where T : I3DBoundable
    {
        public OctreeNode(BoundingBox bounds) { _bounds = bounds; }

        protected int _subDivIndex = 0, _subDivLevel = 0;
        protected bool _visible = true;
        protected BoundingBox _bounds;
        protected ThreadSafeList<T> _items = new ThreadSafeList<T>();
        protected OctreeNode<T>[] _subNodes = new OctreeNode<T>[8];
        protected OctreeNode<T> parentNode;
        protected object _lock = new object();

        public OctreeNode<T> ParentNode { get => parentNode; set => parentNode = value; }
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
                    Debug.WriteLine("OCTREE: Error looping through items to set visibility");
                }
                finally
                {
                    IsLoopingItems = false;
                }

                //No need to set visibility of child nodes if this node isn't visible.
                if (_visible)
                {
                    try
                    {
                        IsLoopingSubNodes = true;
                        foreach (OctreeNode<T> node in _subNodes)
                            if (node != null)
                                node.Visible = _visible;
                    }
                    catch
                    {
                        Debug.WriteLine("OCTREE: Error looping through sub nodes to set visibility");
                    }
                    finally
                    {
                        IsLoopingSubNodes = false;
                    }
                }
            }
        }
        public BoundingBox GetSubdivision(int index)
        {
            //IsLoopingItems = true;
            //Node[] subNodes = _subNodes;
            //Node node;
            lock (_lock)
            {
                OctreeNode<T> node = _subNodes[index];
                if (node != null)
                {
                    //IsLoopingItems = false;
                    return node.Bounds;
                }
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
        
        #region Loop Threading Backlog

        //Backlog for adding and removing items when other threads are currently looping
        protected ConcurrentQueue<Tuple<bool, T>> _itemQueue = new ConcurrentQueue<Tuple<bool, T>>();
        //Backlog for setting sub nodes when other threads are currently looping
        protected ConcurrentQueue<Tuple<int, OctreeNode<T>>> _subNodeQueue = new ConcurrentQueue<Tuple<int, OctreeNode<T>>>();
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
                        _items.Add(result.Item2);
                    else
                        _items.Remove(result.Item2);
                }
            }
        }
        protected bool IsLoopingSubNodes
        {
            get => _isLoopingSubNodes;
            set
            {
                _isLoopingSubNodes = value;
                while (!_isLoopingSubNodes && !_subNodeQueue.IsEmpty && _subNodeQueue.TryDequeue(out Tuple<int, OctreeNode<T>> result))
                    _subNodes[result.Item1] = result.Item2;
            }
        }

        #endregion
    }
}
