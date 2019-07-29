using System;

namespace TheraEngine.Input.Devices.DirectX
{
    [Serializable]
    public class DXMouse : BaseMouse
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
