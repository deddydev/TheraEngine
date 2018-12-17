using SharpDX.XInput;
using System.Collections.Generic;
using System.Linq;

namespace TheraEngine.Input.Devices.DirectX
{
    public class DXGamepad : BaseGamePad
    {
        private Controller _controller;

        public DXGamepad(int index) : base(index)
            => _controller = new Controller(UserIndex.One + (byte)index);

        public DXGamepadConfiguration Config { get; set; } = new DXGamepadConfiguration();

        public override void Vibrate(float lowFreq, float highFreq)
            => _controller.SetVibration(new Vibration()
            {
                LeftMotorSpeed = (ushort)(lowFreq * ushort.MaxValue),
                RightMotorSpeed = (ushort)(highFreq * ushort.MaxValue)
            });

        protected override List<bool> ButtonsExist(IEnumerable<EGamePadButton> buttons)
        {
            Capabilities c = _controller.GetCapabilities(DeviceQueryType.Gamepad);
            return buttons.Select(x => ButtonExists(x, c)).ToList();
        }
        protected override List<bool> AxesExist(IEnumerable<EGamePadAxis> axes)
        {
            Capabilities c = _controller.GetCapabilities(DeviceQueryType.Gamepad);
            return axes.Select(x => AxisExists(x, c)).ToList();
        }

        protected override bool ButtonExists(EGamePadButton button)
            => ButtonExists(button, _controller.GetCapabilities(DeviceQueryType.Gamepad));
        protected override bool AxisExists(EGamePadAxis axis)
            => AxisExists(axis, _controller.GetCapabilities(DeviceQueryType.Gamepad));

        private bool ButtonExists(EGamePadButton button, Capabilities c)
        {
            switch (button)
            {
                case EGamePadButton.FaceUp:         return c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Y);
                case EGamePadButton.FaceDown:       return c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.A);
                case EGamePadButton.FaceLeft:       return c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.X);
                case EGamePadButton.FaceRight:      return c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.B);
                case EGamePadButton.DPadUp:         return c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadUp);
                case EGamePadButton.DPadDown:       return c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadDown);
                case EGamePadButton.DPadLeft:       return c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadLeft);
                case EGamePadButton.DPadRight:      return c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadRight);
                case EGamePadButton.LeftBumper:     return c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftShoulder);
                case EGamePadButton.RightBumper:    return c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.RightShoulder);
                case EGamePadButton.LeftStick:      return c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftThumb);
                case EGamePadButton.RightStick:     return c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.RightThumb);
                case EGamePadButton.SpecialLeft:    return c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Back);
                case EGamePadButton.SpecialRight:   return c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Start);
            }
            return false;
        }
        private bool AxisExists(EGamePadAxis axis, Capabilities c)
        {
            switch (axis)
            {
                case EGamePadAxis.LeftTrigger:      return c.Gamepad.LeftTrigger    != 0;
                case EGamePadAxis.RightTrigger:     return c.Gamepad.RightTrigger   != 0;
                case EGamePadAxis.LeftThumbstickX:  return c.Gamepad.LeftThumbX     != 0;
                case EGamePadAxis.LeftThumbstickY:  return c.Gamepad.LeftThumbY     != 0;
                case EGamePadAxis.RightThumbstickX: return c.Gamepad.RightThumbX    != 0;
                case EGamePadAxis.RightThumbstickY: return c.Gamepad.RightThumbY    != 0;
            }
            return false;
        }
        
        protected override void UpdateStates(float delta)
        {
            if (!UpdateConnected(_controller.IsConnected))
                return;

            State state = _controller.GetState();

            for (int i = 0; i < 14; ++i)
                _buttonStates[i]?.Tick(Config.Map((EGamePadButton)i, state.Gamepad), delta);

            for (int i = 0; i < 6; ++i)
                _axisStates[i]?.Tick(Config.Map((EGamePadAxis)i, state.Gamepad), delta);
        }
    }
}
