using System;
using TheraEngine;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Actors;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    //[EditorFor(typeof(TMaterial))]
    public partial class MaterialEditorForm : TheraForm
    {
        private UIMaterialEditor _hud;
        public MaterialEditorForm()
        {
            InitializeComponent();
            //renderPanel1.UI = _hud = new UIMaterialEditor(renderPanel1.ClientSize);
        }
        public MaterialEditorForm(TMaterial m) : this()
        {
            Material = m;
        }
        public TMaterial Material
        {
            get => _hud.TargetMaterial;
            set => _hud.TargetMaterial = value;
        }
    }
}
