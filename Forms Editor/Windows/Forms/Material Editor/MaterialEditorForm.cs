using System;
using TheraEngine;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Actors;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    //[EditorFor(typeof(TMaterial))]
    public partial class MaterialEditorForm : DockContent, IEditorControl
    {
        private UIMaterialEditor _hud;
        public MaterialEditorForm()
        {
            InitializeComponent();
            renderPanel1.UI = _hud = new UIMaterialEditor(renderPanel1.ClientSize);
        }
        public MaterialEditorForm(TMaterial m) : this()
        {
            Material = m;
        }
        protected override void OnGotFocus(EventArgs e)
        {
            Editor.SetActiveEditorControl(this);
            base.OnGotFocus(e);
        }
        protected override void OnLoad(EventArgs e)
        {
            renderPanel1.RegisterTick();
            base.OnLoad(e);
        }
        public TMaterial Material
        {
            get => _hud.TargetMaterial;
            set => _hud.TargetMaterial = value;
        }
        LocalPlayerIndex IEditorControl.PlayerIndex => LocalPlayerIndex.One;
        BaseRenderPanel IEditorControl.RenderPanel => renderPanel1;
        IPawn IEditorControl.EditorPawn => _hud;
    }
}
