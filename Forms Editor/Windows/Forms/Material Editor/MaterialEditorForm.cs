using System;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEditor.Windows.Forms
{
    [EditorFor(typeof(Material))]
    public partial class MaterialEditorForm : TheraForm
    {
        public MaterialEditorForm()
        {
            InitializeComponent();
        }
        public MaterialEditorForm(Material m) : this()
        {
            Material = m;
        }
        
        public Material Material
        {
            get => materialEditor1.Material;
            set => materialEditor1.Material = value;
        }
        protected override void OnGotFocus(EventArgs e)
        {
            Editor.SetActiveEditorControl(materialEditor1);
            base.OnGotFocus(e);
        }
    }
}
