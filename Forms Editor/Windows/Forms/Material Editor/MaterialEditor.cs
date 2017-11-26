using System;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Worlds.Actors;

namespace TheraEditor.Windows.Forms
{
    public partial class MaterialEditor : UserControl, IEditorControl
    {
        UIMaterialEditor _hud;
        public MaterialEditor()
        {
            InitializeComponent();
            renderPanel1.UI = _hud = new UIMaterialEditor(renderPanel1.ClientSize);
        }
        protected override void OnLoad(EventArgs e)
        {
            renderPanel1.RegisterTick();
            base.OnLoad(e);
        }
        public Material Material
        {
            get => _hud.TargetMaterial;
            set => _hud.TargetMaterial = value;
        }

        LocalPlayerIndex IEditorControl.PlayerIndex => LocalPlayerIndex.One;
        BaseRenderPanel IEditorControl.RenderPanel => renderPanel1;
        IPawn IEditorControl.EditorPawn => _hud;
    }
}
