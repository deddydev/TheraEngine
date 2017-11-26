using TheraEditor.Windows.Forms;
using TheraEngine.GameModes;
using TheraEngine.Worlds.Actors;
using TheraEngine.Worlds.Actors.Types.Pawns;

namespace TheraEditor
{
    public class EditorGameMode : GameMode<FlyingCameraPawn, EditorPlayerController>
    {
        protected override void HandleLocalPlayerJoined(EditorPlayerController item)
        {
            if (item.LocalPlayerIndex != LocalPlayerIndex.One)
                return;

            DockableRenderForm form = DockableRenderForm.ActiveRenderForm;
            if (form != null)
            {
                form.RenderPanel.GetViewport(0)?.RegisterController(item);
                item.ControlledPawn = form.EditorPawn;
            }
        }
        protected override void HandleLocalPlayerLeft(EditorPlayerController item)
        {
            if (item.LocalPlayerIndex != LocalPlayerIndex.One)
                return;

            DockableRenderForm form = DockableRenderForm.ActiveRenderForm;
            if (form != null)
            {
                form.RenderPanel.UnregisterController(item);
                item.ControlledPawn = null;
            }
        }

        protected override void OnBeginGameplay()
        {
            //Engine.World.SpawnActor(new TestCharacter(), new Vec3(-5.0f, 50.0f, -5.0f));
        }
    }
}
