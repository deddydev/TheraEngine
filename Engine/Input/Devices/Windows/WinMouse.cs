using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheraEngine.Input.Devices.Windows
{
    public class WinMouse : CMouse
    {
        public WinMouse(int index) : base(index) { }
        
        public override void SetCursorPosition(float x, float y)
        {
            _cursor.Tick(x, y, 0.0f);
            Cursor.Position = new System.Drawing.Point((int)x, (int)y);
        }

        protected override void UpdateStates(float delta)
        {

        }
    }
}
