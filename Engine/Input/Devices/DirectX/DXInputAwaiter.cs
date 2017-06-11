using TheraEngine.Players;
using SlimDX.XInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Worlds.Actors;

namespace TheraEngine.Input.Devices.DirectX
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

        protected override void Tick(float delta)
        {
            throw new NotImplementedException();
        }
        
        public DXInputAwaiter(DelFoundInput uponFound) : base(uponFound) { }
    }
}
