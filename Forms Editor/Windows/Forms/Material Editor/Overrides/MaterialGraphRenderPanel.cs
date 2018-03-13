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
        
        protected override Scene2D GetScene(Viewport v) => UI?.UIScene;
        protected override Camera GetCamera(Viewport v) => UI?.Camera;

        public MaterialGraphRenderPanel()
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;
            Viewport v = AddViewport();
            v.HUD = UI = new UIMaterialEditor(ClientSize);
            v.Camera = UI.Camera;
        }
    }
}