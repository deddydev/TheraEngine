using SlimDX.XInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Input.Devices.DirectX
{
    public class DXInputAwaiter : InputAwaiter
    {
        Controller[] _controllers = new Controller[]
        {
            new Controller(UserIndex.One),
            new Controller(UserIndex.Two),
            new Controller(UserIndex.Three),
            new Controller(UserIndex.Four),
        };
        public override Gamepad CreateGamepad(int controllerIndex)
        {
            return new DXGamepad(controllerIndex);
        }
        public override Keyboard CreateKeyboard(int index)
        {
            throw new NotImplementedException();
        }
        public override Mouse CreateMouse(int index)
        {
            throw new NotImplementedException();
        }
        public DXInputAwaiter(Action<InputDevice> uponFound) : base(uponFound) { }
        protected override Dictionary<InputDeviceType, List<int>> GetConnected()
        {
            Dictionary<InputDeviceType, List<int>> connected = new Dictionary<InputDeviceType, List<int>>();
            //for (int i = 0; i < _controllers.Length; ++i)
            //    if (_controllers[i].IsConnected)
            //        connected.Add(i);
            return connected;
        }
    }
}
