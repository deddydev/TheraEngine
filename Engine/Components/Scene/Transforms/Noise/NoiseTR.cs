﻿using TheraEngine.ComponentModel;
using TheraEngine.Input.Devices;

namespace TheraEngine.Components.Scene.Transforms
{
    [TFileDef("Translational & Rotational Noise Component")]
    public class NoiseTRComponent : TRComponent
    {
        public override void OnSpawned()
        {
            RegisterTick(ETickGroup.DuringPhysics, ETickOrder.Logic, Tick, EInputPauseType.TickOnlyWhenUnpaused);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            UnregisterTick(ETickGroup.DuringPhysics, ETickOrder.Logic, Tick, EInputPauseType.TickOnlyWhenUnpaused);
            base.OnDespawned();
        }
        private void Tick(float delta)
        {

        }
    }
}
