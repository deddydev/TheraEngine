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
            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Input);
        }
        ~InputAwaiter() { Dispose(); }

        public void Dispose() { UnregisterTick(); }

        internal override void Tick(float delta)
        {
            //Dictionary<InputDeviceType, List<int>> connected = GetConnected();
            //List<int> alreadyBound = InputInterface.CurrentInputs.Select(x => x.ControllerIndex).ToList();
            //foreach (InputDeviceType t in connected.Keys)
            //    foreach (int i in connected[t])
            //        if (!alreadyBound.Contains(i))
            //            FoundController?.Invoke(t, i);
        }
        
        public abstract Gamepad CreateGamepad(int index);
        public abstract Keyboard CreateKeyboard(int index);
        public abstract Mouse CreateMouse(int index);

        protected abstract Dictionary<InputDeviceType, List<int>> GetConnected();
    }
}
