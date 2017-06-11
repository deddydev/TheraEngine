using TheraEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public class RenderOctree : Octree<IRenderable, RenderNode>
    {
        public RenderOctree(BoundingBox bounds) : base(bounds) { }
        public RenderOctree(BoundingBox bounds, List<IRenderable> items) : base(bounds, items) { }

        public void Cull(
            Frustum frustum,
            bool debugRender,
            Deque<IRenderable> opaque,
            Deque<IRenderable> transparent)
        {
            if (_head == null)
                return;

            ((RenderNode)_head).Cull(frustum, debugRender, opaque, transparent);
        }
    }
    public class RenderNode : OctreeNode<IRenderable>
    {
        public RenderNode(BoundingBox bounds) : base(bounds) { }

        public void Cull(Frustum frustum, bool debugRender, Deque<IRenderable> opaque, Deque<IRenderable> transparent)
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
                    foreach (IRenderable item in _items)
                    {
                        item.IsRendering = item.CullingVolume != null ? item.CullingVolume.ContainedWithin(frustum) != EContainment.Disjoint : true;
                        if (item.IsRendering)
                        {
                            if (item.HasTransparency)
                            {
                                transparent.PushFront(item);
                            }
                            else
                            {
                                opaque.PushFront(item);
                            }
                        }
                    }
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
                    foreach (RenderNode n in _subNodes)
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
    }
}
