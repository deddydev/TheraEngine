﻿using OpenTK;
using OpenTK.Input;
//using System.Windows.Forms;

namespace TheraEngine.Input.Devices.OpenTK
{
    public class TKRift : BaseRift
    {
        public TKRift() : base() { }
        private OculusRift _rift = new OculusRift();

        protected override void UpdateStates(float delta)
        {
            if (!UpdateConnected(_rift.IsConnected))
                return;
            

        }
    }
}
