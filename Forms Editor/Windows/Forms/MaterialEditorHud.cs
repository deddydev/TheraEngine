using TheraEngine.Rendering.HUD;
using TheraEngine.Rendering.Models.Materials;
using System;

namespace TheraEditor.Windows.Forms
{
    public class MaterialEditorHud : HudManager
    {
        public MaterialEditorHud(Vec2 bounds) : base(bounds) { }

        protected override DockableHudComponent OnConstruct()
        {
            DockableHudComponent root = new DockableHudComponent()
            {
                DockStyle = HudDockStyle.Fill,
                SideAnchorFlags = AnchorFlags.Right | AnchorFlags.Left | AnchorFlags.Top | AnchorFlags.Bottom
            };
            ResultBasicFunc end = new ResultBasicFunc();
            root.ChildComponents.Add(end);
            return root;
        }
    }
}
