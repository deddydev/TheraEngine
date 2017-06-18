using TheraEngine.Input;
using TheraEngine.Rendering.HUD;
using TheraEngine.Worlds;

namespace Testris
{
    public class TetrisHud : HudManager
    {
        public override void OnSpawned(World world)
        {
            base.OnSpawned(world);
        }
        public override void OnPossessed(PawnController possessor)
        {
            base.OnPossessed(possessor);
        }
    }
}