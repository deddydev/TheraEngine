using TheraEditor.Actors.Types.Pawns;
using TheraEditor.Windows.Forms;
using TheraEngine.GameModes;
using TheraEngine.Input;
using TheraEngine.Rendering;

namespace TheraEditor
{
    public class EditorGameMode : GameMode<WorldEditorCameraPawn, EditorPlayerController>
    {
        protected override void HandleLocalPlayerJoined(EditorPlayerController item)
        {
            LinkControllerToViewport(item);
        }
        public override Viewport LinkControllerToViewport(LocalPlayerController item)
        {
            var vp = base.LinkControllerToViewport(item);
            if (vp?.RenderHandler is IEditorRenderHandler c)
                item.ControlledPawn = c.EditorPawn;
            return vp;
        }
        protected override void HandleLocalPlayerLeft(EditorPlayerController item)
        {
            item.Viewport.RenderHandler.UnregisterController(item);
            item.UnlinkControlledPawn();
        }
    }
}
