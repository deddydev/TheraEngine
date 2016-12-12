using OpenTK.Input;

namespace CustomEngine.Input.Devices.OpenTK
{
    public class TKKeyboard : CKeyboard
    {
        public TKKeyboard(int index) : base(index) { }

        protected override void UpdateStates(float delta)
        {
            KeyboardState state = Keyboard.GetState();
            if (!UpdateConnected(state.IsConnected))
                return;

            foreach (EKey key in _registeredKeys)
                _buttonStates[(int)key].Tick(state.IsKeyDown((Key)(int)key), delta);
        }
    }
}
