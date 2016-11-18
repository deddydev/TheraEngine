using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Input.Devices.OpenTK
{
    public class DXKeyboard : CKeyboard
    {
        public DXKeyboard(int index) : base(index) { }

        protected override void UpdateStates(float delta)
        {
            throw new NotImplementedException();
        }
    }
}
