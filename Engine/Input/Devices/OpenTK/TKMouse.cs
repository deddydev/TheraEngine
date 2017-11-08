using OpenTK.Input;

namespace TheraEngine.Input.Devices.OpenTK
{
    public class TKMouse : BaseMouse
    {
        public TKMouse(int index) : base(index) { }

        public override void SetCursorPosition(float x, float y)
        {
            _cursor.Tick(x, y, 0.0f);
            Mouse.SetPosition(x, y);
        }

        protected override void UpdateStates(float delta)
        {
            MouseState state = Mouse.GetState();
            if (!UpdateConnected(state.IsConnected))
                return;

            _cursor.Tick(state.X, state.Y, delta);
            _wheel.Tick(state.WheelPrecise, delta);
            LeftClick?.Tick(state.LeftButton == ButtonState.Pressed, delta);
            RightClick?.Tick(state.RightButton == ButtonState.Pressed, delta);
            MiddleClick?.Tick(state.MiddleButton == ButtonState.Pressed, delta);
        }
    }
}
