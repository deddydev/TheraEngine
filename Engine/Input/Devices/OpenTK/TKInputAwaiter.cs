using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Input.Devices.OpenTK
{
    public class TKInputAwaiter : InputAwaiter
    {
        public const int MaxControllers = 4;

        public TKInputAwaiter(Action<InputDevice> uponFound) : base(uponFound) { }

        public override CGamePad CreateGamepad(int index)
        {
            return new TKGamepad(index);
        }
        public override CKeyboard CreateKeyboard(int index)
        {
            return new TKKeyboard(index);
        }
        public override CMouse CreateMouse(int index)
        {
            return new TKMouse(index);
        }
        internal override void Tick(float delta)
        {
            var gamepads = InputDevice.CurrentDevices[InputDeviceType.Gamepad];
            var keyboards = InputDevice.CurrentDevices[InputDeviceType.Keyboard];
            var mice = InputDevice.CurrentDevices[InputDeviceType.Mouse];
            for (int i = 0; i < MaxControllers; ++i)
            {
                if (gamepads[i] == null)
                {
                    GamePadState gamepadState = GamePad.GetState(i);
                    if (gamepadState.IsConnected)
                        OnFoundGamepad(i);
                }
                if (keyboards[i] == null)
                {
                    KeyboardState keyboardState = Keyboard.GetState(i);
                    if (keyboardState.IsConnected)
                        OnFoundKeyboard(i);
                }
                if (mice[i] == null)
                {
                    MouseState mouseState = Mouse.GetState(i);
                    if (mouseState.IsConnected)
                        OnFoundMouse(i);
                }
            }
        }
    }
}
