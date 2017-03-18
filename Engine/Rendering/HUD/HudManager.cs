using CustomEngine.Rendering.Cameras;
using System.Drawing;
using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering.HUD
{
    /// <summary>
    /// Each viewport has a hud manager. 
    /// The main form also has a hud manager to overlay over everything if necessary.
    /// </summary>
    public partial class HudManager : DockableHudComponent
    {
        internal List<HudComponent> _renderables = new List<HudComponent>();
        internal Quadtree _childComponentTree;
        private Viewport _owningViewport;
        private RenderPanel _owningPanel;
        private OrthographicCamera _camera;
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

        public override BoundingRectangle Resize(BoundingRectangle parentRegion)
        {
            //base.Resize will handle resizing child components
            BoundingRectangle region = base.Resize(parentRegion);
            //Child tree must be resized AFTER child components are resized
            //_childComponentTree.Resize(region.Bounds);
            //Resize the drawing board
            _camera.Resize(Width, Height);
            return region;
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
        protected override void OnChildAdded(HudComponent child)
        {
            child.Manager = this;
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
