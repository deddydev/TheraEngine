﻿using System;

namespace CustomEngine.Input.Gamepads
{
    public class DXGamepadManager : GamepadManager
    {
        public DXGamepadManager() : base() { }

        public override void Vibrate(float left, float right)
        {
            throw new NotImplementedException();
        }

        protected override void CreateStates()
        {
            throw new NotImplementedException();
        }

        protected override void UpdateStates()
        {

        }
    }
}
