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
        public EditorHud(RenderPanel panel) : base(panel)
        {

        }
        protected override void PreConstruct()
        {
            base.PreConstruct();
            //RegisterTick(ETickGroup.PrePhysics, ETickOrder.Logic, Tick);
        }
        public override void RegisterInput(InputInterface input)
        {
            base.RegisterInput(input);
            input.RegisterButtonEvent(GamePadButton.FaceDown, ButtonInputType.Pressed, OnGamepadSelect);
            input.RegisterButtonPressed(EMouseButton.LeftClick, OnLeftClick);
        }
        protected override void OnMouseMove(float x, float y)
        {
            base.OnMouseMove(x, y);
            HighlightScene(false);
        }
        private void OnLeftClick(bool pressed)
        {
            PickScene(false);
        }
        private void OnGamepadSelect()
        {
            PickScene(true);
        }
        private void PickScene(bool gamepad)
        {

        }
        private void HighlightScene(bool gamepad)
        {
            Viewport v = GetViewport();
            if (v == null)
                return;

            SceneComponent comp = v.PickScene(gamepad ? v.Center : v.AbsoluteToRelative(_cursorPos), !gamepad);
            if (comp.OwningActor is EditorTransformTool3D tool)
            {

            }
            else if (comp is HudComponent hudComp)
            {

            }
            else if (comp != null)
            {

            }
        }
    }
}