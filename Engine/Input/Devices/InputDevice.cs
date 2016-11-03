using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Input.Devices
{
    public abstract class InputDevice : ObjectBase
    {
        public static Dictionary<InputDeviceType, List<InputDevice>> CurrentDevices = new Dictionary<InputDeviceType, List<InputDevice>>()
        {
            { InputDeviceType.Gamepad, new List<InputDevice>() },
            { InputDeviceType.Keyboard, new List<InputDevice>() },
            { InputDeviceType.Mouse, new List<InputDevice>() },
        };

        protected int _index;
        public int Index { get { return _index; } }

        public InputDevice(int index) { _index = index; }

        public List<InputInterface> _registeredInterfaces = new List<InputInterface>();
    }
}
