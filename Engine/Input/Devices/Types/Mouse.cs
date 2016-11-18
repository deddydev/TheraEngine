using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;

namespace CustomEngine.Input.Devices
{
    public abstract class CMouse : InputDevice
    {
        public CMouse(int index) : base(index) { }

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
        LeftButton,
        RightButton,
        Keypad1,
        Keypad2,
        Keypad3,
        Keypad4,
        Keypad5,
        Keypad6,
        Keypad7,
        Keypad8,
        Keypad9,
        Keypad10,
        Keypad11,
        Keypad12,
        DpiUp,
        DpiDown,
    }
}
