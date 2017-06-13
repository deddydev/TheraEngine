using TheraEngine.Rendering.Cameras;
using TheraEngine.Worlds;
using System;
using System.Collections.Generic;

namespace TheraEngine.Rendering
{
    public enum RenderPass
    {
        OpaqueDeferred,
        OpaqueForward,
        TransparentForward
    }
    [Flags]
    public enum RenderPassFlags
    {
        OpaqueDeferred,
        OpaqueForward,
        TransparentForward
    }
    public class RenderPasses
    {
        private Deque<IRenderable> _opaqueDeferred = new Deque<IRenderable>();
        private Deque<IRenderable> _opaqueForward = new Deque<IRenderable>();
        private Deque<IRenderable> _transparentForward = new Deque<IRenderable>();

        public Deque<IRenderable> OpaqueDeferred => _opaqueDeferred;
        public Deque<IRenderable> OpaqueForward => _opaqueForward;
        public Deque<IRenderable> TransparentForward => _transparentForward;

        public void Render(RenderPass pass)
        {
            switch (pass)
            {
                case RenderPass.OpaqueDeferred:
                    foreach (IRenderable r in OpaqueDeferred)
                        r.Render();
                    OpaqueDeferred.Clear();
                    break;
                case RenderPass.OpaqueForward:
                    foreach (IRenderable r in OpaqueForward)
                        r.Render();
                    OpaqueForward.Clear();
                    break;
                case RenderPass.TransparentForward:
                    foreach (IRenderable r in TransparentForward)
                        r.Render();
                    TransparentForward.Clear();
                    break;
            }
        }
    }
    public class SceneProcessor
    {
        private RenderPasses _passes;
        private RenderOctree _renderTree;
        private LightManager _lightManager;

        internal RenderOctree RenderTree => _renderTree;
        internal LightManager Lights => _lightManager;
        public RenderPasses RenderPasses => _passes;

        internal void WorldChanged()
        {
            if (Engine.World == null)
            {
                _renderTree = null;
                _lightManager = null;
                _passes = null;
                return;
            }
            
            _renderTree = new RenderOctree(Engine.World.Settings.Bounds);
            _lightManager = new LightManager();
            _passes = new RenderPasses();
        }
        internal void Cull(Camera camera)
        {
            AbstractRenderer.CurrentCamera = camera;
            _renderTree.Cull(camera.GetFrustum(), Engine.Settings.RenderOctree, _passes);
            AbstractRenderer.CurrentCamera = null;
        }
        internal void Render(Camera camera, RenderPass pass)
        {
            if (_renderTree == null || camera == null)
                return;

            AbstractRenderer.CurrentCamera = camera;
            _passes.Render(pass);
            AbstractRenderer.CurrentCamera = null;
        }
        public void Add(IRenderable obj)
        {
            _renderTree?.Add(obj);
        }
        public void Remove(IRenderable obj)
        {
            _renderTree?.Remove(obj);
        }
        internal void SetUniforms()
        {
            AbstractRenderer.CurrentCamera.SetUniforms();
            Lights.SetUniforms();
        }
    }
}
