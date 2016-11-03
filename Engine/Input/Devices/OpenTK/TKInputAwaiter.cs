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

        public override Gamepad CreateGamepad(int index)
        {
            return new TKGamepad(index);
        }
        public override Keyboard CreateKeyboard(int index)
        {
            return new TKKeyboard(index);
        }
        public override Mouse CreateMouse(int index)
        {
            return new TKMouse(index);
        }

        protected override Dictionary<InputDeviceType, List<int>> GetConnected()
        {
            Dictionary<InputDeviceType, List<int>> connected = new Dictionary<InputDeviceType, List<int>>();
            connected.Add(InputDeviceType.Gamepad, new List<int>());
            for (int i = 0; i < MaxControllers; ++i)
            {
                GamePadState state = GamePad.GetState(i);
                //if (state.IsConnected && !InputDevice.CurrentDevices[InputDeviceType.Gamepad].Contains[i])
                //    connected[InputDeviceType.Gamepad].Add(i);
            }
            return connected;
        }
    }
}
