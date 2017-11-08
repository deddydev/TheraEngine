using OpenTK.Input;

namespace TheraEngine.Input.Devices.OpenTK
{
    public class TKInputAwaiter : InputAwaiter
    {
        public const int MaxControllers = 4;

        public TKInputAwaiter(DelFoundInput uponFound) : base(uponFound) { }
        
        public override BaseGamePad CreateGamepad(int index) => new TKGamepad(index);
        public override BaseKeyboard CreateKeyboard(int index) => new TKKeyboard(index);
        public override BaseMouse CreateMouse(int index) => new TKMouse(index);

        protected override void Tick(float delta)
        {
            var gamepads = InputDevice.CurrentDevices[InputDeviceType.Gamepad];
            var keyboards = InputDevice.CurrentDevices[InputDeviceType.Keyboard];
            var mice = InputDevice.CurrentDevices[InputDeviceType.Mouse];
            for (int i = 0; i < MaxControllers; ++i)
                if (gamepads[i] == null)
                {
                    GamePadState gamepadState = GamePad.GetState(i);
                    if (gamepadState.IsConnected)
                    {
                        GamePadCapabilities c = GamePad.GetCapabilities(i);
                        OnFoundGamepad(i);
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
