using TheraEditor.Actors.Types.Pawns;
using TheraEditor.Windows.Forms;
using TheraEngine.GameModes;
using TheraEngine.Input;
using TheraEngine.Rendering;

namespace TheraEditor
{
    public class EditorGameMode : GameMode<EditorCameraPawn, EditorPlayerController>
    {
        protected override void HandleLocalPlayerJoined(EditorPlayerController item)
        {
            LinkControllerToViewport(item);
        }
        public override Viewport LinkControllerToViewport(LocalPlayerController item)
        {
            var vp = base.LinkControllerToViewport(item);
            if (vp?.OwningPanel is IEditorRenderableControl c)
                item.ControlledPawn = c.EditorPawn;
            return vp;
        }
        protected override void HandleLocalPlayerLeft(EditorPlayerController item)
        {
            item.Viewport.OwningPanel.UnregisterController(item);
            item.UnlinkControlledPawn();
        }
    }
}
