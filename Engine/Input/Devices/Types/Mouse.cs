using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Input.Devices
{
    public abstract class CMouse : InputDevice
    {
        public CMouse(int index) : base(index) { }

        public abstract void SetCursorPosition(float x, float y);

        protected override int GetAxisCount() { return 0; }
        protected override int GetButtonCount() { return 0; }

        public void RegisterButtonPressed(EMouseButton button, Action<bool> func)
        {

        }
        public void RegisterButtonEvent(EMouseButton button, ButtonInputType type, Action func)
        {

        }
        public void RegisterScroll(Action<bool> func)
        {

        }
    }
    public enum EMouseButton
    {
        LeftClick,
        RightClick,
        MiddleClick,
        ScrollUp,
        ScrollDown,
    }
}
