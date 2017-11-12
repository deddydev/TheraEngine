using System.ComponentModel;

namespace TheraEngine.Worlds.Actors.Components.Scene.Transforms
{
    [FileClass("cnoise", "Noise Component")]
    public class NoisePositionComponent : PositionComponent
    {
        public override void OnSpawned()
        {
            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Logic, Tick, Input.Devices.InputPauseType.TickOnlyWhenUnpaused);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            UnregisterTick(ETickGroup.PrePhysics, ETickOrder.Logic, Tick, Input.Devices.InputPauseType.TickOnlyWhenUnpaused);
            base.OnDespawned();
        }
        private void Tick(float delta)
        {

        }
    }
}
