using System;
using OpenTK.Input;

namespace CustomEngine.Input.Gamepads
{
    public class TKGamepadManager : GamepadManager
    {
        public TKGamepadManager() : base() { }

        public override void Vibrate(float left, float right)
        {
            GamePad.SetVibration(_controllerIndex, left, right);
        }

        protected override void CreateStates()
        {
            GamePadCapabilities c = GamePad.GetCapabilities(_controllerIndex);
            _buttonStates.Add(GamePadButton.FaceDown, new ButtonState(c.HasAButton));
            _buttonStates.Add(GamePadButton.FaceUp, new ButtonState(c.HasYButton));
            _buttonStates.Add(GamePadButton.FaceLeft, new ButtonState(c.HasBButton));
            _buttonStates.Add(GamePadButton.FaceRight, new ButtonState(c.HasXButton));
        }

        protected override void UpdateStates()
        {
            GamePadState state = GamePad.GetState(_controllerIndex);
            _isConnected = state.IsConnected;
            _buttonStates[GamePadButton.FaceUp].Update(state.Buttons.A == OpenTK.Input.ButtonState.Pressed);
        }
    }
}
