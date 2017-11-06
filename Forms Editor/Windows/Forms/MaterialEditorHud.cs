using TheraEngine.Rendering.HUD;
using TheraEngine.Rendering.Models.Materials;
using System;

namespace TheraEditor.Windows.Forms
{
    public class MaterialEditorHud : HudManager
    {
        public MaterialEditorHud(Vec2 bounds) : base(bounds) { }

        private ResultBasicFunc _end;
        private ResultBasicFunc End
        {
            get => _end;
            set
            {
                if (_end != null)
                    RootComponent.ChildComponents.Remove(_end);
                _end = value;
                if (_end != null)
                    RootComponent.ChildComponents.Add(_end);
            }
        }

        protected override DockableHudComponent OnConstruct()
        {
            DockableHudComponent root = new DockableHudComponent()
            {
                DockStyle = HudDockStyle.Fill,
                SideAnchorFlags = AnchorFlags.Right | AnchorFlags.Left | AnchorFlags.Top | AnchorFlags.Bottom
            };
            return root;
        }

        private Material _targetMaterial;
        public Material TargetMaterial
        {
            get => _targetMaterial;
            set
            {
                _targetMaterial = value;
                End = _targetMaterial != null ? new ResultBasicFunc(_targetMaterial) : null;
            }
        }
    }
}
