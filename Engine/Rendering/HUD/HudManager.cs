using CustomEngine.Rendering.Cameras;
using System.Drawing;
using System;

namespace CustomEngine.Rendering.HUD
{
    /// <summary>
    /// Each viewport has a hud manager. 
    /// The main form also has a hud manager to overlay over everything if necessary.
    /// </summary>
    public partial class HudManager : DockableHudComponent
    {
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
            _childComponentTree.Resize(region.Bounds);
            //Resize the drawing board
            _camera.Resize(Width, Height);
            return region;
        }
        public void DebugPrint(string message)
        {
            
        }
        public void Render()
        {

        }
        protected override void OnChildAdded(HudComponent child)
        {
            child.Manager = this;
        }
    }
}
