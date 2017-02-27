using CustomEngine;
using CustomEngine.Input.Devices;
using CustomEngine.Rendering.HUD;

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
    }
}