using TheraEngine;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering;
using TheraEngine.Rendering.HUD;
using TheraEngine.Worlds;
using TheraEngine.Worlds.Actors;
using TheraEngine.Worlds.Actors.Types;
using System;

namespace TheraEditor
{
    public class EditorHud : HudManager
    {
        public EditorHud(Vec2 bounds) : base(bounds)
        {

        }
        protected override void PreConstruct()
        {
            base.PreConstruct();
            //RegisterTick(ETickGroup.PrePhysics, ETickOrder.Logic, Tick);
        }
        public override void RegisterInput(InputInterface input)
        {
            //input.RegisterMouseScroll(OnScrolledInput, InputPauseType.TickOnlyWhenPaused);
            input.RegisterMouseMove(OnMouseMove, false, InputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.LeftClick, ButtonInputType.Pressed, OnLeftClickSelect, InputPauseType.TickOnlyWhenPaused);
            
            input.RegisterButtonEvent(GamePadButton.FaceDown, ButtonInputType.Pressed, OnGamepadSelect, InputPauseType.TickOnlyWhenPaused);
            input.RegisterButtonEvent(GamePadButton.FaceRight, ButtonInputType.Pressed, OnBackInput, InputPauseType.TickOnlyWhenPaused);
        }
        protected override void OnMouseMove(float x, float y)
        {
            base.OnMouseMove(x, y);
            HighlightScene(false);
        }
        protected override void OnLeftClickSelect()
        {
            PickScene(false);
        }
        protected override void OnGamepadSelect()
        {
            PickScene(true);
        }
        private void PickScene(bool gamepad)
        {
            if (_highlightedComponent != null)
            {
                if (_highlightedComponent.OwningActor is EditorTransformTool3D tool)
                {

                }
                else if (_highlightedComponent is HudComponent hudComp)
                {

                }
                else// if (comp != null)
                {

                }
            }
        }
        SceneComponent _highlightedComponent;
        private void HighlightScene(bool gamepad)
        {
            Viewport v = Engine.ActivePlayers[0].Viewport;
            if (v != null)
            {
                Vec2 viewportPoint = gamepad ? v.Center : v.AbsoluteToRelative(_cursorPos);
                if (EditorTransformTool3D.CurrentInstance != null)
                {
                    Ray cursor = v.GetWorldRay(viewportPoint);
                    if (EditorTransformTool3D.CurrentInstance.Highlight(cursor, v.Camera, false))
                        _highlightedComponent = EditorTransformTool3D.CurrentInstance.RootComponent;
                }
                if (_highlightedComponent == null)
                {
                    _highlightedComponent = v.PickScene(viewportPoint, !gamepad);
                    if (_highlightedComponent != null)
                    {
                        EditorState state = _highlightedComponent.OwningActor.EditorState;
                        state.Highlighted = true;
                    }
                }
            }
        }
    }
}