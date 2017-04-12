using CustomEngine.Rendering.Cameras;
using System.Drawing;
using System;
using System.Collections.Generic;
using CustomEngine.Worlds.Actors;

namespace CustomEngine.Rendering.HUD
{
    //TODO: make base hud component that contains quadtree
    /// <summary>
    /// Each viewport has a hud manager. 
    /// The main form also has a hud manager to overlay over everything if necessary.
    /// </summary>
    public partial class HudManager : Pawn<DockableHudComponent>
    {
        internal List<HudComponent> _renderables = new List<HudComponent>();
        internal Quadtree _childComponentTree;
        private Viewport _owningViewport;
        private RenderPanel _owningPanel;
        private OrthographicCamera _camera;

        public OrthographicCamera Camera => _camera;

        public HudManager(Viewport v)
        {
            _owningViewport = v;
            _owningPanel = _owningViewport.OwningPanel;
            _camera = new OrthographicCamera();
            _childComponentTree = new Quadtree(_owningViewport.Region.Bounds);
        }
        public HudManager(RenderPanel p)
        {
            _owningViewport = null;
            _owningPanel = p;
            _camera = new OrthographicCamera();
            _childComponentTree = new Quadtree(_owningPanel.ClientSize);
        }

        public void Resize(Vec2 bounds)
        {
            _camera.Resize(bounds.X, bounds.Y);
            RootComponent.Resize(new BoundingRectangle(Vec2.Zero, bounds));
        }
        public void DebugPrint(string message)
        {
            
        }
        public void Render()
        {
            AbstractRenderer.CurrentCamera = _camera;
            _childComponentTree.DebugRender();
            var e = _renderables.GetEnumerator();
            while (e.MoveNext())
                if (e.Current.IsRendering)
                    e.Current.Render();
            AbstractRenderer.CurrentCamera = null;
        }
        protected void OnChildAdded(HudComponent child)
        {
            child.Owner = this;
        }

        internal void UncacheComponent(HudComponent component)
        {
            _childComponentTree.Remove(component);
            _renderables.Remove(component);
        }
        internal void CacheComponent(HudComponent component)
        {
            _childComponentTree.Add(component);
            _renderables.Add(component);
        }
    }
}
