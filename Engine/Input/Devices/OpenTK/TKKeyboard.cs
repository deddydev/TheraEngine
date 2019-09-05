using OpenTK.Input;
using System;

namespace TheraEngine.Input.Devices.OpenTK
{
    [Serializable]
    public class TKKeyboard : BaseKeyboard
    {
        public TKKeyboard(int index) : base(index) { }

        protected override void UpdateStates(float delta)
        {
            KeyboardState state = Keyboard.GetState();
            if (!UpdateConnected(state.IsConnected))
                return;

            for (int i = 0; i < _registeredKeys.Count; ++i)
            {
                int keyIndex = (int)_registeredKeys[i];
                _buttonStates[keyIndex]?.Tick(state.IsKeyDown((Key)keyIndex), delta);
            }
        }
    }
}
