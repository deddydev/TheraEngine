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
        private bool ButtonExists(EGamePadButton button, GamePadCapabilities c)
        {
            switch (button)
            {
                case EGamePadButton.FaceDown: return c.HasAButton;
                case EGamePadButton.FaceRight: return c.HasBButton;
                case EGamePadButton.FaceLeft: return c.HasXButton;
                case EGamePadButton.FaceUp: return c.HasYButton;
                case EGamePadButton.DPadDown: return c.HasDPadDownButton;
                case EGamePadButton.DPadRight: return c.HasDPadRightButton;
                case EGamePadButton.DPadLeft: return c.HasDPadLeftButton;
                case EGamePadButton.DPadUp: return c.HasDPadUpButton;
                case EGamePadButton.LeftBumper: return c.HasLeftShoulderButton;
                case EGamePadButton.RightBumper: return c.HasRightShoulderButton;
                case EGamePadButton.LeftStick: return c.HasLeftStickButton;
                case EGamePadButton.RightStick: return c.HasRightStickButton;
                case EGamePadButton.SpecialLeft: return c.HasBackButton;
                case EGamePadButton.SpecialRight: return c.HasStartButton;
            }
            return false;
        }
        protected override bool ButtonExists(EGamePadButton button)
        {
            return ButtonExists(button, GamePad.GetCapabilities(_index));
        }
        protected override List<bool> ButtonsExist(List<EGamePadButton> buttons)
        {
            GamePadCapabilities c = GamePad.GetCapabilities(_index);
            return buttons.Select(x => ButtonExists(x, c)).ToList();
        }
        private bool AxistExists(EGamePadAxis axis, GamePadCapabilities c)
        {
            switch (axis)
            {
                case EGamePadAxis.LeftTrigger: return c.HasLeftTrigger;
                case EGamePadAxis.RightTrigger: return c.HasRightTrigger;
                case EGamePadAxis.LeftThumbstickX: return c.HasLeftXThumbStick;
                case EGamePadAxis.LeftThumbstickY: return c.HasLeftYThumbStick;
                case EGamePadAxis.RightThumbstickX: return c.HasRightXThumbStick;
                case EGamePadAxis.RightThumbstickY: return c.HasRightYThumbStick;
            }
            return false;
        }
        protected override bool AxistExists(EGamePadAxis axis)
        {
            return AxistExists(axis, GamePad.GetCapabilities(_index));
        }
        protected override List<bool> AxesExist(List<EGamePadAxis> axes)
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
