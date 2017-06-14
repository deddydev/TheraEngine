using TheraEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;

namespace System
{
    public class RenderOctree : Octree<IRenderable, RenderNode>
    {
        public RenderOctree(BoundingBox bounds) : base(bounds) { }
        public RenderOctree(BoundingBox bounds, List<IRenderable> items) : base(bounds, items) { }

        /// <summary>
        /// Disables rendering of scene items not visible from the given camera.
        /// </summary>
        /// <param name="frustum"></param>
        /// <param name="resetVisibility">If false, does not reset the visibility of objects that have been previously enabled.</param>
        /// <param name="cullOffscreen">If true, will disable the visibility of offscreen objects.</param>
        /// <param name="passes">The varying passes for objects of different types.</param>
        /// <param name="debugRender">If true, bounding boxes for visible octree subdivisions will be rendered.</param>
        public void Cull(
            Camera camera,
            bool resetVisibility,
            bool cullOffscreen,
            RenderPasses passes,
            bool debugRender)
        {
            if (_head == null)
                return;
            
            Frustum frustum = camera.GetFrustum();
            _head.Cull(frustum, debugRender, passes);
        }
    }
    public class RenderNode : OctreeNode<IRenderable>
    {
        public RenderNode(BoundingBox bounds) : base(bounds) { }

        public void Cull(Frustum frustum, bool debugRender, RenderPasses passes)
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
                                passes.TransparentForward.PushFront(item);
                            }
                            else
                            {
                                if (Engine.Settings.ShadingStyle == ShadingStyle.Deferred)
                                    passes.OpaqueDeferred.PushFront(item);
                                else
                                    passes.OpaqueForward.PushFront(item);
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
