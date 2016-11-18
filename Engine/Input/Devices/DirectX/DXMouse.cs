using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Input.Devices.OpenTK
{
    public class DXMouse : CMouse
    {
        public DXMouse(int index) : base(index) { }

        public override void SetCursorPosition(float x, float y)
        {
            throw new NotImplementedException();
        }

        protected override void UpdateStates(float delta)
        {
            throw new NotImplementedException();
        }
    }
}
