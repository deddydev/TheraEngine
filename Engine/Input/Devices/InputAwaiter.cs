namespace TheraEngine.Input.Devices
{
    public delegate void DelFoundInput(InputDevice device);
    public abstract class InputAwaiter : ObjectBase
    {
        public event DelFoundInput FoundInput;

        public InputAwaiter(DelFoundInput uponFound)
        {
            FoundInput += uponFound;
            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Input, Tick);
        }
        ~InputAwaiter() { Dispose(); }

        public void Dispose()
        {
            UnregisterTick(ETickGroup.PrePhysics, ETickOrder.Input, Tick);
        }
        
        public abstract BaseGamePad CreateGamepad(int index);
        public abstract BaseKeyboard CreateKeyboard(int index);
        public abstract BaseMouse CreateMouse(int index);

        protected void OnFoundGamepad(int index)
        {
            InputDevice device = CreateGamepad(index);
            InputDevice.CurrentDevices[InputDeviceType.Gamepad][index] = device;
            FoundInput?.Invoke(device);
        }
        protected void OnFoundKeyboard(int index)
        {
            InputDevice device = CreateKeyboard(index);
            InputDevice.CurrentDevices[InputDeviceType.Keyboard][index] = device;
            FoundInput?.Invoke(device);
        }
        protected void OnFoundMouse(int index)
        {
            InputDevice device = CreateMouse(index);
            InputDevice.CurrentDevices[InputDeviceType.Mouse][index] = device;
            FoundInput?.Invoke(device);
        }

        protected abstract void Tick(float delta);
    }
}
