using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;

namespace CustomEngine.Input.Devices.OpenTK
{
    public class TKMouse : CMouse
    {
        public TKMouse(int index) : base(index) { }

        public override void SetCursorPosition(float x, float y) { Mouse.SetPosition(x, y); }

        protected override void UpdateStates(float delta)
        {
            MouseState state = Mouse.GetState(_index);
            if (!UpdateConnected(state.IsConnected))
                return;


        }
    }
}
