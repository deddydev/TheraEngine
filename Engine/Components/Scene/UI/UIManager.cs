using TheraEngine.Rendering.Cameras;
using System;
using TheraEngine.Core.Shapes;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering.UI;
using TheraEngine.Rendering;
using TheraEngine.Worlds;

namespace TheraEngine.Actors.Types.Pawns
{
    /// <summary>
    /// Each viewport has a hud manager. 
    /// The main form also has a hud manager to overlay over everything if necessary.
    /// </summary>
    public partial class UIManager : Pawn<UIDockableComponent>//, I3DRenderable
    {
        internal Scene2D _scene;
        private OrthographicCamera _camera;
        private bool _visible = true;
        private Vec2 _bounds;
        private IPawn _owningPawn;

        public OrthographicCamera Camera => _camera;
        
        public virtual bool Visible
        {
            get => _visible;
            set => _visible = value;
        }

        public IPawn OwningPawn
        {
            get => _owningPawn;
            set
            {
                InputInterface input;
                
                if (_owningPawn != null)
                {
                    if (_owningPawn.IsSpawned)
                        Despawned();

                    if (_owningPawn != this && _owningPawn.LocalPlayerController != null)
                    {
                        //Unlink input commands from the owning controller to this hud
                        input = _owningPawn.LocalPlayerController.Input;
                        input.TryUnregisterInput();
                        input.WantsInputsRegistered -= RegisterInput;
                        input.TryRegisterInput();
                    }
                }

                _owningPawn = value;
                if (_owningPawn != null)
                {
                    if (_owningPawn.IsSpawned)
                        Spawned(_owningPawn.OwningWorld);

                    if (_owningPawn != this && _owningPawn.LocalPlayerController != null)
                    {
                        //Link input commands from the owning controller to this hud
                        input = _owningPawn.LocalPlayerController.Input;
                        input.WantsInputsRegistered += RegisterInput;
                        input.TryRegisterInput();
                    }
                }
            }
        }
        
        public RenderInfo3D RenderInfo { get; } 
            = new RenderInfo3D(ERenderPass3D.OnTopForward, null, false, false);

        public Shape CullingVolume => null;
        public IOctreeNode OctreeNode { get; set; }
        public Scene2D Scene => _scene;

        public UIManager() : base()
        {
            _camera = new OrthographicCamera();
            _scene = new Scene2D();
        }
        public UIManager(Vec2 bounds) : this()
        {
            Resize(bounds);
        }

        public void Resize(Vec2 bounds)
        {
            _bounds = bounds;
            if (_bounds == Vec2.Zero)
                return;
            _scene.Resize(bounds);
            _camera.Resize(bounds.X, bounds.Y);
            RootComponent?.Resize(new BoundingRectangle(Vec2.Zero, bounds));
        }
        protected override void PostConstruct()
        {
            base.PostConstruct();
            if (_bounds != Vec2.Zero)
                RootComponent?.Resize(new BoundingRectangle(Vec2.Zero, _bounds));
        }
        //public void Render()
        //{
        //    if (!Visible)
        //        return;
        //    //AbstractRenderer.PushCurrentCamera(_camera);
        //    _scene.Render(AbstractRenderer.CurrentCamera, AbstractRenderer.CurrentCamera.Frustum, null, false);
        //    //AbstractRenderer.PopCurrentCamera();
        //}
        protected void OnChildAdded(UIComponent child)
        {
            child.OwningActor = this;
        }

        //public void Render()
        //{
        //    _scene.DoRender(AbstractRenderer.CurrentCamera, null);
        //}

        internal void RemoveRenderableComponent(I2DRenderable component)
        {
            _scene.Remove(component);

            //_renderables.Remove(component);
        }
        internal void AddRenderableComponent(I2DRenderable component)
        {
            _scene.Add(component);

            //if (_renderables.Count == 0)
            //{
            //    _renderables.AddFirst(component);
            //    return;
            //}

            //int frontDist = _renderables.First.Value.RenderInfo.LayerIndex - component.RenderInfo.LayerIndex;
            //if (frontDist > 0)
            //{
            //    _renderables.AddFirst(component);
            //    return;
            //}

            //int backDist = component.RenderInfo.LayerIndex - _renderables.Last.Value.RenderInfo.LayerIndex;
            //if (backDist > 0)
            //{
            //    _renderables.AddLast(component);
            //    return;
            //}

            ////TODO: check if the following code is right
            //if (frontDist < backDist)
            //{
            //    //loop from back
            //    var last = _renderables.Last;
            //    while (last.Value.RenderInfo.LayerIndex > component.RenderInfo.LayerIndex)
            //        last = last.Previous;
            //    _renderables.AddBefore(last, component);
            //}
            //else
            //{
            //    //loop from front
            //    var first = _renderables.First;
            //    while (first.Value.RenderInfo.LayerIndex < component.RenderInfo.LayerIndex)
            //        first = first.Next;
            //    _renderables.AddAfter(first, component);
            //}
        }
    }
}
