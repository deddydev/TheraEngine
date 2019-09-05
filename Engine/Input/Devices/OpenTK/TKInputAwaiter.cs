using OpenTK.Input;
using System;

namespace TheraEngine.Input.Devices.OpenTK
{
    [Serializable]
    public class TKInputAwaiter : InputAwaiter
    {
        public const int MaxControllers = 4;

        public TKInputAwaiter(DelFoundInput uponFound) : base(uponFound) { }
        
        public override BaseGamePad CreateGamepad(int index) => new TKGamepad(index);
        public override BaseKeyboard CreateKeyboard(int index) => new TKKeyboard(index);
        public override BaseMouse CreateMouse(int index) => new TKMouse(index);

        protected override void Tick(float delta)
        {
            var gamepads = InputDevice.CurrentDevices[EInputDeviceType.Gamepad];
            var keyboards = InputDevice.CurrentDevices[EInputDeviceType.Keyboard];
            var mice = InputDevice.CurrentDevices[EInputDeviceType.Mouse];
            for (int i = 0; i < MaxControllers; ++i)
                if (gamepads[i] == null)
                {
                    try
                    {
                        GamePadState gamepadState = GamePad.GetState(i);
                        if (gamepadState.IsConnected)
                        {
                            //GamePadCapabilities c = GamePad.GetCapabilities(i);
                            OnFoundGamepad(i);
                        }
                    }
                    catch { }
                }

            if (keyboards[0] == null)
            {
                try
                {
                    KeyboardState keyboardState = Keyboard.GetState();
                    if (keyboardState.IsConnected)
                        OnFoundKeyboard(0);
                }
                catch { }
            }
            if (mice[0] == null)
            {
                try
                {
                    MouseState mouseState = Mouse.GetState();
                    if (mouseState.IsConnected)
                        OnFoundMouse(0);
                }
                catch { }
            }
        }
    }
}
