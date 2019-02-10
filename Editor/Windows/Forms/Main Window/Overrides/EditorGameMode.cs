using TheraEditor.Actors.Types.Pawns;
using TheraEditor.Windows.Forms;
using TheraEngine.GameModes;

namespace TheraEditor
{
    public class EditorGameMode : GameMode<EditorCameraPawn, EditorPlayerController>
    {
        protected override void HandleLocalPlayerJoined(EditorPlayerController item)
        {
            if (!(Editor.ActiveRenderForm is DockableWorldRenderForm form) || item.LocalPlayerIndex != form.PlayerIndex)
                return;

            form.RenderPanel.GetOrAddViewport(0).RegisterController(item);
            item.ControlledPawn = form.EditorPawn;
        }
        protected override void HandleLocalPlayerLeft(EditorPlayerController item)
        {
            if (!(Editor.ActiveRenderForm is DockableWorldRenderForm form) || item.LocalPlayerIndex != form.PlayerIndex)
                return;

            form.RenderPanel.UnregisterController(item);
            item.ControlledPawn = null;
        }
    }
}
