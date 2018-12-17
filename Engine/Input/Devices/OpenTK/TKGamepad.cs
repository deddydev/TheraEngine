using OpenTK.Input;
using System.Collections.Generic;
using System.Linq;

namespace TheraEngine.Input.Devices.OpenTK
{
    public class TKGamepad : BaseGamePad
    {
        public TKGamepad(int index) : base(index) { }

        public TKGamepadConfiguration Config { get; set; } = new TKGamepadConfiguration();

        public override void Vibrate(float left, float right)
            => GamePad.SetVibration(_index, left, right);

        protected override List<bool> ButtonsExist(IEnumerable<EGamePadButton> buttons)
        {
            GamePadCapabilities c = GamePad.GetCapabilities(_index);
            return buttons.Select(x => ButtonExists(x, c)).ToList();
        }
        protected override List<bool> AxesExist(IEnumerable<EGamePadAxis> axes)
        {
            GamePadCapabilities c = GamePad.GetCapabilities(_index);
            return axes.Select(x => AxisExists(x, c)).ToList();
        }

        protected override bool ButtonExists(EGamePadButton button)
            => ButtonExists(button, GamePad.GetCapabilities(_index));
        protected override bool AxisExists(EGamePadAxis axis)
            => AxisExists(axis, GamePad.GetCapabilities(_index));

        public static bool ButtonExists(EGamePadButton button, GamePadCapabilities c)
        {
            switch (button)
            {
                case EGamePadButton.FaceDown:       return c.HasAButton;
                case EGamePadButton.FaceRight:      return c.HasBButton;
                case EGamePadButton.FaceLeft:       return c.HasXButton;
                case EGamePadButton.FaceUp:         return c.HasYButton;
                case EGamePadButton.DPadDown:       return c.HasDPadDownButton;
                case EGamePadButton.DPadRight:      return c.HasDPadRightButton;
                case EGamePadButton.DPadLeft:       return c.HasDPadLeftButton;
                case EGamePadButton.DPadUp:         return c.HasDPadUpButton;
                case EGamePadButton.LeftBumper:     return c.HasLeftShoulderButton;
                case EGamePadButton.RightBumper:    return c.HasRightShoulderButton;
                case EGamePadButton.LeftStick:      return c.HasLeftStickButton;
                case EGamePadButton.RightStick:     return c.HasRightStickButton;
                case EGamePadButton.SpecialLeft:    return c.HasBackButton;
                case EGamePadButton.SpecialRight:   return c.HasStartButton;
            }
            return false;
        }
        public static bool AxisExists(EGamePadAxis axis, GamePadCapabilities c)
        {
            switch (axis)
            {
                case EGamePadAxis.LeftTrigger:      return c.HasLeftTrigger;
                case EGamePadAxis.RightTrigger:     return c.HasRightTrigger;
                case EGamePadAxis.LeftThumbstickX:  return c.HasLeftXThumbStick;
                case EGamePadAxis.LeftThumbstickY:  return c.HasLeftYThumbStick;
                case EGamePadAxis.RightThumbstickX: return c.HasRightXThumbStick;
                case EGamePadAxis.RightThumbstickY: return c.HasRightYThumbStick;
            }
            return false;
        }

        protected override void UpdateStates(float delta)
        {
            GamePadState state = GamePad.GetState(_index);

            if (!UpdateConnected(state.IsConnected))
                return;

            for (int i = 0; i < 14; ++i)
                _buttonStates[i]?.Tick(Config.Map((EGamePadButton)i, state), delta);

            for (int i = 0; i < 6; ++i)
                _axisStates[i]?.Tick(Config.Map((EGamePadAxis)i, state), delta);
        }
    }
}
