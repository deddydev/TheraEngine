using TheraEngine.ComponentModel;
using TheraEngine.Input.Devices;

namespace TheraEngine.Components.Scene.Transforms
{
    [TFileDef("2D Screen Shake Component")]
    public class ScreenShake2DComponent : NoiseTRComponent
    {
        protected override void OnSpawned()
        {
            RegisterTick(ETickGroup.DuringPhysics, ETickOrder.Logic, Tick, EInputPauseType.TickOnlyWhenUnpaused);
            base.OnSpawned();
        }
        protected override void OnDespawned()
        {
            UnregisterTick(ETickGroup.DuringPhysics, ETickOrder.Logic, Tick, EInputPauseType.TickOnlyWhenUnpaused);
            base.OnDespawned();
        }
        private void Tick(float delta)
        {

        }
    }
}
