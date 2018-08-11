using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering;
using TheraEngine.Rendering.UI;

namespace TheraEngine.Actors.Types.Pawns
{
    /// <summary>
    /// Each viewport has a hud manager that manages 2D user interface elements.
    /// </summary>
    public partial class UIManager<T> : Pawn<T> where T : UIDockableComponent, new()
    {
        protected Vec2 _cursorPos = Vec2.Zero;
        private UIComponent _focusedComponent;

        /// <summary>
        /// Returns the cursor position on the screen relative to the the viewport 
        /// controlling this UI or the owning pawn's viewport which uses this UI as its HUD.
        /// </summary>
        public Vec2 CursorPosition()
        {
            Viewport v = OwningPawn?.LocalPlayerController?.Viewport ?? Viewport;
            Point absolute = Cursor.Position;
            BaseRenderPanel panel = v.OwningPanel;
            absolute = panel.PointToClient(absolute);
            Vec2 result = new Vec2(absolute.X, absolute.Y);
            result = v.AbsoluteToRelative(result);
            return result;
        }
        /// <summary>
        /// Returns the cursor position in the world relative to the the viewport 
        /// controlling this UI or the owning pawn's viewport which uses this UI as its HUD.
        /// </summary>
        public Vec2 CursorPositionWorld()
        {
            Viewport v = OwningPawn?.LocalPlayerController?.Viewport ?? Viewport;
            return v.ScreenToWorld(CursorPosition(v)).Xy;
        }
        public Vec2 CursorPositionWorld(Vec2 viewportPosition)
        {
            Viewport v = OwningPawn?.LocalPlayerController?.Viewport ?? Viewport;
            return v.ScreenToWorld(viewportPosition).Xy;
        }
        /// <summary>
        /// Returns the cursor position relative to the the viewport.
        /// </summary>
        public Vec2 CursorPosition(Viewport v)
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
        {
            return v.ScreenToWorld(CursorPosition(v)).Xy;
        }
        public Vec2 CursorPositionWorld(Viewport v, Vec2 viewportPosition)
        {
            return v.ScreenToWorld(viewportPosition).Xy;
        }
        protected override T OnConstruct() => new T() { DockStyle = UIDockStyle.Fill };
        public override void RegisterInput(InputInterface input)
        {
            input.RegisterMouseScroll(OnScrolledInput, EInputPauseType.TickOnlyWhenPaused);
            input.RegisterMouseMove(MouseMove, MouseMoveType.Absolute, EInputPauseType.TickOnlyWhenPaused);
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
    }
}
