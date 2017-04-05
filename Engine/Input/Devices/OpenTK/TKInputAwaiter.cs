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

        public override CGamePad CreateGamepad(int index) { return new TKGamepad(index); }
        public override CKeyboard CreateKeyboard(int index) { return new TKKeyboard(index); }
        public override CMouse CreateMouse(int index) { return new TKMouse(index); }

        protected internal override void Tick(float delta)
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
                    {
                        GamePadCapabilities c = GamePad.GetCapabilities(i);
                        OnFoundGamepad(i);
                    }
                }
            }
            if (keyboards[0] == null)
            {
                KeyboardState keyboardState = Keyboard.GetState();
                if (keyboardState.IsConnected)
                    OnFoundKeyboard(0);
            }
            if (mice[0] == null)
            {
                MouseState mouseState = Mouse.GetState();
                if (mouseState.IsConnected)
                    OnFoundMouse(0);
            }
        }
    }
}
