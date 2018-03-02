using TheraEngine.GameModes;

namespace TheraEditor.Windows.Forms
{
    public class MaterialEditorGameMode : GameMode<UIMaterialEditor, MaterialEditorController>
    {
        public DockableMatGraph Editor { get; set; }
        protected override void HandleLocalPlayerJoined(MaterialEditorController item)
        {
            Editor.RenderPanel.GetOrAddViewport(0)?.RegisterController(item);
            item.EnqueuePosession(Editor.RenderPanel.UI);
            item.Viewport.HUD = Editor.RenderPanel.UI;
            item.ViewportCamera = Editor.RenderPanel.UI.Camera;
        }
        protected override void HandleLocalPlayerLeft(MaterialEditorController item)
        {
            Editor.RenderPanel.UnregisterController(item);
            item.UnlinkControlledPawn();
        }
    }
}
