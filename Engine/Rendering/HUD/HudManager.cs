using TheraEngine.Rendering.Cameras;
using System.Drawing;
using System;
using System.Collections.Generic;
using TheraEngine.Worlds.Actors;

namespace TheraEngine.Rendering.HUD
{
    //TODO: make base hud component that contains quadtree
    /// <summary>
    /// Each viewport has a hud manager. 
    /// The main form also has a hud manager to overlay over everything if necessary.
    /// </summary>
    public partial class HudManager : Pawn<DockableHudComponent>
    {
        internal LinkedList<HudComponent> _renderables = new LinkedList<HudComponent>();
        internal Quadtree _childComponentTree;
        private Viewport _owningViewport;
        private RenderPanel _owningPanel;
        private OrthographicCamera _camera;
        private bool _visible;

        public OrthographicCamera Camera => _camera;

        public Viewport OwningViewport
        {
            get => _owningViewport;
            set
            {
                _owningViewport = value;
                if (_owningViewport == null)
                    _owningPanel = null;
                else
                    _owningPanel = _owningViewport.OwningPanel;
            }
        }
        public RenderPanel OwningPanel
        {
            get => _owningPanel;
            set
            {
                _owningPanel = value;
                _owningViewport = null;
            }
        }

        public virtual bool Visible
        {
            get => _visible;
            set => _visible = value;
        }

        public HudManager()
        {
            OwningPanel = null;
            _camera = new OrthographicCamera();
            _childComponentTree = null;
        }
        public HudManager(Viewport v)
        {
            OwningViewport = v;
            _camera = new OrthographicCamera();
            _childComponentTree = new Quadtree(_owningViewport.Region.Bounds);
        }
        public HudManager(RenderPanel p)
        {
            OwningPanel = p;
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
            if (!Visible)
                return;
            AbstractRenderer.CurrentCamera = _camera;
            _childComponentTree.DebugRender();
            foreach (HudComponent comp in _renderables)
                if (comp.IsRendering)
                    comp.Render();
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
            if (_renderables.Count == 0)
            {
                _renderables.AddFirst(component);
                return;
            }

            int frontDist = _renderables.First.Value.LayerIndex - component.LayerIndex;
            if (frontDist > 0)
            {
                _renderables.AddFirst(component);
                return;
            }
            
            int backDist = component.LayerIndex - _renderables.Last.Value.LayerIndex;
            if (backDist > 0)
            {
                _renderables.AddLast(component);
                return;
            }
            
            //TODO: check if the following code is right
            if (frontDist < backDist)
            {
                //loop from back
                var last = _renderables.Last;
                while (last.Value.LayerIndex > component.LayerIndex)
                    last = last.Previous;
                _renderables.AddBefore(last, component);
            }
            else
            {
                //loop from front
                var first = _renderables.First;
                while (first.Value.LayerIndex < component.LayerIndex)
                    first = first.Next;
                _renderables.AddAfter(first, component);
            }
        }
    }
}
