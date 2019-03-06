using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Rendering
{
    /// <summary>
    /// Processes all scene information that will be sent to the renderer.
    /// </summary>
    public class Scene2D : BaseScene
    {
        [Category("Scene 2D")]
        public Quadtree<I2DRenderable> RenderTree { get; private set; }
        [Category("Scene 2D")]
        public EventList<I2DRenderable> Renderables { get; }
        //public override int Count => Renderables.Count;

        public Scene2D() : this(Vec2.Zero) { }
        public Scene2D(Vec2 bounds)
        {
            Render = DoRender;
            Clear(bounds);

            Renderables = new EventList<I2DRenderable>();
            //Renderables.PostAnythingAdded += Renderables_PostAnythingAdded;
            //Renderables.PostAnythingRemoved += Renderables_PostAnythingRemoved;
        }

        private void Renderables_PostAnythingAdded(I2DRenderable item) => RenderTree.Add(item);
        private void Renderables_PostAnythingRemoved(I2DRenderable item) => RenderTree.Remove(item);

        //public void CollectVisibleRenderables(Frustum frustum)
        //{
        //    bool hasTopLeft = Collision.RayIntersectsPlane(frustum.NearTopLeft, frustum.FarTopLeft - frustum.NearTopLeft, Vec3.Zero, Vec3.Backward, out Vec3 topLeft);
        //    bool hasTopRight = Collision.RayIntersectsPlane(frustum.NearTopRight, frustum.FarTopRight - frustum.NearTopRight, Vec3.Zero, Vec3.Backward, out Vec3 topRight);
        //    bool hasBottomLeft = Collision.RayIntersectsPlane(frustum.NearBottomLeft, frustum.FarBottomLeft - frustum.NearBottomLeft, Vec3.Zero, Vec3.Backward, out Vec3 bottomLeft);
        //    bool hasBottomRight = Collision.RayIntersectsPlane(frustum.NearBottomRight, frustum.FarBottomRight - frustum.NearBottomRight, Vec3.Zero, Vec3.Backward, out Vec3 bottomRight);

        //    float minX = TMath.Min(topLeft.X, topRight.X, bottomLeft.X, bottomRight.X);
        //    float maxX = TMath.Max(topLeft.X, topRight.X, bottomLeft.X, bottomRight.X);
        //    float minY = TMath.Min(topLeft.Y, topRight.Y, bottomLeft.Y, bottomRight.Y);
        //    float maxY = TMath.Max(topLeft.Y, topRight.Y, bottomLeft.Y, bottomRight.Y);

        //    CollectVisibleRenderables(BoundingRectangle.FromMinMaxSides(minX, maxX, minY, maxY, 0.0f, 0.0f));
        //}
        public void CollectVisibleRenderables(BoundingRectangle bounds)
        {
            //RenderTree.CollectVisible(bounds, _passes);

            //foreach (I2DRenderable r in _renderables)
            //    r.AddRenderables(_passes);
        }
        public override void CollectVisible(RenderPasses populatingPasses, IVolume collectionVolume, Camera camera, bool shadowPass)
        {
            //RenderTree.CollectVisible(null, populatingPasses, camera);
            foreach (I2DRenderable r in Renderables)
                if (r.RenderInfo.Visible)
                    r.AddRenderables(populatingPasses, camera);
        }
        public I2DRenderable FindDeepest(Vec2 viewportPoint)
        {
            foreach (I2DRenderable r in Renderables)
            {
                if (r.RenderInfo.AxisAlignedRegion.Contains(viewportPoint))
                    return r;
            }
            return null;
        }

        public void Resize(Vec2 bounds)
        {
            RenderTree?.Remake(new BoundingRectangleFStruct(Vec2.Zero, bounds));
        }
        
        public void Clear(Vec2 bounds)
        {
            RenderTree = new Quadtree<I2DRenderable>(new BoundingRectangleFStruct(new Vec2(0.0f), bounds));
        }

        public override void RegenerateTree()
        {

        }

        public override void GlobalPreRender()
        {

        }

        public override void GlobalSwap()
        {
            RenderTree.Swap();
        }

        public override void GlobalUpdate()
        {

        }

        public void DoRender(RenderPasses renderingPasses, Camera camera, Viewport viewport, FrameBuffer target)
        {
            Engine.Renderer.PushCamera(camera);
            Engine.Renderer.PushCurrent2DScene(this);
            {
                if (viewport != null)
                {
                    viewport.RenderingCameras.Push(camera);

                    //Render the to the actual screen resolution
                    Engine.Renderer.PushRenderArea(viewport.Region);
                    {
                        target?.Bind(EFramebufferTarget.DrawFramebuffer);

                        Engine.Renderer.EnableDepthTest(true);
                        Engine.Renderer.ClearDepth(1.0f);
                        Engine.Renderer.Clear(EFBOTextureType.Color | EFBOTextureType.Depth);
                        renderingPasses.ClearRendering(ERenderPass.OpaqueDeferredLit);

                        Engine.Renderer.AllowDepthWrite(false);
                        renderingPasses.Render(ERenderPass.Background);

                        Engine.Renderer.AllowDepthWrite(true);
                        renderingPasses.Render(ERenderPass.OpaqueForward);
                        renderingPasses.Render(ERenderPass.TransparentForward);

                        //Disable depth fail for objects on top
                        Engine.Renderer.DepthFunc(EComparison.Always);
                        renderingPasses.Render(ERenderPass.OnTopForward);

                        //Engine.Renderer.EnableDepthTest(false);
                        //RenderTree.DebugRender(v.Region, false, 0.1f);

                        target?.Unbind(EFramebufferTarget.DrawFramebuffer);
                    }
                    Engine.Renderer.PopRenderArea();

                    viewport.RenderingCameras.Pop();
                }
                else
                {
                    target?.Bind(EFramebufferTarget.DrawFramebuffer);

                    Engine.Renderer.Clear(EFBOTextureType.Color | EFBOTextureType.Depth);

                    renderingPasses.ClearRendering(ERenderPass.OpaqueDeferredLit);

                    Engine.Renderer.AllowDepthWrite(false);
                    renderingPasses.Render(ERenderPass.Background);

                    Engine.Renderer.AllowDepthWrite(true);
                    renderingPasses.Render(ERenderPass.OpaqueForward);
                    renderingPasses.Render(ERenderPass.TransparentForward);

                    //Disable depth fail for objects on top
                    Engine.Renderer.DepthFunc(EComparison.Always);
                    renderingPasses.Render(ERenderPass.OnTopForward);

                    target?.Unbind(EFramebufferTarget.DrawFramebuffer);
                }
            }
            Engine.Renderer.PopCurrent2DScene();
            Engine.Renderer.PopCamera();
        }

        public override IEnumerator<IRenderable> GetEnumerator() => Renderables.GetEnumerator();
    }
}
