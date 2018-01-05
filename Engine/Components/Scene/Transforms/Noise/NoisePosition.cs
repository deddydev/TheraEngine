using System.ComponentModel;
using TheraEngine.Input.Devices;

namespace TheraEngine.Worlds.Actors.Components.Scene.Transforms
{
    [FileDef("Translational Noise Component")]
    public class NoisePositionComponent : PositionComponent
    {
        public override void OnSpawned()
        {
            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Logic, Tick, EInputPauseType.TickOnlyWhenUnpaused);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            UnregisterTick(ETickGroup.PrePhysics, ETickOrder.Logic, Tick, EInputPauseType.TickOnlyWhenUnpaused);
            base.OnDespawned();
        }
        private void Tick(float delta)
        {

        }
    }
}
