using SlimDX.XInput;
using System;
using System.Collections.Generic;

namespace CustomEngine.Input.Devices.DirectX
{
    public class DXGamepad : Gamepad
    {
        const float ByteDiv = 1.0f / byte.MaxValue;
        const float ShortDiv = 1.0f / short.MaxValue;

        private Controller _controller;

        public DXGamepad(int controllerIndex) : base(controllerIndex) { }

        public override void Vibrate(float left, float right)
        {
            Vibration v = new Vibration();
            v.LeftMotorSpeed = (ushort)(left * ushort.MaxValue);
            v.RightMotorSpeed = (ushort)(right * ushort.MaxValue);
            _controller.SetVibration(v);
        }

        protected override void CreateStates()
        {
            _controller = new Controller(UserIndex.One + _controllerIndex);

            if (!_controller.IsConnected)
                return;

            Capabilities c = _controller.GetCapabilities(DeviceQueryType.Gamepad);

            _buttonStates = new ButtonManager[14];
            _axisStates = new AxisManager[6];

            SetButton(GamePadButton.FaceDown, c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.A));
            SetButton(GamePadButton.FaceRight, c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.B));
            SetButton(GamePadButton.FaceLeft, c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.X));
            SetButton(GamePadButton.FaceUp, c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Y));

            SetButton(GamePadButton.DPadDown, c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadDown));
            SetButton(GamePadButton.DPadRight, c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadRight));
            SetButton(GamePadButton.DPadLeft, c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadLeft));
            SetButton(GamePadButton.DPadUp, c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadUp));

            SetButton(GamePadButton.LeftBumper, c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftShoulder));
            SetButton(GamePadButton.RightBumper, c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.RightShoulder));
            SetButton(GamePadButton.LeftStick, c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftThumb));
            SetButton(GamePadButton.RightStick, c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.RightThumb));
            SetButton(GamePadButton.SpecialLeft, c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Back));
            SetButton(GamePadButton.SpecialRight, c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Start));

            SetAxis(GamePadAxis.LeftTrigger, c.Gamepad.LeftTrigger != 0);
            SetAxis(GamePadAxis.RightTrigger, c.Gamepad.RightTrigger != 0);
            SetAxis(GamePadAxis.LeftThumbstickX, c.Gamepad.LeftThumbX != 0);
            SetAxis(GamePadAxis.LeftThumbstickY, c.Gamepad.LeftThumbY != 0);
            SetAxis(GamePadAxis.RightThumbstickX, c.Gamepad.RightThumbX != 0);
            SetAxis(GamePadAxis.RightThumbstickY, c.Gamepad.RightThumbY != 0);

            _hasCreatedStates = true;
            Console.WriteLine("Gamepad input states created.");
        }
        protected override void UpdateStates(float delta)
        {
            if (!UpdateConnected(_controller.IsConnected))
                return;

            SlimDX.XInput.State state = _controller.GetState();

            FaceDown?.Tick(state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.A), delta);
            FaceRight?.Tick(state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.B), delta);
            FaceLeft?.Tick(state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.X), delta);
            FaceUp?.Tick(state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Y), delta);

            DPadDown?.Tick(state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadDown), delta);
            DPadRight?.Tick(state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadRight), delta);
            DPadLeft?.Tick(state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadLeft), delta);
            DPadUp?.Tick(state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadUp), delta);

            LeftBumper?.Tick(state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftShoulder), delta);
            RightBumper?.Tick(state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.RightShoulder), delta);
            LeftStick?.Tick(state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftThumb), delta);
            RightStick?.Tick(state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.RightThumb), delta);
            SpecialLeft?.Tick(state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Back), delta);
            SpecialRight?.Tick(state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Start), delta);

            LeftTrigger?.Tick(state.Gamepad.LeftTrigger * ByteDiv, delta);
            RightTrigger?.Tick(state.Gamepad.RightTrigger * ByteDiv, delta);
            LeftThumbstickX?.Tick(state.Gamepad.LeftThumbX * ShortDiv, delta);
            LeftThumbstickY?.Tick(state.Gamepad.LeftThumbY * ShortDiv, delta);
            RightThumbstickX?.Tick(state.Gamepad.RightThumbX * ShortDiv, delta);
            RightThumbstickY?.Tick(state.Gamepad.RightThumbY * ShortDiv, delta);
        }
    }
}
