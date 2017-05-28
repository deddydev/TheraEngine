using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Input.Devices
{
    public abstract class InputAwaiter : ObjectBase, IDisposable
    {
        public event Action<InputDevice> FoundInput;

        public InputAwaiter(Action<InputDevice> uponFound)
        {
            FoundInput += uponFound;
            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Input, Tick);
        }
        ~InputAwaiter() { Dispose(); }

        public void Dispose() { UnregisterTick(ETickGroup.PrePhysics, ETickOrder.Input, Tick); }
        
        public abstract CGamePad CreateGamepad(int index);
        public abstract CKeyboard CreateKeyboard(int index);
        public abstract CMouse CreateMouse(int index);

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
