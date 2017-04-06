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

        public override CGamePad CreateGamepad(int controllerIndex)
            => new DXGamepad(controllerIndex);

        public override CKeyboard CreateKeyboard(int index)
        {
            throw new NotImplementedException();
        }
        public override CMouse CreateMouse(int index)
        {
            throw new NotImplementedException();
        }

        protected internal override void Tick(float delta)
        {
            base.Tick(delta);
        }

        public DXInputAwaiter(Action<InputDevice> uponFound) : base(uponFound) { }
    }
}
