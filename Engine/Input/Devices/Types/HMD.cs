using TheraEngine.Networking;

namespace TheraEngine.Input.Devices
{
    public abstract class BaseHMD : InputDevice
    {
        public BaseHMD() : base(0) { }
        
        protected override int GetAxisCount() => 0; 
        protected override int GetButtonCount() => 3;
        public override EDeviceType DeviceType => EDeviceType.Mouse;
    }
    public enum ERiftButton
    {
        LeftClick   = 0,
        RightClick  = 1,
        MiddleClick = 2,
    }
}
