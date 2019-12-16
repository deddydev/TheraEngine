using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.UI;

namespace TheraEngine.Actors.Types.Pawns
{
    public interface IUserInterfacePawn : IPawn
    {
        IPawn OwningPawn { get; set; }

        IScene2D ScreenSpaceUIScene { get; }
        OrthographicCamera ScreenOverlayCamera { get; }

        Vec2 Bounds { get; }
        RenderPasses RenderPasses { get; set; }

        Vec2 CursorPosition();
        Vec2 CursorPositionWorld();
        Vec2 ViewportPositionToWorld(Vec2 viewportPosition);
        Vec2 CursorPositionWorld(Viewport v);
        Vec2 CursorPositionWorld(Viewport v, Vec2 viewportPosition);

        void Resize(Vec2 bounds);
        IUIComponent FindDeepestComponent(Vec2 viewportPoint);
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
    public class UserInterfacePawn : UserInterfacePawn<UICanvasComponent>
    {
        public UserInterfacePawn() : base() { }
        public UserInterfacePawn(Vec2 bounds) : base(bounds) { }
    }
    /// <summary>
    /// Each viewport has a HUD that manages 2D user interface elements.
    /// </summary>
    [TFileExt("ui")]
    [TFileDef("User Interface")]
    public class UserInterfacePawn<T> : Pawn<T>, IUserInterfacePawn where T : UICanvasComponent, new()
    {
        public UserInterfacePawn() : base()
        {
            ScreenOverlayCamera = new OrthographicCamera(Vec3.One, Vec3.Zero, Rotator.GetZero(), Vec2.Zero, -0.5f, 0.5f);
            ScreenOverlayCamera.SetOriginBottomLeft();
            ScreenOverlayCamera.Resize(1, 1);

            _screenSpaceUIScene = new Scene2D();
        }
        public UserInterfacePawn(Vec2 bounds) : this()
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
        public IScene2D ScreenSpaceUIScene => _screenSpaceUIScene;
        [Browsable(false)]
        public RenderPasses RenderPasses { get; set; } = new RenderPasses();
        [Browsable(false)]
        public IUIComponent FocusedComponent { get; set; }
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
                        input.InputRegistration -= RegisterInput;
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
                        input.InputRegistration += RegisterInput;
                        input.TryRegisterInput();
                    }
                }
            }
        }
        
        protected override T OnConstructRoot() => new T() { };
        public override void RegisterInput(InputInterface input)
        {
            RootComponent.RegisterInputs(input);
            
            input.RegisterMouseMove(MouseMove, EMouseMoveType.Absolute, EInputPauseType.TickAlways);
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
        public IUIComponent FindDeepestComponent(Vec2 viewportPoint)
            => RootComponent.FindDeepestComponent(viewportPoint, false);
        //UIComponent current = null;
        ////Larger z-indices means the component is closer
        //foreach (UIComponent comp in results)
        //    if (current is null || comp.LayerIndex >= current.LayerIndex)
        //        current = comp;
        //return current;
        //return RootComponent.FindComponent(viewportPoint);

        public virtual void Resize(Vec2 bounds)
        {
            Bounds = bounds;
            if (Bounds == Vec2.Zero)
                return;
            _screenSpaceUIScene.Resize(bounds);
            RootComponent.Resize();
            ScreenOverlayCamera.Resize(bounds.X, bounds.Y);
        }
        protected override void PostConstruct()
        {
            base.PostConstruct();
            if (Bounds != Vec2.Zero)
                RootComponent?.ArrangeChildren(Vec2.Zero, Bounds);
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
            component.RenderInfo.UnlinkScene();

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

        public IUIComponent FindComponent()
            => FindComponent(CursorPositionWorld());
        public IUIComponent FindComponent(Vec2 cursorWorldPos)
            => RootComponent.FindDeepestComponent(cursorWorldPos, false);

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
                RenderContext ctx = v.RenderHandler.Context;
                absolute = ctx.PointToClient(absolute);
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
            return v?.ScreenToWorld(Viewport.CursorPosition(v)).Xy ?? Vec2.Zero;
        }

        public Vec2 CursorPositionWorld(Viewport v)
            => v.ScreenToWorld(Viewport.CursorPosition(v)).Xy;
        public Vec2 CursorPositionWorld(Viewport v, Vec2 viewportPosition)
            => v.ScreenToWorld(viewportPosition).Xy;

        #endregion
    }
}
