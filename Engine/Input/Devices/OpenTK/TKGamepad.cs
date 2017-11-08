using System.Collections.Generic;
using OpenTK.Input;
using System.Linq;

namespace TheraEngine.Input.Devices.OpenTK
{
    public class TKGamepad : BaseGamePad
    {
        public TKGamepad(int index) : base(index) { }

        public override void Vibrate(float left, float right)
        {
            GamePad.SetVibration(_index, left, right);
        }
        private bool ButtonExists(GamePadButton button, GamePadCapabilities c)
        {
            switch (button)
            {
                case GamePadButton.FaceDown: return c.HasAButton;
                case GamePadButton.FaceRight: return c.HasBButton;
                case GamePadButton.FaceLeft: return c.HasXButton;
                case GamePadButton.FaceUp: return c.HasYButton;
                case GamePadButton.DPadDown: return c.HasDPadDownButton;
                case GamePadButton.DPadRight: return c.HasDPadRightButton;
                case GamePadButton.DPadLeft: return c.HasDPadLeftButton;
                case GamePadButton.DPadUp: return c.HasDPadUpButton;
                case GamePadButton.LeftBumper: return c.HasLeftShoulderButton;
                case GamePadButton.RightBumper: return c.HasRightShoulderButton;
                case GamePadButton.LeftStick: return c.HasLeftStickButton;
                case GamePadButton.RightStick: return c.HasRightStickButton;
                case GamePadButton.SpecialLeft: return c.HasBackButton;
                case GamePadButton.SpecialRight: return c.HasStartButton;
            }
            return false;
        }
        protected override bool ButtonExists(GamePadButton button)
        {
            return ButtonExists(button, GamePad.GetCapabilities(_index));
        }
        protected override List<bool> ButtonsExist(List<GamePadButton> buttons)
        {
            GamePadCapabilities c = GamePad.GetCapabilities(_index);
            return buttons.Select(x => ButtonExists(x, c)).ToList();
        }
        private bool AxistExists(GamePadAxis axis, GamePadCapabilities c)
        {
            switch (axis)
            {
                case GamePadAxis.LeftTrigger: return c.HasLeftTrigger;
                case GamePadAxis.RightTrigger: return c.HasRightTrigger;
                case GamePadAxis.LeftThumbstickX: return c.HasLeftXThumbStick;
                case GamePadAxis.LeftThumbstickY: return c.HasLeftYThumbStick;
                case GamePadAxis.RightThumbstickX: return c.HasRightXThumbStick;
                case GamePadAxis.RightThumbstickY: return c.HasRightYThumbStick;
            }
            return false;
        }
        protected override bool AxistExists(GamePadAxis axis)
        {
            return AxistExists(axis, GamePad.GetCapabilities(_index));
        }
        protected override List<bool> AxesExist(List<GamePadAxis> axes)
        {
            GamePadCapabilities c = GamePad.GetCapabilities(_index);
            return axes.Select(x => AxistExists(x, c)).ToList();
        }
        public bool IsPressed(ButtonState state) { return state == ButtonState.Pressed; }
        protected override void UpdateStates(float delta)
        {
            GamePadState state = GamePad.GetState(_index);

            if (!UpdateConnected(state.IsConnected))
                return;

            FaceDown?.Tick(IsPressed(state.Buttons.A), delta);
            FaceRight?.Tick(IsPressed(state.Buttons.B), delta);
            FaceLeft?.Tick(IsPressed(state.Buttons.X), delta);
            FaceUp?.Tick(IsPressed(state.Buttons.Y), delta);
            DPadDown?.Tick(IsPressed(state.DPad.Down), delta);
            DPadRight?.Tick(IsPressed(state.DPad.Right), delta);
            DPadLeft?.Tick(IsPressed(state.DPad.Left), delta);
            DPadUp?.Tick(IsPressed(state.DPad.Up), delta);
            LeftBumper?.Tick(IsPressed(state.Buttons.LeftShoulder), delta);
            RightBumper?.Tick(IsPressed(state.Buttons.RightShoulder), delta);
            LeftStick?.Tick(IsPressed(state.Buttons.LeftStick), delta);
            RightStick?.Tick(IsPressed(state.Buttons.RightStick), delta);
            SpecialLeft?.Tick(IsPressed(state.Buttons.Back), delta);
            SpecialRight?.Tick(IsPressed(state.Buttons.Start), delta);
            LeftTrigger?.Tick(state.Triggers.Left, delta);
            RightTrigger?.Tick(state.Triggers.Right, delta);
            LeftThumbstickX?.Tick(state.ThumbSticks.Left.X, delta);
            LeftThumbstickY?.Tick(state.ThumbSticks.Left.Y, delta);
            RightThumbstickX?.Tick(state.ThumbSticks.Right.X, delta);
            RightThumbstickY?.Tick(state.ThumbSticks.Right.Y, delta);
        }
    }
}
