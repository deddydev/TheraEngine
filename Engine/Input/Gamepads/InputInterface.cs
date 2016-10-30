using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomEngine.Input.Gamepads
{
    public enum InputType
    {
        Pressed,
        Released,
        Held,
        DoublePressed,
        AxisUpdate,
    }
    /// <summary>
    /// Input for server
    /// </summary>
    public abstract class InputInterface : ObjectBase
    {
        public void Register(GamePadButton button, InputType type, Action func)
        {

        }
        public void Register(GamePadButton button, InputType type, Action<float> func)
        {

        }
        public void Register(GamePadAxis axis, InputType type, Action func)
        {
            
        }
        public void Register(GamePadAxis axis, InputType type, Action<float> func)
        {

        }
    }
}
