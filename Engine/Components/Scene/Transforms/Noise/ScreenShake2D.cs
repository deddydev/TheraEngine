using System.ComponentModel;
using TheraEngine.Input.Devices;

namespace TheraEngine.Worlds.Actors.Components.Scene.Transforms
{
    [FileDef("2D Screen Shake Component")]
    public class ScreenShake2DComponent : NoiseTRComponent
    {
        public override void OnSpawned()
        {
            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Logic, Tick, InputPauseType.TickOnlyWhenUnpaused);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            UnregisterTick(ETickGroup.PrePhysics, ETickOrder.Logic, Tick, InputPauseType.TickOnlyWhenUnpaused);
            base.OnDespawned();
        }
        private void Tick(float delta)
        {

        }
    }
}
