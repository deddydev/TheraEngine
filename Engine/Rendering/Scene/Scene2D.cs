using System;
using System.Collections.Generic;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Rendering
{
    /// <summary>
    /// Processes all scene information that will be sent to the renderer.
    /// </summary>
    public class Scene2D : BaseScene
    {
        //public Quadtree<I2DRenderable> RenderTree { get; private set; }
        //public override int Count => _renderables.Count;// RenderTree.Count;

        private List<I2DRenderable> _renderables = new List<I2DRenderable>();

        public Scene2D()
        {
            Render = DoRender;
            Clear(Vec2.Zero);
        }

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
        //public void CollectVisibleRenderables(BoundingRectangle bounds)
        //{
        //    //RenderTree.CollectVisible(bounds, _passes);

        //    foreach (I2DRenderable r in _renderables)
        //        r.AddRenderables(_passes);
        //}
        public override void CollectVisible(RenderPasses populatingPasses, IVolume collectionVolume, Camera camera, bool shadowPass)
        {
            foreach (I2DRenderable r in _renderables)
                if (r.RenderInfo.Visible)
                    r.AddRenderables(populatingPasses);
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

        public I2DRenderable FindDeepest(Vec2 viewportPoint)
        {
            foreach (I2DRenderable r in _renderables)
            {
                if (r.RenderInfo.AxisAlignedRegion.Contains(viewportPoint))
                    return r;
            }
            return null;
        }

        public void Resize(Vec2 bounds)
        {
            //RenderTree?.Remake(new BoundingRectangle(Vec2.Zero, bounds));
        }

        public override void Add(IRenderable obj) => Add(obj as I2DRenderable);
        public override void Remove(IRenderable obj) => Remove(obj as I2DRenderable);
        public void Add(I2DRenderable obj)
        {
            if (obj == null)
                return;
            
            _renderables.Add(obj);
            //RenderTree?.Add(obj);
        }
        public void Remove(I2DRenderable obj)
        {
            if (obj == null)
                return;
            
            _renderables.Remove(obj);
            //RenderTree?.Remove(obj);
        }
        public void Clear(Vec2 bounds)
        {
            //RenderTree = new Quadtree<I2DRenderable>(new BoundingRectangle(new Vec2(0.0f), bounds));
            //_renderables.Clear();
        }

        public override void RegenerateTree()
        {

        }

        public override void GlobalPreRender()
        {

        }

        public override void GlobalSwap()
        {

        }

        public override void GlobalUpdate()
        {

        }
    }
}
