using System;

namespace TheraEngine.Input.Devices.DirectX
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
