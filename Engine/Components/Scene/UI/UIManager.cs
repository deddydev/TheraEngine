using System;
using System.Collections.Generic;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.UI;

namespace TheraEngine.Actors.Types.Pawns
{
    public interface IUIManager : IPawn
    {
        IPawn OwningPawn { get; set; }
        Scene2D UIScene { get; }
        OrthographicCamera Camera { get; }
        Vec2 Bounds { get; }
        RenderPasses RenderPasses { get; set; }

        Vec2 CursorPosition();
        Vec2 CursorPositionWorld();
        Vec2 CursorPositionWorld(Vec2 viewportPosition);
        Vec2 CursorPosition(Viewport v);
        Vec2 CursorPositionWorld(Viewport v);
        Vec2 CursorPositionWorld(Viewport v, Vec2 viewportPosition);

        void Resize(Vec2 bounds);
        UIBoundableComponent FindDeepestComponent(Vec2 viewportPoint);
        List<I2DRenderable> FindAllComponentsIntersecting(Vec2 viewportPoint);
        void RemoveRenderableComponent(I2DRenderable r);
        void AddRenderableComponent(I2DRenderable r);
    }
    /// <summary>
    /// Each viewport has a hud manager. 
    /// The main form also has a hud manager to overlay over everything if necessary.
    /// </summary>
    public partial class UIManager<T> : Pawn<T>, IUIManager where T : UIDockableComponent, new()
    {
        internal Scene2D _scene;
        private IPawn _owningPawn;

        public Vec2 Bounds { get; private set; }
        public OrthographicCamera Camera { get; }
        public RenderInfo3D RenderInfo { get; }
            = new RenderInfo3D(ERenderPass.OnTopForward, false, false);
        public Shape CullingVolume => null;
        public IOctreeNode OctreeNode { get; set; }
        public Scene2D UIScene => _scene;
        public RenderPasses RenderPasses { get; set; } = new RenderPasses();
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

        public UIManager() : base()
        {
            Camera = new OrthographicCamera(Vec3.One, Vec3.Zero, Rotator.GetZero(), Vec2.Zero, -0.5f, 0.5f);
            Camera.SetOriginBottomLeft();
            Camera.Resize(1, 1);
            _scene = new Scene2D();
        }
        public UIManager(Vec2 bounds) : this()
        {
            Resize(bounds);
        }

        public void Resize(Vec2 bounds)
        {
            Bounds = bounds;
            if (Bounds == Vec2.Zero)
                return;
            _scene.Resize(bounds);
            RootComponent.Resize(bounds);
            Camera.Resize(bounds.X, bounds.Y);
        }
        protected override void PostConstruct()
        {
            base.PostConstruct();
            if (Bounds != Vec2.Zero)
                RootComponent?.Resize(Bounds);
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

        public void RemoveRenderableComponent(I2DRenderable component)
        {
            _scene.Remove(component);

            //_renderables.Remove(component);
        }
        public void AddRenderableComponent(I2DRenderable component)
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

        public UIComponent FindComponent()
            => FindComponent(CursorPositionWorld());
        public UIBoundableComponent FindComponent(Vec2 cursorWorldPos)
            => RootComponent.FindDeepestComponent(cursorWorldPos);
    }
}
