using System;

namespace TheraEngine.Input.Devices.DirectX
{
    [Serializable]
    public class DXKeyboard : BaseKeyboard
    {
        public DXKeyboard(int index) : base(index) { }

        protected override void UpdateStates(float delta)
        {
            throw new NotImplementedException();
        }
    }
}
