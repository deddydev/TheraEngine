using System;
using OpenTK.Input;

namespace CustomEngine.Input.Gamepads
{
    public class TKGamepadManager : GamepadManager
    {
        public TKGamepadManager() : base() { }

        public override void Vibrate(bool left)
        {
            throw new NotImplementedException();
        }

        protected override void CreateStates()
        {
            GamePadCapabilities c = GamePad.GetCapabilities(_controllerIndex);
            _buttonStates.Add(GamePadButton.FaceDown, new ButtonState(c.HasAButton));
            _buttonStates.Add(GamePadButton.FaceUp, new ButtonState(c.HasYButton));
            _buttonStates.Add(GamePadButton.FaceDown, new ButtonState(c.HasAButton));
            _buttonStates.Add(GamePadButton.FaceDown, new ButtonState(c.HasAButton));
        }

        protected override void UpdateStates()
        {
            GamePadState state = GamePad.GetState(_controllerIndex);
            IsConnected = state.IsConnected;
        }
    }
}
