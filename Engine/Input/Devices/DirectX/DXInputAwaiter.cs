using System;

namespace TheraEngine.Input.Devices.DirectX
{
    [Serializable]
    public class DXInputAwaiter : InputAwaiter
    {
        public const int MaxControllers = 4;

        //Controller[] _controllers = new Controller[]
        //{
        //    new Controller(UserIndex.One),
        //    new Controller(UserIndex.Two),
        //    new Controller(UserIndex.Three),
        //    new Controller(UserIndex.Four),
        //};

        public DXInputAwaiter(DelFoundInput uponFound) : base(uponFound) { }

        public override BaseGamePad CreateGamepad(int controllerIndex)
            => new DXGamepad(controllerIndex);
        public override BaseKeyboard CreateKeyboard(int index)
            => new DXKeyboard(index);
        public override BaseMouse CreateMouse(int index)
            => new DXMouse(index);

        protected override void Tick(float delta)
        {
            //var gamepads = InputDevice.CurrentDevices[InputDeviceType.Gamepad];
            //var keyboards = InputDevice.CurrentDevices[InputDeviceType.Keyboard];
            //var mice = InputDevice.CurrentDevices[InputDeviceType.Mouse];
            //for (int i = 0; i < MaxControllers; ++i)
            //    if (gamepads[i] == null)
            //    {

            //        GamePadState gamepadState = GamePad.GetState(i);
            //        if (gamepadState.IsConnected)
            //        {
            //            GamePadCapabilities c = GamePad.GetCapabilities(i);
            //            OnFoundGamepad(i);
            //        }
            //    }

            //if (keyboards[0] == null)
            //{
            //    KeyboardState keyboardState = Keyboard.GetState();
            //    if (keyboardState.IsConnected)
            //        OnFoundKeyboard(0);
            //}
            //if (mice[0] == null)
            //{
            //    MouseState mouseState = Mouse.GetState();
            //    if (mouseState.IsConnected)
            //        OnFoundMouse(0);
            //}
        }
    }
}
