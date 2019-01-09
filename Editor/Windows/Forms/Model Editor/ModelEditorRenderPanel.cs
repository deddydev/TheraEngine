using System;
using TheraEditor.Windows.Forms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;

namespace TheraEngine.Windows.Forms
{
    public class ModelEditorRenderPanel : RenderPanel<BaseScene>
    {
        public DockableModelEditorRenderForm Owner { get; set; }
        public Func<Viewport, IVolume> GetCullingVolumeOverride { get; set; }
        public bool IsEditView { get; set; }

        protected override BaseScene GetScene(Viewport v)
            => Owner.Form.World?.Scene;
        protected override IVolume GetCullingVolume(Viewport v)
            => GetCullingVolumeOverride?.Invoke(v) ?? base.GetCullingVolume(v);
    }
}
