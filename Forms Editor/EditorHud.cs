using CustomEngine;
using CustomEngine.Input.Devices;
using CustomEngine.Rendering;
using CustomEngine.Rendering.HUD;
using CustomEngine.Worlds;
using CustomEngine.Worlds.Actors.Types;

namespace TheraEditor
{
    public class EditorHud : HudManager
    {
        public EditorHud(RenderPanel panel) : base(panel)
        {

        }
        public override void RegisterInput(InputInterface input)
        {
            base.RegisterInput(input);
        }
        private void OnMouseMove(float x, float y)
        {

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
        protected override void OnSelect()
        {
            base.OnSelect();
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