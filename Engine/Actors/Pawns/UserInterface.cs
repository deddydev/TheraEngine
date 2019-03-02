using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.UI;

namespace TheraEngine.Actors.Types.Pawns
{
    public interface IUserInterface : IPawn
    {
        IPawn OwningPawn { get; set; }

        Scene2D ScreenSpaceUIScene { get; }
        OrthographicCamera ScreenOverlayCamera { get; }

        Vec2 Bounds { get; }
        RenderPasses RenderPasses { get; set; }

        Vec2 CursorPosition();
        Vec2 CursorPositionWorld();
        Vec2 ViewportPositionToWorld(Vec2 viewportPosition);
        Vec2 CursorPositionViewport(Viewport v);
        Vec2 CursorPositionWorld(Viewport v);
        Vec2 CursorPositionWorld(Viewport v, Vec2 viewportPosition);

        void Resize(Vec2 bounds);
        UIBoundableComponent FindDeepestComponent(Vec2 viewportPoint);
        List<I2DRenderable> FindAllComponentsIntersecting(Vec2 viewportPoint);
        void RemoveRenderableComponent(I2DRenderable r);
        void AddRenderableComponent(I2DRenderable r);
    }
    public class UICanvasComponent : UIBoundableComponent
    {
        public enum ECanvasDrawSpace
        {
            /// <summary>
            /// Canvas is drawn on top of the viewport.
            /// </summary>
            Screen,
            /// <summary>
            /// Canvas is drawn in front of the camera.
            /// </summary>
            Camera,
            /// <summary>
            /// Canvas is drawn in the world like any other actor.
            /// Camera is irrelevant.
            /// </summary>
            World,
        }

        //[Browsable(false)]
        //public WorldFileRef<Camera> CanvasCamera { get; }
    }
    public class UserInterface : UserInterface<UICanvasComponent>
    {
        public UserInterface() : base() { }
        public UserInterface(Vec2 bounds) : base(bounds) { }
    }
    /// <summary>
    /// Each viewport has a HUD that manages 2D user interface elements.
    /// </summary>
    [TFileExt("ui")]
    [TFileDef("User Interface")]
    public class UserInterface<T> : Pawn<T>, IUserInterface where T : UICanvasComponent, new()
    {
        public UserInterface() : base()
        {
            ScreenOverlayCamera = new OrthographicCamera(Vec3.One, Vec3.Zero, Rotator.GetZero(), Vec2.Zero, -0.5f, 0.5f);
            ScreenOverlayCamera.SetOriginBottomLeft();
            ScreenOverlayCamera.Resize(1, 1);

            _screenSpaceUIScene = new Scene2D();
        }
        public UserInterface(Vec2 bounds) : this()
        {
            Resize(bounds);
        }

        protected Vec2 _cursorPos = Vec2.Zero;

        internal Scene2D _screenSpaceUIScene;
        private IPawn _owningPawn;

        [Browsable(false)]
        public Vec2 Bounds { get; private set; }
        [Browsable(false)]
        public OrthographicCamera ScreenOverlayCamera { get; }
        [Browsable(false)]
        public Scene2D ScreenSpaceUIScene => _screenSpaceUIScene;
        [Browsable(false)]
        public RenderPasses RenderPasses { get; set; } = new RenderPasses();
        [Browsable(false)]
        public UIComponent FocusedComponent { get; set; }
        [Browsable(false)]
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
        
        protected override T OnConstructRoot() => new T() { };
        public override void RegisterInput(InputInterface input)
        {
            RootComponent.RegisterInputs(input);
            
            input.RegisterMouseMove(MouseMove, EMouseMoveType.Absolute, EInputPauseType.TickOnlyWhenPaused);
            //input.RegisterButtonEvent(EMouseButton.LeftClick, ButtonInputType.Pressed, OnLeftClickSelect, InputPauseType.TickOnlyWhenPaused);

            //input.RegisterAxisUpdate(GamePadAxis.LeftThumbstickX, OnLeftStickX, false, EInputPauseType.TickOnlyWhenPaused);
            //input.RegisterAxisUpdate(GamePadAxis.LeftThumbstickY, OnLeftStickY, false, EInputPauseType.TickOnlyWhenPaused);
            //input.RegisterButtonEvent(GamePadButton.DPadUp, ButtonInputType.Pressed, OnDPadUp, EInputPauseType.TickOnlyWhenPaused);
            //input.RegisterButtonEvent(GamePadButton.FaceDown, ButtonInputType.Pressed, OnGamepadSelect, InputPauseType.TickOnlyWhenPaused);
            //input.RegisterButtonEvent(GamePadButton.FaceRight, ButtonInputType.Pressed, OnBackInput, EInputPauseType.TickOnlyWhenPaused);
        }

