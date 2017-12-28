using TheraEngine.Input.Devices;
using TheraEngine.Worlds.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using TheraEngine.Rendering.UI;

namespace TheraEngine.Worlds.Actors.Types.Pawns
{
    public partial class UIManager : Pawn<UIDockableComponent>
    {
        private Vec2 _cursorPos = Vec2.Zero;
        private UIComponent _focusedComponent;

        public Vec2 CursorPosition //=> _cursorPos;
        {
            get
            {
                Point absolute = Cursor.Position;
                if (BaseRenderPanel.HoveredPanel != null)
                    absolute = (Point)BaseRenderPanel.HoveredPanel.Invoke(BaseRenderPanel.HoveredPanel.PointToClientDelegate, absolute);
                return new Vec2(absolute.X, absolute.Y);
            }
        }

        protected override UIDockableComponent OnConstruct()
        {
            return new UIDockableComponent()
            {
                DockStyle = HudDockStyle.Fill,
            };
        }
        public override void RegisterInput(InputInterface input)
        {
            input.RegisterMouseScroll(OnScrolledInput, InputPauseType.TickOnlyWhenPaused);
            input.RegisterMouseMove(OnMouseMove, false, InputPauseType.TickOnlyWhenPaused);
            //input.RegisterButtonEvent(EMouseButton.LeftClick, ButtonInputType.Pressed, OnLeftClickSelect, InputPauseType.TickOnlyWhenPaused);

            input.RegisterAxisUpdate(GamePadAxis.LeftThumbstickX, OnLeftStickX, false, InputPauseType.TickOnlyWhenPaused);
            input.RegisterAxisUpdate(GamePadAxis.LeftThumbstickY, OnLeftStickY, false, InputPauseType.TickOnlyWhenPaused);
            input.RegisterButtonEvent(GamePadButton.DPadUp, ButtonInputType.Pressed, OnDPadUp, InputPauseType.TickOnlyWhenPaused);
            //input.RegisterButtonEvent(GamePadButton.FaceDown, ButtonInputType.Pressed, OnGamepadSelect, InputPauseType.TickOnlyWhenPaused);
            input.RegisterButtonEvent(GamePadButton.FaceRight, ButtonInputType.Pressed, OnBackInput, InputPauseType.TickOnlyWhenPaused);
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
        protected void OnScrolledInput(bool up)
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

        protected virtual void OnMouseMove(float x, float y)
        {
            _cursorPos.X = x;
            _cursorPos.Y = y;
        }

        public List<UIComponent> FindAllComponents(Vec2 viewportPoint)
        {
            List<I2DBoundable> results = _scene.RenderTree.FindClosest(viewportPoint);
            return results?.Select(x => (UIComponent)x).ToList();
            //return RootComponent.FindComponent(viewportPoint);
        }
        public UIComponent FindClosestComponent(Vec2 viewportPoint)
        {
            List<UIComponent> results = FindAllComponents(viewportPoint);
            if (results == null)
                return null;
            UIComponent current = null;
            //Larger z-indices means the component is closer
            foreach (UIComponent comp in results)
                if (current == null || comp.LayerIndex >= current.LayerIndex)
                    current = comp;
            return current;
            //return RootComponent.FindComponent(viewportPoint);
        }
    }
}
