using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Input.Devices
{
    public abstract class InputDevice : ObjectBase
    {
        public static Dictionary<InputDeviceType, InputDevice[]> CurrentDevices =
            new Dictionary<InputDeviceType, InputDevice[]>()
        {
            { InputDeviceType.Gamepad, new InputDevice[4] },
            { InputDeviceType.Keyboard, new InputDevice[4] },
            { InputDeviceType.Mouse, new InputDevice[4] },
        };

        protected ButtonManager[] _buttonStates = new ButtonManager[14];
        protected AxisManager[] _axisStates = new AxisManager[6];

        protected int _index;
        protected bool _isConnected;

        public ConnectedStateChange ConnectionStateChanged;

        public bool IsConnected { get { return _isConnected; } }
        public int Index { get { return _index; } }

        public InputDevice(int index)
        {
            _index = index;
            Console.WriteLine(GetType().ToString() + _index + " created.");
            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Input);
        }
        protected abstract int GetButtonCount();
        protected abstract int GetAxisCount();
        private void ResetStates()
        {
            _buttonStates = new ButtonManager[GetButtonCount()];
            _axisStates = new AxisManager[GetAxisCount()];
        }
        protected abstract void UpdateStates(float delta);
        /// <summary>
        /// Returns true if connected.
        /// </summary>
        protected bool UpdateConnected(bool isConnected)
        {
            if (_isConnected == isConnected)
                return _isConnected;
            _isConnected = isConnected;
            ConnectionStateChanged?.Invoke(_isConnected);
            return _isConnected;
        }
        internal override void Tick(float delta) { UpdateStates(delta); }
    }
}
