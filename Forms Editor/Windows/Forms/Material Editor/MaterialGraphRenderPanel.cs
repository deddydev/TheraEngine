using System;
using System.ComponentModel;
using TheraEditor.Windows.Forms;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine
{
    public class MaterialGraphRenderPanel : RenderPanel<Scene2D>
    {
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public UIMaterialEditor UI { get; }
        
        protected override Scene2D GetScene(Viewport v) => UI?.Scene;
        protected override Camera GetCamera(Viewport v) => UI?.Camera;

        public MaterialGraphRenderPanel()
        {
            UI = new UIMaterialEditor(ClientSize);
            AddViewport();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UI.Resize(ClientSize);
        }
    }
}