        protected virtual void OnLeftStickX(float value) { }
        protected virtual void OnLeftStickY(float value) { }

        /// <summary>
        /// Called on either left click or A button.
        /// Default behavior will OnClick the currently focused/highlighted UI component, if anything.
        /// </summary>
        //protected virtual void OnSelectInput()
        //{
            //_focusedComponent?.OnSelect();
        //}
        protected virtual void OnScrolledInput(bool up)
        {
            //_focusedComponent?.OnScrolled(up);
        }
        protected virtual void OnBackInput()
        {
            //_focusedComponent?.OnBack();
        }
        protected virtual void OnDPadUp()
        {

        }

        protected virtual void MouseMove(float x, float y)
        {
            _cursorPos.X = x;
            _cursorPos.Y = y;
        }

        public List<I2DRenderable> FindAllComponentsIntersecting(Vec2 viewportPoint)
            => new List<I2DRenderable>();//_scene.RenderTree.FindAllIntersecting(viewportPoint);
        public UIBoundableComponent FindDeepestComponent(Vec2 viewportPoint)
        {
            return RootComponent.FindDeepestComponent(viewportPoint);
        }
        //UIComponent current = null;
        ////Larger z-indices means the component is closer
        //foreach (UIComponent comp in results)
        //    if (current == null || comp.LayerIndex >= current.LayerIndex)
        //        current = comp;
        //return current;
        //return RootComponent.FindComponent(viewportPoint);

        public virtual void Resize(Vec2 bounds)
        {
            Bounds = bounds;
            if (Bounds == Vec2.Zero)
                return;
            _screenSpaceUIScene.Resize(bounds);
            RootComponent.Resize(bounds);
            ScreenOverlayCamera.Resize(bounds.X, bounds.Y);
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
            _screenSpaceUIScene.Renderables.Remove(component);

            //_renderables.Remove(component);
        }
        public void AddRenderableComponent(I2DRenderable component)
        {
            component.RenderInfo.LinkScene(component, _screenSpaceUIScene);
            //_screenSpaceUIScene.Add(component);

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

        #region Cursor Position
        /// <summary>
        /// Returns the cursor position on the screen relative to the the viewport 
        /// controlling this UI or the owning pawn's viewport which uses this UI as its HUD.
        /// </summary>
        public Vec2 CursorPosition()
        {
            Viewport v = OwningPawn?.LocalPlayerController?.Viewport ?? Viewport;
            Point absolute = Cursor.Position;
            Vec2 result;
            if (v != null)
            {
                BaseRenderPanel panel = v.OwningPanel;
                absolute = panel.PointToClient(absolute);
                result = new Vec2(absolute.X, absolute.Y);
                result = v.AbsoluteToRelative(result);
            }
            else
                result = new Vec2(absolute.X, absolute.Y);
            return result;
        }
        /// <summary>
        /// Returns a position in the world using a position relative to the viewport
        /// controlling this UI or the owning pawn's viewport which uses this UI as its HUD.
        /// </summary>
        public Vec2 ViewportPositionToWorld(Vec2 viewportPosition)
        {
            Viewport v = OwningPawn?.LocalPlayerController?.Viewport ?? Viewport;
            return v?.ScreenToWorld(viewportPosition).Xy ?? Vec2.Zero;
        }
        /// <summary>
        /// Returns the cursor position in the world relative to the the viewport 
        /// controlling this UI or the owning pawn's viewport which uses this UI as its HUD.
        /// </summary>
        public Vec2 CursorPositionWorld()
        {
            Viewport v = OwningPawn?.LocalPlayerController?.Viewport ?? Viewport;
            return v?.ScreenToWorld(CursorPositionViewport(v)).Xy ?? Vec2.Zero;
        }
        /// <summary>
        /// Returns the cursor position relative to the the viewport.
        /// </summary>
        public Vec2 CursorPositionViewport(Viewport v)
        {
            Point absolute = Cursor.Position;
            if (v == null)
                return new Vec2(absolute.X, absolute.Y);
            BaseRenderPanel panel = v.OwningPanel;
            absolute = panel.PointToClient(absolute);
            Vec2 result = new Vec2(absolute.X, absolute.Y);
            result = v.AbsoluteToRelative(result);
            return result;
        }

        public Vec2 CursorPositionWorld(Viewport v)
            => v.ScreenToWorld(CursorPositionViewport(v)).Xy;
        public Vec2 CursorPositionWorld(Viewport v, Vec2 viewportPosition)
            => v.ScreenToWorld(viewportPosition).Xy;

        #endregion
    }
}
