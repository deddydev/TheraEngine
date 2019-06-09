namespace TheraEngine.Input.Devices
{
    public delegate void DelFoundInput(InputDevice device);
    public abstract class InputAwaiter : TObject
    {
        public event DelFoundInput FoundInput;

        public InputAwaiter(DelFoundInput uponFound)
        {
            FoundInput += uponFound;
            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Input, Tick);
        }
        //~InputAwaiter() { Dispose(); }

        //public void Dispose()
        //{
        //    UnregisterTick(ETickGroup.PrePhysics, ETickOrder.Input, Tick);
        //}
        
        public abstract BaseGamePad CreateGamepad(int index);
        public abstract BaseKeyboard CreateKeyboard(int index);
        public abstract BaseMouse CreateMouse(int index);

        protected void OnFoundGamepad(int index)
        {
            InputDevice device = CreateGamepad(index);
            InputDevice.CurrentDevices[EInputDeviceType.Gamepad][index] = device;
            FoundInput?.Invoke(device);
        }
        protected void OnFoundKeyboard(int index)
        {
            InputDevice device = CreateKeyboard(index);
            InputDevice.CurrentDevices[EInputDeviceType.Keyboard][index] = device;
            FoundInput?.Invoke(device);
        }
        protected void OnFoundMouse(int index)
        {
            InputDevice device = CreateMouse(index);
            InputDevice.CurrentDevices[EInputDeviceType.Mouse][index] = device;
            FoundInput?.Invoke(device);
        }

        protected abstract void Tick(float delta);
    }
}
