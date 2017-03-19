using CustomEngine;
using CustomEngine.Input.Devices;
using CustomEngine.Rendering;
using CustomEngine.Rendering.HUD;
using CustomEngine.Worlds;
using CustomEngine.Worlds.Actors.Types;
using System;

namespace TheraEditor
{
    public class EditorHud : HudManager
    {
        public EditorHud(RenderPanel panel) : base(panel)
        {

        }
        protected override void SetDefaults()
        {
            base.SetDefaults();
            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Logic);
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

            IActor actor = v.PickScene(gamepad ? v.Center : v.AbsoluteToRelative(_cursorPos), !gamepad);
            if (actor is EditorTransformTool tool)
            {

            }
            else if (actor is HudComponent hudComp)
            {

            }
            else if (actor != null)
            {

            }
        }
    }
}