﻿using System;

namespace TheraEngine.Input.Devices.DirectX
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
