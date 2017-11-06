using TheraEngine.Rendering.HUD;
using TheraEngine.Rendering.Models.Materials;
using System;

namespace TheraEditor.Windows.Forms
{
    public class MaterialEditorHud : HudManager
    {
        public MaterialEditorHud(Vec2 bounds) : base(bounds) { }

        private ResultBasicFunc _endFunc;
        private ResultBasicFunc EndFunc
        {
            get => _endFunc;
            set
            {
                if (_endFunc != null)
                {
                    RootComponent.ChildComponents.Remove(_endFunc);
                }
                _endFunc = value;
                if (_endFunc != null)
                {
                    RootComponent.ChildComponents.Add(_endFunc);
                    _endFunc.Material = _targetMaterial;
                }
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
                if (EndFunc != null)
                    EndFunc.Material = _targetMaterial;
            }
        }
    }
}
