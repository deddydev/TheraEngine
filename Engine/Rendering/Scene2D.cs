using TheraEngine.Rendering.Cameras;
using System;
using System.Collections.Generic;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Rendering
{
    public class RenderPasses2D
    {
        public RenderPasses2D()
        {
            _sorter = new RenderSort();
            _passes = new List<I2DRenderable>[]
            {
                new List<I2DRenderable>(),
                new List<I2DRenderable>(),
                new List<I2DRenderable>(),
                new List<I2DRenderable>(),
            };
        }

        private RenderSort _sorter;
        private List<I2DRenderable>[] _passes;

        public List<I2DRenderable> Background => _passes[0];
        public List<I2DRenderable> Opaque => _passes[1];
        public List<I2DRenderable> Transparent => _passes[2];
        public List<I2DRenderable> OnTop => _passes[3];
        
        private class RenderSort : IComparer<I2DRenderable>
        {
            int IComparer<I2DRenderable>.Compare(I2DRenderable x, I2DRenderable y)
            {
                if (x.RenderInfo.LayerIndex < y.RenderInfo.LayerIndex)
                    return -1;
                if (x.RenderInfo.LayerIndex > y.RenderInfo.LayerIndex)
                    return 1;
                if (x.RenderInfo.IndexWithinLayer < y.RenderInfo.IndexWithinLayer)
                    return -1;
                if (x.RenderInfo.IndexWithinLayer > y.RenderInfo.IndexWithinLayer)
                    return 1;
                return 0;
            }
        }

        public void Render(ERenderPass2D pass)
        {
            var list = _passes[(int)pass];
            list.ForEach(x =>
            {
                x.Render();
                x.RenderInfo.LastRenderedTime = DateTime.Now;
            });
            list.Clear();
        }

        public void Add(I2DRenderable item)
        {
            List<I2DRenderable> r = _passes[(int)item.RenderInfo.RenderPass];
            r.Add(item);
        }

        public void Sort()
        {
            foreach (var list in _passes)
                list.Sort(_sorter);
        }
    }
    /// <summary>
    /// Processes all scene information that will be sent to the renderer.
    /// </summary>
    public class Scene2D : Scene
    {
        public Quadtree<I2DRenderable> RenderTree { get; private set; }
        public override int Count => RenderTree.Count;
        private RenderPasses2D _passes = new RenderPasses2D();

        //private List<I2DRenderable> _renderables = new List<I2DRenderable>();

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

        public override void CollectVisibleRenderables(Frustum frustum, bool shadowPass)
        {
            CollectVisibleRenderables();
        }
        public void CollectVisibleRenderables()
        {
            CollectVisibleRenderables(RenderTree.Bounds);
            //CollectVisibleRenderables(BoundingRectangle.Empty);
        }
        public void CollectVisibleRenderables(BoundingRectangle bounds)
        {
            RenderTree.CollectVisible(bounds, _passes);

            //foreach (I2DRenderable r in _renderables)
            //    _passes.Add(r);
            _passes.Sort();
        }
        
        public void DoRender(Camera c, Viewport v, MaterialFrameBuffer target)
        {
            target?.Bind(EFramebufferTarget.DrawFramebuffer);

            AbstractRenderer.PushCurrentCamera(c);
            AbstractRenderer.PushCurrent2DScene(this);
            {
                foreach (IPreRendered p in _preRenderList)
                    p.PreRender(c);

                if (v != null)
                {
                    //Render the to the actual screen resolution
                    Engine.Renderer.PushRenderArea(v.Region);
                    {
                        //Engine.Renderer.Clear(EBufferClear.Color | EBufferClear.Depth);

                        Engine.Renderer.AllowDepthWrite(false);
                        _passes.Render(ERenderPass2D.Background);

                        Engine.Renderer.AllowDepthWrite(true);
                        _passes.Render(ERenderPass2D.Opaque);

                        RenderTree.DebugRender(true, v.Region, 1.0f);

                        _passes.Render(ERenderPass2D.Transparent);

                        //Disable depth fail for objects on top
                        Engine.Renderer.DepthFunc(EComparison.Always);

                        _passes.Render(ERenderPass2D.OnTop);
                    }
                    Engine.Renderer.PopRenderArea();
                }
                else
                {
                    //Engine.Renderer.Clear(EBufferClear.Color | EBufferClear.Depth);

                    Engine.Renderer.AllowDepthWrite(false);
                    _passes.Render(ERenderPass2D.Background);

                    Engine.Renderer.AllowDepthWrite(true);
                    _passes.Render(ERenderPass2D.Opaque);
                    _passes.Render(ERenderPass2D.Transparent);

                    //Disable depth fail for objects on top
                    Engine.Renderer.DepthFunc(EComparison.Always);
                    _passes.Render(ERenderPass2D.OnTop);
                }
            }
            AbstractRenderer.PopCurrent2DScene();
            AbstractRenderer.PopCurrentCamera();

            target?.Unbind(EFramebufferTarget.DrawFramebuffer);
        }
        
        public void Resize(Vec2 bounds)
        {
            RenderTree?.Remake(new BoundingRectangle(Vec2.Zero, bounds));
        }

        public void Add(I2DRenderable obj)
        {
            //_renderables.Add(obj);
            RenderTree?.Add(obj);
        }
        public void Remove(I2DRenderable obj)
        {
            //_renderables.Remove(obj);
            RenderTree?.Remove(obj);
        }
        public void Clear(Vec2 bounds)
        {
            RenderTree = new Quadtree<I2DRenderable>(new BoundingRectangle(new Vec2(0.0f), bounds));
            //_renderables.Clear();
            _passes = new RenderPasses2D();
        }
    }
}
