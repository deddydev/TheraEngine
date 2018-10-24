using SharpDX.XInput;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.Networking;

namespace TheraEngine.Input.Devices.DirectX
{
    public class DXGamepad : BaseGamePad
    {
        const float ByteDiv = 1.0f / byte.MaxValue;
        const float ShortDiv = 1.0f / short.MaxValue;
        
        private Controller _controller;

        public DXGamepad(int index) : base(index)
        {
            _controller = new Controller(UserIndex.One + (byte)index);
        }

        public override void Vibrate(float lowFreq, float highFreq)
        {
            Vibration v = new Vibration()
            {
                LeftMotorSpeed = (ushort)(lowFreq * ushort.MaxValue),
                RightMotorSpeed = (ushort)(highFreq * ushort.MaxValue)
            };
            _controller.SetVibration(v);
        }
        private bool AxistExists(EGamePadAxis axis, Capabilities c)
        {
            switch (axis)
            {
                case EGamePadAxis.LeftTrigger: return c.Gamepad.LeftTrigger != 0;
                case EGamePadAxis.RightTrigger: return c.Gamepad.RightTrigger != 0;
                case EGamePadAxis.LeftThumbstickX: return c.Gamepad.LeftThumbX != 0;
                case EGamePadAxis.LeftThumbstickY: return c.Gamepad.LeftThumbY != 0;
                case EGamePadAxis.RightThumbstickX: return c.Gamepad.RightThumbX != 0;
                case EGamePadAxis.RightThumbstickY: return c.Gamepad.RightThumbY != 0;
            }
            return false;
        }
        protected override bool AxistExists(EGamePadAxis axis)
        {
            return AxistExists(axis, _controller.GetCapabilities(DeviceQueryType.Gamepad));
        }
        protected override List<bool> AxesExist(List<EGamePadAxis> axes)
        {
            Capabilities c = _controller.GetCapabilities(DeviceQueryType.Gamepad);
            return axes.Select(x => AxistExists(x, c)).ToList();
        }
        private bool ButtonExists(EGamePadButton button, Capabilities c)
        {
            switch (button)
            {
                case EGamePadButton.FaceDown: return c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.A);
                case EGamePadButton.FaceRight: return c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.B);
                case EGamePadButton.FaceLeft: return c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.X);
                case EGamePadButton.FaceUp: return c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Y);
                case EGamePadButton.DPadDown: return c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadDown);
                case EGamePadButton.DPadRight: return c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadRight);
                case EGamePadButton.DPadLeft: return c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadLeft);
                case EGamePadButton.DPadUp: return c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadUp);
                case EGamePadButton.LeftBumper: return c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftShoulder);
                case EGamePadButton.RightBumper: return c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.RightShoulder);
                case EGamePadButton.LeftStick: return c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftThumb);
                case EGamePadButton.RightStick: return c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.RightThumb);
                case EGamePadButton.SpecialLeft: return c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Back);
                case EGamePadButton.SpecialRight: return c.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Start);
            }
            return false;
        }
        protected override bool ButtonExists(EGamePadButton button)
        {
            return ButtonExists(button, _controller.GetCapabilities(DeviceQueryType.Gamepad));
        }
        protected override List<bool> ButtonsExist(List<EGamePadButton> buttons)
        {
            Capabilities c = _controller.GetCapabilities(DeviceQueryType.Gamepad);
            return buttons.Select(x => ButtonExists(x, c)).ToList();
        }
        protected override void UpdateStates(float delta)
        {
            if (!UpdateConnected(_controller.IsConnected))
                return;

            State state = _controller.GetState();

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
